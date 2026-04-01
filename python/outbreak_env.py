import gym
from gym import spaces
import numpy as np
import socket
import json
import time

class OutbreakEnv(gym.Env):
    """Custom Environment that follows gym interface"""
    metadata = {'render.modes': ['human']}

    def __init__(self, host='127.0.0.1', port=9876):
        super(OutbreakEnv, self).__init__()
        # Continuous observation: [infected_count, hospital_occupancy, remaining_budget]
        self.observation_space = spaces.Box(low=np.array([0, 0, -10000]), 
                                            high=np.array([1000, 1000, 10000]), 
                                            dtype=np.float32)
        # Discrete actions: 0=No Action, 1=Regional Lockdown, 2=Targeted Vaccination
        self.action_space = spaces.Discrete(3)
        
        self.host = host
        self.port = port
        self.sock = None
        self.connect_to_unity()

    def connect_to_unity(self):
        self.sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        print(f"Waiting for Unity at {self.host}:{self.port}...")
        while True:
            try:
                self.sock.connect((self.host, self.port))
                print("Connected to Unity!")
                break
            except ConnectionRefusedError:
                time.sleep(1)

    def step(self, action):
        # Send action
        action_payload = json.dumps({"action": int(action)})
        try:
            self.sock.sendall((action_payload + "\n").encode())
        except:
            self.connect_to_unity()
            self.sock.sendall((action_payload + "\n").encode())

        # Receive observation
        obs_data = self.receive_observation()
        if not obs_data:
            return np.zeros(3), 0, True, {}

        infectedCount = obs_data.get("infected_count", 0)
        hospOccup = obs_data.get("hospital_occupancy", 0)
        remBudget = obs_data.get("remaining_budget", 0)
        
        obs = np.array([infectedCount, hospOccup, remBudget], dtype=np.float32)
        done = obs_data.get("done", False)

        # Reward formulation
        # High penalty for taking actions that drain budget below 0
        w_h = 1.0  # health cost
        w_b = 5.0  # budget penalty multiplier
        w_o = 10.0 # hospital overflow penalty
        
        reward = -(infectedCount * w_h)
        if remBudget < 0:
            reward -= (abs(remBudget) * w_b)
        if hospOccup > 50: # max cap
            reward -= ((hospOccup - 50) * w_o)

        return obs, reward, done, {}

    def receive_observation(self):
        try:
            buffer = ""
            while True:
                data = self.sock.recv(1024).decode()
                if not data:
                    break
                buffer += data
                if '\n' in buffer:
                    line = buffer.split('\n')[0]
                    return json.loads(line)
        except Exception as e:
            print(e)
            return None

    def reset(self):
        # Unity handles the reset natively, but RL needs initial state
        print("Resetting environment. Note: Requires Unity side reset logic.")
        return np.zeros(3)

    def render(self, mode='human'):
        pass

    def close(self):
        if self.sock:
            self.sock.close()
