# boatSimulator
2D boat simulator made in Unity.
The simulator is a very simplified simulator, using a camera, or state values representing the distance, angle and more.

All of the tasks(scenarios) has an arrow pointing to the next dock, which is a simple method for telling it where to go.
When the boats drive closer to the docks, it gets a reward based on how much closer they get to the dock, if the boats drive away they receive a scaled penalty.
When the boat arrives at a dock, it receives a very high reward, this can be scaled down if required.

As the simulator is made in Unity, it includes graphics which take up a lot of space, and so to store the simulator it uses git lfs.

For controlling it you can either use ml agents, or install the gym-unity package and use a gym interface to control the vehicle.

Example code to be appended!
