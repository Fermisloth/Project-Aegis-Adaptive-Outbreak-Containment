from stable_baselines3 import PPO
from outbreak_env import OutbreakEnv

print("Loading Trained Governor...")
model = PPO.load("ppo_outbreak_governor_v1")

env = OutbreakEnv()

obs = env.reset()
print("Starting live governor inference...")
while True:
    action, _states = model.predict(obs, deterministic=True)
    obs, reward, done, info = env.step(action)
    if done:
        obs = env.reset()
        print("Episode finished.")
