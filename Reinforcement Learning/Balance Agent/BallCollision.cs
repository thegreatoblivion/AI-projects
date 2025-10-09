using UnityEngine;

public class BallCollision : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]private BalanceAgent balanceAgent;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerExit(Collider other)
    {
        //if sphere leaves then penalty is incurred
        if (other.gameObject.name == "Ball")
        {
            balanceAgent.AddReward(-1f);
            balanceAgent.CumulativeReward = balanceAgent.GetCumulativeReward();
            balanceAgent.EndEpisode();
        }
    }
}
