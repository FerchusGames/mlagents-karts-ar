using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class KartAgent : Agent
{
    public CheckpointManager _checkpointManager;
    private KartController _kartController;

    //called once at the start
    public override void Initialize()
    {
        _kartController = GetComponent<KartController>();
    }

    //Called each time it has timed-out or has reached the goal
    public override void OnEpisodeBegin()
    {
        _checkpointManager.ResetCheckpoints();
        _kartController.Respawn();
    }

    #region Edit this region!

    //Collecting extra Information that isn't picked up by the RaycastSensors
    public override void CollectObservations(VectorSensor sensor)
    {
        // Vector between the agent and the next checkpoint
        Vector3 diff =
            _checkpointManager.nextCheckPointToReach.transform.position - transform.position;
        sensor.AddObservation(diff / 20f); // Normalized vector. Checkpoints are less than 20 units away
        AddReward(-0.001f); // Punish the agent for taking too long to reach the next checkpoint
    }

    //Processing the actions received
    public override void OnActionReceived(ActionBuffers actions)
    {
        var input = actions.ContinuousActions;

        _kartController.Steer(input[0]);
        _kartController.ApplyAcceleration(input[1]);
    }

    //For manual testing with human input, the actionsOut defined here will be sent to OnActionRecieved
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var action = actionsOut.ContinuousActions;

        action[0] = Input.GetAxis("Horizontal"); // Steering
        action[1] = Input.GetKey(KeyCode.W) ? 1f : 0f; // Acceleration
    }

    #endregion
}
