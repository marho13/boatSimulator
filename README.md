# boatSimulator
2D boat simulator made in Unity.
Low graphical quality, used for publications with FFI.
The simulator is a very simplified version of their simulator, including the same control scheme, but using a camera instead of LIDAR.

All of the tasks(scenarios) has an arrow pointing to the next dock, which is a simple method for telling it where to go.
When the boats drive closer to the docks, it gets a reward based on how much closer they get to the dock, if the boats drive away they receive a scaled penalty.
When the boat arrives at a dock, it receives a very high reward, this can be scaled down if required.

As the simulator is made in Unity, it includes graphics which take up a lot of space, and so to store the simulator it uses git lfs.

For the controls related to the simulation, you can go to: https://github.com/marho13/SteeringDockingPaper

The socketio module is taken from: https://github.com/udacity/self-driving-car-sim
To change the port, go into the assets folder, then the socketio folder, then the websocketsharp, in there you will find a script called WebSocket.cs... 
This is where you change the port, in case you want to use train multiple agents
