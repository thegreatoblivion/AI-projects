using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.InputSystem.Interactions;
using System.Numerics;

public class SeekerAgent : Agent
{
    [SerializeField] private float seekerRotationSpeed;
    [SerializeField] private float seekerSpeed;
    
    //private SeekerAgent seekerAgent;
    [SerializeField] private HiderAgent hiderAgent;

    private float seekerCumulativeReward = 0f;
    private float hiderCumulativeReward = 0f;
    public float timeReward = 0.01f;
    public float caughtReward = 1f;
    public override void Initialize()
    {
        //run once before the whole thing
        seekerCumulativeReward = 0f;
        hiderCumulativeReward = 0f;
    }

    public override void OnEpisodeBegin()
    {
        Debug.Log("New episode, Seeker Reward: " + seekerCumulativeReward.ToString());
        Debug.Log("Hider reward: " + hiderCumulativeReward.ToString());
        seekerCumulativeReward = 0f;
        hiderCumulativeReward = 0f;
        SpawnObjects();
    }

    private void SpawnObjects()
    {
       
        
        // Spawn seeker at fixed position and random rotation
        transform.localPosition = new UnityEngine.Vector3(0f, 0.25f, 0f);
        float seekerAngle = Random.Range(0f, 360f);
        transform.localRotation = UnityEngine.Quaternion.Euler(0f, seekerAngle, 0f);

        // Spawn hider at random position and random rotation within 4x4 area
        float hiderX = Random.Range(-4f, 4f);
        float hiderZ = Random.Range(-4f, 4f);
        float hiderAngle = Random.Range(0f, 360f);
        hiderAgent.transform.localPosition = new UnityEngine.Vector3(hiderX, 0.25f, hiderZ);
        hiderAgent.transform.localRotation = UnityEngine.Quaternion.Euler(0f, hiderAngle, 0f);

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
            Debug.Log("I AM MOVING");
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
                transform.Translate(UnityEngine.Vector3.forward * seekerSpeed * Time.deltaTime);
                break;
            case 2:
                transform.Translate(-UnityEngine.Vector3.forward * seekerSpeed * Time.deltaTime);
                break;

        }
        switch (rotateAction)
        {
            case 1:
                transform.Rotate(0f, -seekerRotationSpeed * Time.deltaTime, 0f);
                break;
            case 2:
                transform.Rotate(0f, seekerRotationSpeed * Time.deltaTime, 0f);
                break;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Hider")
        {
            AddReward(caughtReward);
            hiderAgent.AddReward(-caughtReward);

            seekerCumulativeReward = GetCumulativeReward();
            hiderCumulativeReward = hiderAgent.GetCumulativeReward();


            hiderAgent.EndEpisode();
            EndEpisode();
        }
    }
    private void FixedUpdate()
    {
        AddReward(-timeReward * Time.deltaTime);
        hiderAgent.AddReward(+timeReward * Time.deltaTime);
        //reduces reward each time hider doesnt catch seeker
        seekerCumulativeReward = GetCumulativeReward();
        hiderCumulativeReward = hiderAgent.GetCumulativeReward();
    }
}

