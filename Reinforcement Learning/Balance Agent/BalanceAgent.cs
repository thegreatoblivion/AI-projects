using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System.Collections;
using UnityEngine.Rendering;
using Unity.VisualScripting;
using UnityEditor.UI;

public class BalanceAgent : Agent
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [HideInInspector] public int CurrentEpisode = 0;
    [HideInInspector] public float CumulativeReward = 0f;

    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private Transform ball;
    [SerializeField] private Rigidbody rb;

    [SerializeField] private Vector2 distanceRange = new Vector2(0f, 2f);
    [SerializeField] private Vector2 speedRange = new Vector2(1f, 2f);


    public override void Initialize()
    {
        Debug.Log("Initialize()");
        CurrentEpisode = 0;
        CumulativeReward = 0f;
    }

    // Update is called once per frame
    public override void OnEpisodeBegin()
    {
        Debug.Log("OnEpisodeBegin: Episode " + CurrentEpisode + ", last reward " + CumulativeReward.ToString());
        CurrentEpisode++;
        CumulativeReward = 0f;
        SpawnObjects();
    }
    private void SpawnObjects()
    {
        //initialize position of ball and platfrom relative to env
        transform.localPosition = new Vector3(0f, 0f, 0f);
        transform.localRotation = Quaternion.identity;
        float randomAngle = Random.Range(0f, 360f);
        float randomDistance = Random.Range(distanceRange[0], distanceRange[1]);
        float randomSpeed = Random.Range(speedRange[0], speedRange[1]);

        ball.localPosition = Quaternion.Euler(0f, randomAngle, 0f) * Vector3.forward * randomDistance + new Vector3(0, 1, 0);
        randomAngle = Random.Range(0f, 360f);
        rb.linearVelocity = Quaternion.Euler(0f, randomAngle, 0f) * Vector3.forward * randomSpeed;

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Debug.Log(sensor.ObservationSize());
        //obtains all values necessary. Will also do vector stacking of size 2 maybe?
        float ballPosX_normalized = ball.localPosition.x / 5f;
        float ballPosY_normalized = ball.localPosition.y / 5f;
        float ballPosZ_normalized = ball.localPosition.z / 5f;
        float balanceRotX_normalized = (transform.localRotation.eulerAngles.x / 360f) * 2f - 1f;
        float balanceRotY_normalized = (transform.localRotation.eulerAngles.y / 360f) * 2f - 1f;
        float balanceRotZ_normalized = (transform.localRotation.eulerAngles.z / 360f) * 2f - 1f;
        float ballVelocityX_normalized = rb.linearVelocity.x / 4f;
        float ballVelocityY_normalized = rb.linearVelocity.y / 4f;
        float ballVelocityZ_normalized = rb.linearVelocity.z / 4f;
        sensor.AddObservation(ballPosX_normalized);
        sensor.AddObservation(ballPosY_normalized);
        sensor.AddObservation(ballPosZ_normalized);
        sensor.AddObservation(ballVelocityX_normalized);
        sensor.AddObservation(ballVelocityY_normalized);
        sensor.AddObservation(ballVelocityZ_normalized);
        sensor.AddObservation(balanceRotX_normalized);
        sensor.AddObservation(balanceRotY_normalized);
        sensor.AddObservation(balanceRotZ_normalized);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = 0;

        if (Input.GetKey(KeyCode.W))
        {
            discreteActionsOut[0] = 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            discreteActionsOut[0] = 2;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            discreteActionsOut[0] = 3;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            discreteActionsOut[0] = 4;
        }

    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        MoveAgent(actions.DiscreteActions);
    }
    public void MoveAgent(ActionSegment<int> act)
    {
        var action = act[0];

        //1,2 is rotate in x. 3,4 rotate in z
        switch (action)
        {
            case 1:

                transform.Rotate(rotationSpeed * Time.deltaTime, 0f, 0f);
                break;
            case 2:

                transform.Rotate(-rotationSpeed * Time.deltaTime, 0f, 0f);
                break;
            case 3:

                transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
                break;
            case 4:

                transform.Rotate(0f, 0f, -rotationSpeed * Time.deltaTime);
                break;
        }

    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.name == "Ball")
        {
            AddReward(0.01f * Time.deltaTime);
            CumulativeReward = GetCumulativeReward();
            //if it is in contact with the ball the reward increases 0.01 per sec
        }
    }
    //private void FixedUpdate()
    //{
        //float distance = Vector3.Distance(ball.transform.localPosition, transform.localPosition);
        //AddReward(-distance * 0.001f);
        //KEEP THE BALL CENTER
        //float ballSpeed = rb.linearVelocity.magnitude;
        //if (ballSpeed > 1)
        //{
            //AddReward(-0.01f * Time.deltaTime);
        //}
    //}
}
