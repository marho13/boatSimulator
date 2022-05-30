# boatSimulator
2D boat simulator made in Unity.
The simulator is a very simplified simulator, using a camera, or state values representing the distance, angle and more.

All of the tasks(scenarios) has an arrow pointing to the next dock, which is a simple method for telling it where to go.
When the boats drive closer to the docks, it gets a reward based on how much closer they get to the dock, if the boats drive away they receive a scaled penalty.
When the boat arrives at a dock, it receives a very high reward, this can be scaled down if required.

As the simulator is made in Unity, it includes graphics which take up a lot of space, and so to store the simulator it uses git lfs.

For controlling it you can either use ml agents, or install the gym-unity package and use a gym interface to control the vehicle.

Gym-Unity code: https://github.com/marho13/gym-unity-interface

ML Agents requires Unity, and to install the ml agents package, after which you add a file called boatAgent.yaml in the config folder of mlagents
Example file: 
behaviors:
  boatAgent:
    trainer_type: ppo
    hyperparameters:
      batch_size: 64
      buffer_size: 100
      learning_rate: 3.0e-3
      beta: 5.0e-4
      epsilon: 0.2
      lambd: 0.99
      num_epoch: 4
      learning_rate_schedule: linear
      beta_schedule: constant
      epsilon_schedule: linear
    network_settings:
      normalize: false
      hidden_units: 128
      num_layers: 2
    reward_signals:
      extrinsic:
        gamma: 0.99
        strength: 1.0
    max_steps: 500000
    time_horizon: 64
    summary_freq: 10000
    threaded: True
