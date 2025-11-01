using System.Buffers.Text;
using Unity.VisualScripting;
using UnityEngine;

public class Green : Creature
{

    private void Start()
    {
        Initialize();
        layerMask = LayerMask.GetMask("Red", "Blue");
    }

    void Update()
    {
        UpdateCycle();
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        WallDetection(collision);
    }
    void OnDestroy()
    {
        graph.greenLifeTime.Add(lifeTime);
    }
}
