Ivan Ho

All related project files are located in "Assets/ML-Agents/Examples/3DBball". Source code can be found in the "Scripts"
folder.

ML Agents is utilized in the "3DBall" scene. You can test the code by simply playing the scene and modifying the
agents' models in the "Agent"-labeled game objects. Results about the training can be found in a separate "results"
folder in the root directory.

Tabular Q-Learning is utilized in the "3DBall QLearning" scene. You can test the code by simply playing the scene
and modifying the hyperparameters. Data is recorded in the "Assets/StreamingAssets/Data" folder, where cumulative
rewards are recorded at the end of every episode. The logic can be found in "QLearningAgent.cs" and
"QLearningManager.cs".

Overall ML Agents performs much better and more efficiently than Q-Learning, as 3DBall contains continuous actions
with floating point values. Q-Learning is much more suited for discrete actions with integer values, and as such
is not as efficient as training with ML Agents. For the sake of simplicity, the states and actions are represented
in "simplified values" (i.e. float --> int), but as such the accuracy of the result has been greatly compromised.
Additionally, the Q-Learning algorithm is not as efficient as ML Agents, as it is not able to take advantage
of the GPU to train the model. As such, the training time is much longer than ML Agents and is currently incomplete
and the current data of Q-Learning does not display any convergence.

Notes
 - Training on both ML Agents and Q-Learning is extremely slow and thus the recorded data and models are 
   incomplete due to time constraints.
 - "results/3DBall_01_3DBall.csv" contains graphs for ML Agents results.
 - "StreamingAssets/Data/3DBall-QLearning-2-3.csv" contains a graph for Q-Learning results.
