import numpy as np
from model import Agent
from utils import plotLearning
import data
import time
import PER.rainbow as Model


if(__name__ == "__main__"):
    load_checkpoint = False


    agent1 = Model.Agent(gamma=0.99,epsilon=1,lr=4e-4, input_dims=[626],n_actions=25, mem_size=300_000, eps_min=0.01, batch_size=32, checkpoint_name="team1",eps_dec=2e-6,replace=100)
    agent2 = Model.Agent(gamma=0.99,epsilon=1,lr=4e-4, input_dims=[622],n_actions=19, mem_size=300_000, eps_min=0.01, batch_size=32,checkpoint_name="team2",eps_dec=2e-6,replace=100)


    if load_checkpoint:
        agent1.load_models()
        agent2.load_models()


    filename = 'StickWar-DDDQN-Adam-lr4e-4-replace100-1.png'
    num_games = 10000


    def on_connection_established(client_socket):
        print("Connection established")

        min_reward = float('inf')
        max_reward = -float('inf')
        for i in range(num_games):
            done1 = False
            done2 = False
            speed = 0.1



            while not(done1 and done2):
                time.sleep(0.1 / speed) #6 seconds between  actions
                observation = data.get_state()
                action, was_random = agent1.choose_action(observation, i, num_games)
                reward, done1,observation_, speed, who = data.play_step(action)

                if reward > max_reward:
                    max_reward = reward
                if reward < min_reward:
                    min_reward = reward
                
                agent1.store_transition(observation, action, data.normalize_reward(reward), observation_, int(done1))
                agent1.learn()

                observation = data.get_state()
                action, was_random = agent2.choose_action(observation, i, num_games)
                reward, done2,observation_, speed, who = data.play_step(action)

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




        print("- training process over -")
        print("- creating model graph -")
        x = [i+1 for i in range(num_games)]
        #plotLearning(x,scores,eps_history,filename)


           


    data.create_host(on_connection_established)
