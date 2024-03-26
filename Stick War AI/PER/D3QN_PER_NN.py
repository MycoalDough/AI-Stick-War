import torch as T
import torch.nn as nn
import torch.nn.functional as F
import torch.optim as optim
import numpy as np
import os
import PER.PER as PER
import Noisy.noisy_layer as NL
class DuelingDeepQNetwork(nn.Module):
    def __init__(self, lr, input_dims, n_actions, checkpoint_dir, name):
        super(DuelingDeepQNetwork, self).__init__()
        self.checkpoint_dir = checkpoint_dir
        self.checkpoint_file = os.path.join(self.checkpoint_dir, name)


        self.input_dims = input_dims
        self.n_actions = n_actions
       
        self.fc1 = NL.NoisyLinear(*self.input_dims, 512)
        self.V = NL.NoisyLinear(512, 1)
        self.A = NL.NoisyLinear(512, self.n_actions)


        self.optimizer = optim.Adam(self.parameters(), lr=lr)
        self.loss = nn.MSELoss()
        self.device = T.device("cuda" if T.cuda.is_available() else "cpu")
        print(self.device)
        self.to(self.device)


    def forward(self, state):
        flat1 = F.relu(self.fc1(state))
        V = self.V(flat1)
        A = self.A(flat1)


        return V,A
   
    def save_checkpoint(self):
        print("-- saving checkpoint --")
        T.save(self.state_dict(), self.checkpoint_file)


    def load_checkpoint(self):
        print("-- loading checkpoint --")
        self.load_state_dict(T.load(self.checkpoint_file))


    def reset_noise(self):
        """Reset all noisy layers."""
        self.fc1.reset_noise()
        self.V.reset_noise()
        self.A.reset_noise()




class Agent():
    def __init__(self, gamma, epsilon, lr, n_actions, input_dims, mem_size, batch_size,checkpoint_name, eps_min = 0.01, eps_dec=5e-7, replace=1000, checkpoint_dir='Agents'):
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


        self.memory = PER.PER_Memory(mem_size)
        self.q_eval = DuelingDeepQNetwork(self.lr, input_dims=self.input_dims, n_actions=self.n_actions, checkpoint_dir=self.checkpoint_dir, name=self.checkpoint_name+ "_eval")
        self.q_next = DuelingDeepQNetwork(self.lr, input_dims=self.input_dims, n_actions=self.n_actions, checkpoint_dir=self.checkpoint_dir, name=self.checkpoint_name + "_next")


        self.USE_EPSILON = False


    def choose_action(self, observation):
        if(self.USE_EPSILON):
            if(np.random.random() > self.epsilon):
                state = T.tensor([observation], dtype=T.float).to(self.q_eval.device)
                _, advantage = self.q_eval.forward(state)
                action = T.argmax(advantage).item()
                randomness = "AI"
            else:
                action = np.random.choice(self.action_space)
                randomness = "Epsilon"
        else:
            state = T.tensor([observation], dtype=T.float).to(self.q_eval.device)
            _, advantage = self.q_eval.forward(state)
            action = T.argmax(advantage).item()
            randomness = "AI"
        return action, randomness
   
    def store_transition(self, state, action, reward, state_, done):
        experience = state, action, reward, state_, done
        self.memory.store(experience)


    def replace_target_network(self):
        if self.learn_step_counter % self.replace_target_count == 0:
            self.q_next.load_state_dict(self.q_eval.state_dict())


    def decrease_epsilon(self):
        self.epsilon = self.epsilon - self.eps_dec \
            if self.epsilon > self.eps_min else self.eps_min


    def save_models(self):
        self.q_eval.save_checkpoint()
        self.q_next.save_checkpoint()


    def load_models(self):
        self.q_eval.load_checkpoint()
        self.q_next.load_checkpoint()


    def learn(self):
        #if self.memory.mem_cntr < self.batch_size:
        #    return


        tree_idx, minibatch = self.memory.sample(self.batch_size)


        self.q_eval.optimizer.zero_grad()
        self.replace_target_network()


        states = np.zeros((self.batch_size, self.input_dims[0]))
        states_ = np.zeros((self.batch_size, self.input_dims[0]))
        actions, rewards, dones = [], [], []
        
        for i in range(self.batch_size):
            states[i] = minibatch[i][0]
            actions.append(minibatch[i][1])
            rewards.append(minibatch[i][2])
            states_[i] = minibatch[i][3]
            dones.append(minibatch[i][4])


        states = T.tensor(states, dtype=T.float).to(self.q_eval.device)
        states_ = T.tensor(states_, dtype=T.float).to(self.q_eval.device)




        indices = np.arange(self.batch_size, dtype=np.int32)


        V_s, A_s = self.q_eval.forward(states)
        V_s_, A_s_ = self.q_next.forward(states_)


        V_s_eval, A_s_eval = self.q_eval.forward(states_)


        q_pred = T.add(V_s, (A_s - A_s.mean(dim=1, keepdim=True)))[indices, actions]
        q_next = T.add(V_s_, (A_s_ - A_s_.mean(dim=1, keepdim=True)))
        q_eval = T.add(V_s_eval, (A_s_eval - A_s_eval.mean(dim=1, keepdim=True)))


        max_actions = T.argmax(q_eval, dim=1)


        dones_np = np.array(dones)
        dones_tensor = T.tensor(dones_np, dtype=T.bool).to(self.q_eval.device)


        q_next[dones_tensor] = 0.0
        rewards_tensor = T.tensor(rewards, dtype=T.float).to(self.q_eval.device)


        q_target = rewards_tensor + self.gamma * q_next[indices, max_actions]
        absolute_errors = T.abs(q_pred - q_target).cpu().detach().numpy()
        self.memory.batch_update(tree_idx, absolute_errors)




        loss = self.q_eval.loss(q_target, q_pred).to(self.q_eval.device)
        loss.backward()
        self.q_eval.optimizer.step()
        self.learn_step_counter += 1


        self.q_eval.reset_noise()
        self.q_next.reset_noise()


        self.decrease_epsilon()
