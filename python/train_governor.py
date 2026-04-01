from stable_baselines3 import PPO
from stable_baselines3.common.env_util import make_vec_env
from outbreak_env import OutbreakEnv

env = make_vec_env(lambda: OutbreakEnv(), n_envs=1)

model = PPO("MlpPolicy", env, verbose=1, tensorboard_log="./ppo_outbreak_tensorboard/")

try:
    print("Starting Training! Connect Unity to port 9876...")
    model.learn(total_timesteps=100000)
    model.save("ppo_outbreak_governor_v1")
    print("Model saved successfully as ppo_outbreak_governor_v1.")
except KeyboardInterrupt:
    print("Training Interrupted! Saving model...")
    model.save("ppo_outbreak_governor_v1_interrupted")
