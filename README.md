# boatSimulator
2D boat simulator made in Unity.
Low graphical quality, used for publications with FFI.
The simulator is a very simplified version of their simulator, including the same control scheme, but using a camera instead of LIDAR.

The simulator has multiple tasks related to it, each with their own usecase.
  First is the task of docking when spawning close to the docks, this is spawned a certain distance away(10-15) and when it reaches the docks will be spawned near the next dock.
  Secondly we have the task in which the boat is spawned on the map, and if it travels away from the dock it receives a penalty and that action is not performed.
  Lastly we have a task similar to the second one, but it will perform the action, and not receive a penalty
All of the tasks has an arrow pointing to the next dock, which is a simple method for telling it where to go.
When the boats drive closer to the docks, it gets a reward based on how much closer they get to the dock, if the boats drive away they receive a scaled penalty.
When the boat arrives at a dock, it receives a very high reward, this can be scaled down if required.

As the simulator is made in Unity, it includes graphics which take up a lot of space, and so to store the simulator it uses git lfs.

For the controls related to the simulation as well as the one related to the FFI simulator, you can go to: https://github.com/marho13/SteeringDockingPaper
Both this repository and that one are private repositories if you want to give someone else access tell me and i can add them.
Though this way no one that is not supposed to know about these systems will be able to access them.

To change the port, go into the assets folder, then the socketio folder, then the websocketsharp, in there you will find a script called WebSocket.cs... This is where you change the port, in case you want to use train multiple agents
