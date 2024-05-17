import torch as T
import torch.nn as nn
import torch.nn.functional as F
import torch.optim as optim
import numpy as np
import os
import PER.PER as PER
from Noisy.noisy_layer import NoisyLinear
from typing import Dict
from torch.nn.utils import clip_grad_norm_

class RAINBOW_Q_Network(nn.Module):
    def __init__(self, lr, input_dims, n_actions, checkpoint_dir, name, atom_size, support):
        super(RAINBOW_Q_Network, self).__init__()
        self.checkpoint_dir = checkpoint_dir
        self.checkpoint_file = os.path.join(self.checkpoint_dir, name)

        self.input_dims = input_dims[0]  # Corrected to an integer
        self.n_actions = n_actions
        self.atom_size = atom_size
        self.support = support

        self.fc1 = nn.Sequential(
            nn.Linear(self.input_dims, 128),  # Corrected input dimensions
            nn.ReLU(),
        )

        self.advantage_hidden_layer = NoisyLinear(128, 128)
        self.advantage_layer = NoisyLinear(128, n_actions * atom_size)

        self.value_hidden_layer = NoisyLinear(128, 128)
        self.value_layer = NoisyLinear(128, atom_size)

        self.optimizer = optim.Adam(self.parameters(), lr=lr)
        self.loss = nn.MSELoss()
        self.device = T.device("cuda" if T.cuda.is_available() else "cpu")
        print(self.device)
        self.to(self.device)

    def forward(self, x: T.Tensor) -> T.Tensor:
        """Forward method implementation."""
        dist = self.dist(x)
        q = T.sum(dist * self.support, dim=2)

        return q

    def dist(self, x: T.Tensor) -> T.Tensor:
        """Get distribution for atoms."""
        feature = self.fc1(x)  # Corrected from feature_layer
        adv_hid = F.relu(self.advantage_hidden_layer(feature))
        val_hid = F.relu(self.value_hidden_layer(feature))

        advantage = self.advantage_layer(adv_hid).view(
            -1, self.n_actions, self.atom_size
        )
        value = self.value_layer(val_hid).view(-1, 1, self.atom_size)
        q_atoms = value + advantage - advantage.mean(dim=1, keepdim=True)

        dist = F.softmax(q_atoms, dim=-1)
        dist = dist.clamp(min=1e-3)  # for avoiding nans

        return dist

    def save_checkpoint(self):
        print("-- saving checkpoint --")
        T.save(self.state_dict(), self.checkpoint_file)

    def load_checkpoint(self):
        print("-- loading checkpoint --")
        self.load_state_dict(T.load(self.checkpoint_file))

    def reset_noise(self):
        """Reset all noisy layers."""
        self.advantage_hidden_layer.reset_noise()
        self.advantage_layer.reset_noise()
        self.value_hidden_layer.reset_noise()
        self.value_layer.reset_noise()


