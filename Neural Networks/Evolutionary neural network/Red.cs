using System.Buffers.Text;
using Unity.VisualScripting;
using UnityEngine;

public class Red : Creature
{
    private float eatCooldown;
    public float eatStunLength = 1f;

    private void Start()
    {
        Initialize();
        eatCooldown = eatStunLength;
        //once you are born you cant immediately eat as well;
        layerMask = LayerMask.GetMask("Blue", "Green");
    }

    void Update()
    {
        eatCooldown -= Time.deltaTime;
        UpdateCycle();
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        WallDetection(collision);
        if (collision.gameObject.CompareTag("Green") && eatCooldown <= 0)
        {
            //Debug.Log("I eat you");
            energy += collision.gameObject.GetComponent<Green>().energy;
            //the energy is transferred to the predator
            reproduceScore += 100;
            Destroy(collision.gameObject);
            eatCooldown = eatStunLength;
            //predator is stunned for stunlength so they cant eat immediately
        }

    }
    void OnDestroy()
    {
        graph.redLifeTime.Add(lifeTime);
    }
}
