import numpy as np
from model import Agent
from utils import plotLearning
import data
import time
import D3QN_NL_PER_NS as Model
from reward_plot import RewardPlotter


if(__name__ == "__main__"):
    load_checkpoint = True
    AI_team1 = False
    AI_team2 = False
    load_graph = False


    agent1 = Model.Agent(gamma=0.99,epsilon=0.05,lr=1e-4, input_dims=[399],n_actions=25, mem_size=400_000, eps_min=0.01, batch_size=32, checkpoint_name="team1",eps_dec=2e-6,replace=100)
    agent2 = Model.Agent(gamma=0.99,epsilon=0.05,lr=1e-4, input_dims=[395],n_actions=19, mem_size=400_000, eps_min=0.01, batch_size=32,checkpoint_name="team2",eps_dec=2e-6,replace=100)


    if load_checkpoint:
        agent1.load_models()
        agent2.load_models()


    filename = 'StickWar-DDDQN-Adam-lr4e-4-replace100-1.png'
    num_games = 100000


    def on_connection_established(client_socket):

        print("Connection established")

        min_reward = float('inf')
        max_reward = -float('inf')
        if load_graph:
            reward1plot = RewardPlotter(figsize=(5, 3), max_points=50, title="Reward (TEAM 1)")
            reward2plot = RewardPlotter(figsize=(5, 3), max_points=50, title="Reward (TEAM 2)")

        for i in range(1, num_games + 1):
            if not(AI_team1 or AI_team2):
                done1 = False
                done2 = False
                speed = 1



                while not(done1 and done2):
                    time.sleep(0.75 / speed) #6 seconds between  actions
                    observation = data.get_state()
                    action, was_random = agent1.choose_action(observation=observation, current=i, iterations=num_games, training=True)
                    reward, done1,observation_, speed, who = data.play_step(action)

                    if load_graph:
                        reward1plot.update(data.normalize_reward(reward))

                    if reward > max_reward:
                        max_reward = reward
                    if reward < min_reward:
                        min_reward = reward
                    
                    agent1.store_transition(observation, action, data.normalize_reward(reward), observation_, int(done1))
                    agent1.learn()

                    observation = data.get_state()
                    action, was_random = agent2.choose_action(observation=observation, current=i, iterations=num_games, training=True)
                    reward, done2,observation_, speed, who = data.play_step(action)

                    if load_graph:
                        reward2plot.update(data.normalize_reward(reward))


                    if reward > max_reward:
                        max_reward = reward
                    if reward < min_reward:
                        min_reward = reward
                    agent2.store_transition(observation, action, data.normalize_reward(reward), observation_, int(done2))
                    agent2.learn()
                        

                


                data.reset()
                print('episode', i, 'epsilon %.2f ' % agent1.epsilon, "| MIN REWARD: ", min_reward, " | MAX REWARD: ", max_reward)
                if i > 10 and i % 10 == 0:
                    agent1.save_models()
                    agent2.save_models()


            elif AI_team1:
                done1 = False
                speed = 1



                while not(done1):
                    time.sleep(0.75 / speed) #6 seconds between  actions
                    observation = data.get_state()
                    action, was_random = agent1.choose_action(observation=observation, current=i, iterations=num_games, training=False)
                    reward, done1,observation_, speed, who = data.play_step(action)

                    if load_graph:
                        reward1plot.update(data.normalize_reward(reward))

                    if reward > max_reward:
                        max_reward = reward
                    if reward < min_reward:
                        min_reward = reward
                data.reset()
                print('episode', i, 'epsilon %.2f ' % agent1.epsilon, "| MIN REWARD: ", min_reward, " | MAX REWARD: ", max_reward)
            elif AI_team2:
                print("AGENT is playing as [CHAOS]")
                done2 = False
                speed = 1



                while not(done2):
                    time.sleep(0.75 / speed) #6 seconds between  actions
                    observation = data.get_state()
                    action, was_random = agent2.choose_action(observation=observation, current=i, iterations=num_games, training=False)
                    reward, done2,observation_, speed, who = data.play_step(action)
                    agent2.q_next.reset_noise()
                    agent2.q_eval.reset_noise()

                        
                data.reset()
                print('episode', i, 'epsilon %.2f ' % agent2.epsilon, "| MIN REWARD: ", min_reward, " | MAX REWARD: ", max_reward)


        print("- training process over -")
        print("- creating model graph -")
        #x = [i+1 for i in range(num_games)]
        #plotLearning(x,scores,eps_history,filename)


           


    data.create_host(on_connection_established)