class Agent:
    def __init__(self, gamma, epsilon, lr, n_actions, input_dims, mem_size, batch_size, checkpoint_name, eps_min=0.01, eps_dec=5e-7, replace=1000, checkpoint_dir='Agents', v_min=-1, v_max=1, atom_size=51):
        self.device = T.device("cuda" if T.cuda.is_available() else "cpu")
        self.gamma = gamma
        self.epsilon = epsilon
        self.lr = lr
        self.n_actions = n_actions
        self.input_dims = input_dims
        self.mem_size = mem_size
        self.batch_size = batch_size
        self.eps_min = eps_min
        self.eps_dec = eps_dec
        self.replace_target_count = replace
        self.checkpoint_dir = checkpoint_dir
        self.learn_step_counter = 0
        self.action_space = [i for i in range(self.n_actions)]
        self.checkpoint_name = checkpoint_name
        self.n_step = 3
        self.alpha = 0.2
        self.beta = 0.6
        self.prior_eps = 1e-6

        self.v_min = v_min
        self.v_max = v_max
        self.atom_size = atom_size
        self.support = T.linspace(
            self.v_min, self.v_max, self.atom_size
        ).to(self.device)

        self.memory = PER.PrioritizedReplayBuffer(obs_dim=self.input_dims[0], size=self.mem_size, batch_size=self.batch_size, alpha=self.alpha)
        self.memory_n = PER.ReplayBuffer(self.input_dims[0], self.mem_size, self.batch_size, self.n_step, self.gamma)
        self.q_eval = RAINBOW_Q_Network(self.lr, input_dims=self.input_dims, n_actions=self.n_actions, checkpoint_dir=self.checkpoint_dir, name=self.checkpoint_name + "_eval", atom_size=self.atom_size, support=self.support)
        self.q_next = RAINBOW_Q_Network(self.lr, input_dims=self.input_dims, n_actions=self.n_actions, checkpoint_dir=self.checkpoint_dir, name=self.checkpoint_name + "_next", atom_size=self.atom_size, support=self.support)

        self.optimizer = optim.Adam(self.q_eval.parameters())
        self.USE_EPSILON = False

    def choose_action(self, observation, current, iterations):
        if self.USE_EPSILON:
            if np.random.random() > self.epsilon:
                state = T.tensor(np.array(observation, dtype=np.float32), device=self.q_eval.device)
                q_values = self.q_eval.forward(state)
                action = T.argmax(q_values).item()
                randomness = "AI"
            else:
                action = np.random.choice(self.action_space)
                randomness = "Epsilon"
        else:
            state = T.tensor(np.array(observation, dtype=np.float32), device=self.device)
            with T.no_grad():
                action = self.q_eval(state).argmax().item()
            randomness = "AI"

        fraction = min(current / iterations, 1.0)
        self.beta = self.beta + fraction * (1.0 - self.beta)
        return action, randomness

    def store_transition(self, state, action, reward, state_, done):
        experience = [state, action, reward, state_, done]
        self.memory_n.store(*experience)
        self.memory.store(*experience)

    def replace_target_network(self):
        if self.learn_step_counter % self.replace_target_count == 0:
            self.q_next.load_state_dict(self.q_eval.state_dict())

    def decrease_epsilon(self):
        self.epsilon = max(self.eps_min, self.epsilon - self.eps_dec)

    def save_models(self):
        self.q_eval.save_checkpoint()
        self.q_next.save_checkpoint()

    def load_models(self):
        self.q_eval.load_checkpoint()
        self.q_next.load_checkpoint()

    def learn(self):
        if len(self.memory) < self.batch_size:
            return

        self.replace_target_network()

        sampled_data = self.memory.sample_batch(beta=self.beta)
        weights = T.FloatTensor(sampled_data["weights"].reshape(-1, 1)).to(self.q_eval.device)
        indices = sampled_data['indices']

        # Compute C51 loss
        elementwise_loss = self._compute_dqn_loss(sampled_data, gamma=self.gamma)
        loss = T.mean(elementwise_loss * weights)
        
        # N-step returns
        gamma = self.gamma ** self.n_step
        samples_n_step = self.memory_n.sample_batch_from_idxs(indices)
        elementwise_loss_n_step = self._compute_dqn_loss(samples_n_step, gamma)
        elementwise_loss += elementwise_loss_n_step

        loss = T.mean(elementwise_loss * weights)

        # Update network
        self.q_eval.optimizer.zero_grad()
        loss.backward()
        clip_grad_norm_(self.q_eval.parameters(), 10.0)
        self.optimizer.step()
        self.learn_step_counter += 1

        # Update priorities in memory
        loss_for_prior = elementwise_loss.detach().cpu().numpy()
        new_priorities = loss_for_prior + self.prior_eps
        self.memory.update_priorities(indices, new_priorities)

        # Reset noise for Noisy Networks
        self.q_eval.reset_noise()
        self.q_next.reset_noise()

        self.decrease_epsilon()

    def _compute_dqn_loss(self, samples: Dict[str, np.ndarray], gamma: float) -> T.Tensor:
        """Return categorical dqn loss."""
        device = self.q_eval.device  # for shortening the following lines
        state = T.FloatTensor(samples["obs"]).to(device)
        next_state = T.FloatTensor(samples["next_obs"]).to(device)
        action = T.LongTensor(samples["acts"]).to(device)
        reward = T.FloatTensor(samples["rews"].reshape(-1, 1)).to(device)
        done = T.FloatTensor(samples["done"].reshape(-1, 1)).to(device)
        
        # Categorical DQN algorithm
        delta_z = float(self.v_max - self.v_min) / (self.atom_size - 1)

        with T.no_grad():
            # Double DQN
            next_action = self.q_eval(next_state).argmax(1)
            next_dist = self.q_next.dist(next_state)
            next_dist = next_dist[range(self.batch_size), next_action]

            t_z = reward + (1 - done) * gamma * self.support
            t_z = t_z.clamp(min=self.v_min, max=self.v_max)
            b = (t_z - self.v_min) / delta_z
            l = b.floor().long()
            u = b.ceil().long()

            offset = (
                T.linspace(
                    0, (self.batch_size - 1) * self.atom_size, self.batch_size
                ).long()
                .unsqueeze(1)
                .expand(self.batch_size, self.atom_size)
                .to(self.q_eval.device)
            )

            proj_dist = T.zeros(next_dist.size(), device=self.device)
            proj_dist.view(-1).index_add_(
                0, (l + offset).view(-1), (next_dist * (u.float() - b)).view(-1)
            )
            proj_dist.view(-1).index_add_(
                0, (u + offset).view(-1), (next_dist * (b - l.float())).view(-1)
            )

        dist = self.q_eval.dist(state)
        log_p = T.log(dist[range(self.batch_size), action])
        elementwise_loss = -(proj_dist * log_p).sum(1)


        print(elementwise_loss)
        return elementwise_loss