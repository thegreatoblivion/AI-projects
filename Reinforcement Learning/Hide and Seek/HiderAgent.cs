using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.InputSystem.Interactions;
using System.Numerics;

public class HiderAgent : Agent
{
    [SerializeField]private float hiderRotationSpeed;
    [SerializeField] private float hiderSpeed;
    public override void Initialize()
    {
        //run once before the whole thing
    }

    public override void OnEpisodeBegin()
    {
        SpawnObjects();
    }

    private void SpawnObjects()
    {
        //initialize position of all objects
        //transform.localPosition = new UnityEngine.Vector3(0f, 0.25f, -4f);
        //transform.localRotation = UnityEngine.Quaternion.identity;

    }

    //public override void CollectObservations(VectorSensor sensor)
    //{
    //obtains all values necessary for the agent to make decisions using sensor.AddObservation()

    //}

    public override void Heuristic(in ActionBuffers actionsOut)
    {

        //allows player to control the agent
        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = 0; //do nothing
        discreteActionsOut[1] = 0;

        if (Input.GetKey(KeyCode.W))
        {
            discreteActionsOut[0] = 1;
            //move forward
        }
        else if (Input.GetKey(KeyCode.S))
        {
            discreteActionsOut[0] = 2;
            //move back
        }
        if (Input.GetKey(KeyCode.A))
        {
            discreteActionsOut[1] = 1;
            //move left
        }
        else if (Input.GetKey(KeyCode.D))
        {
            discreteActionsOut[1] = 2;
            //move right
        }
        
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        MoveAgent(actions.DiscreteActions);
    }

    public void MoveAgent(ActionSegment<int> act)
    {
        //processes the actions received from the neural network
        var moveAction = act[0];
        var rotateAction = act[1];


        switch (moveAction)
        {
            case 1:
                transform.Translate(UnityEngine.Vector3.forward * hiderSpeed * Time.deltaTime);
                break;
            case 2:
                transform.Translate(-UnityEngine.Vector3.forward * hiderSpeed * Time.deltaTime);
                break;

        }
        switch (rotateAction)
         {
            case 1:
                transform.Rotate(0f, -hiderRotationSpeed * Time.deltaTime, 0f);
                break;
            case 2:
                transform.Rotate(0f, hiderRotationSpeed * Time.deltaTime, 0f);
                break;
        }
    }

}

