using UnityEngine;

public class BalanceGUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private BalanceAgent balanceAgent;
    private float CumulativeReward;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnGUI()
    {
        CumulativeReward = balanceAgent.GetCumulativeReward();
        GUI.Label(new Rect(100, 100, 200, 20), "Reward: " + CumulativeReward.ToString());
    }
}
