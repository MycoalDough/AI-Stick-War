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
    def __init__(self, lr, input_dims, n_actions, checkpoint_dir, name):
        super(RAINBOW_Q_Network, self).__init__()
        self.checkpoint_dir = checkpoint_dir
        self.checkpoint_file = os.path.join(self.checkpoint_dir, name)

        self.input_dims = input_dims[0]  # Corrected to an integer
        self.n_actions = n_actions

        self.fc1 = nn.Sequential(
            nn.Linear(self.input_dims, 128), 
            nn.ReLU(),
        )

        self.advantage_hidden_layer = NoisyLinear(128, 128)
        self.advantage_layer = NoisyLinear(128, n_actions)

        self.value_hidden_layer = NoisyLinear(128, 128)
        self.value_layer = NoisyLinear(128, 1)

        self.optimizer = optim.Adam(self.parameters(), lr=lr)
        self.loss = nn.MSELoss()
        self.device = T.device("cuda" if T.cuda.is_available() else "cpu")
        print(self.device)
        self.to(self.device)

    def forward(self, x: T.Tensor) -> T.Tensor:
        """Forward method implementation."""
        # Feature extraction
        feature = self.fc1(x)

        # Processing through the hidden layers for advantage and value streams
        advantage_hidden = self.advantage_hidden_layer(feature)
        value_hidden = self.value_hidden_layer(feature)

        # Final output calculation for advantage and value
        value = self.value_layer(value_hidden)
        advantage = self.advantage_layer(advantage_hidden)

        # Calculate the Q-values using the advantage and value streams
        q = value + advantage - advantage.mean(dim=-1, keepdim=True)
        
        return q


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

    def disable_noise(self):
        """Reset all noisy layers."""
        self.advantage_hidden_layer.disable_noise()
        self.advantage_layer.disable_noise()
        self.value_hidden_layer.disable_noise()
        self.value_layer.disable_noise()

class Agent:
    def __init__(self, gamma, epsilon, lr, n_actions, input_dims, mem_size, batch_size, checkpoint_name, eps_min=0.01, eps_dec=3e-6, replace=1000, checkpoint_dir='Agents', v_min=-1, v_max=1, atom_size=51):
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
        self.n_step = 7
        self.alpha = 0.2
        self.beta = 0.6
        self.prior_eps = 1e-6

        self.memory = PER.PrioritizedReplayBuffer(obs_dim=self.input_dims[0], size=self.mem_size, batch_size=self.batch_size, alpha=self.alpha)
        self.memory_n = PER.ReplayBuffer(self.input_dims[0], self.mem_size, self.batch_size, self.n_step, self.gamma)
        self.q_eval = RAINBOW_Q_Network(self.lr, input_dims=self.input_dims, n_actions=self.n_actions, checkpoint_dir=self.checkpoint_dir, name=self.checkpoint_name + "_eval")
        self.q_next = RAINBOW_Q_Network(self.lr, input_dims=self.input_dims, n_actions=self.n_actions, checkpoint_dir=self.checkpoint_dir, name=self.checkpoint_name + "_next")
        self.q_next.eval()
        self.optimizer = optim.Adam(self.q_eval.parameters())
        self.USE_EPSILON = True

    def choose_action(self, observation, current, iterations, training):
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
            randomness = "AI"
            if training:
                state = T.tensor(np.array(observation, dtype=np.float32), device=self.device)
                with T.no_grad():
                    action = self.q_eval(state).argmax().item()
            else:
                state = T.tensor([observation], dtype=T.float32).to(self.device)
                with T.no_grad():
                    q_values = self.q_eval(state)
                    action = q_values.argmax().item()
                print(f"Q-values: {q_values}, Chosen action: {action}")


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
        self.q_next.eval()

    def learn(self):
        if len(self.memory) < self.batch_size:
            return

        self.replace_target_network()

        sampled_data = self.memory.sample_batch(beta=self.beta)
        weights = T.FloatTensor(sampled_data["weights"].reshape(-1, 1)).to(self.q_eval.device)
        indices = sampled_data['indices']

        elementwise_loss = self._compute_dqn_loss(sampled_data, gamma=self.gamma)
        
        # N-step returns
        gamma = self.gamma ** self.n_step
        samples_n_step = self.memory_n.sample_batch_from_idxs(indices)
        elementwise_loss = self._compute_dqn_loss(sampled_data, gamma=self.gamma)

        loss = T.mean(elementwise_loss * weights)

        # Update network
        self.optimizer.zero_grad()
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
        device = self.device
        state = T.FloatTensor(samples["obs"]).to(device)
        next_state = T.FloatTensor(samples["next_obs"]).to(device)
        action = T.LongTensor(samples["acts"].reshape(-1, 1)).to(device)
        reward = T.FloatTensor(samples["rews"].reshape(-1, 1)).to(device)
        done = T.FloatTensor(samples["done"].reshape(-1, 1)).to(device)

        # Current Q-values from policy network
        curr_q_value = self.q_eval(state).gather(1, action)
        next_q_value = self.q_next(next_state).gather(  # Double DQN
            1, self.q_eval(next_state).argmax(dim=1, keepdim=True)
        ).detach()
        mask = 1 - done
        target = (reward + gamma * next_q_value * mask).to(device)

        # Calculate DQN loss using smooth L1 loss
        loss = F.huber_loss(curr_q_value, target, reduction="none")

        return loss
