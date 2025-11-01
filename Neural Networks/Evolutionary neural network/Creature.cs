using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using System;
using Unity.VisualScripting.FullSerializer;

public class Creature : MonoBehaviour
{
    [HideInInspector] public int layerMask;
    
    [HideInInspector] public Matrix<float>[] weights;
    [HideInInspector] public Vector<float>[] biases;
    [HideInInspector] public int[] dimensions;
    [HideInInspector] public float reproduceScore;
    [HideInInspector] public int numLayers;
    [HideInInspector] public float energy;

    public bool isOriginal = true;

    public bool showRayCast = false;
    
    public Graph graph;
    public Initialization init;
    public Rigidbody2D rb;
    public int rays;
    public float rayLength;
    public float maxEnergy;
    public float linearSpeed;
    public float rotationSpeed;
    public float birthRate;
    //birth rate 20 means it will reproduce once every 100/20 = 5 seconds
    public float deathRate;
    public float mutationProbability;
    public float mutationStrength;

    public int numHiddenNeurons;

    public Transform folder;
    public float lifeTime;
    //call initialize in start()
    public void Initialize()
    {
        //places guy on a random part on the field
        
  
        rb = GetComponent<Rigidbody2D>();
        dimensions = new int[] {rays, numHiddenNeurons, 2};
        transform.rotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(-180f, 180f));

        //layer mask are the layers you want the ray cast to detect
        lifeTime = 0f;
        energy = maxEnergy;
        reproduceScore = 0;
        showRayCast = false;
        float width = init.width;

        numLayers = dimensions.Length;

        if (isOriginal)
        {
            (weights, biases) = InitializeNN(dimensions);


            float randomX = UnityEngine.Random.Range(-width / 2, width / 2);
            float randomY = UnityEngine.Random.Range(-width / 2, width / 2);
            transform.localPosition = new Vector2(randomX, randomY);

            //only if it is original then it displaces it randomly
        }
        else
        {
            //if (UnityEngine.Random.Range(0, 1) < mutationProbability)
            //{
            (weights, biases) = Mutate(weights, biases);
            //}

            //transform.Translate(Vector2.up * 2);

            //2 units in front
        }



        //for (int i = 0; i < dimensions.Length-1; i++)   DEBUG
        //{
        //Debug.Log(weights[i].ToString());
        //Debug.Log(biases[i].ToString());
        //}
    }

    // put UpdateCycle in update 
    public void UpdateCycle()
    {
        lifeTime += Time.deltaTime;
        reproduceScore += Time.deltaTime * birthRate;
        if (reproduceScore >= 100)
        {
            reproduceScore -= 100;
            Reproduce();
        }

        float[] observation = CollectObservations(rays, rayLength, layerMask);

        Vector<float> input = Vector<float>.Build.DenseOfArray(observation);

        Vector<float> output = ForwardPropagation(weights, biases, input);

        TakeAction(output);

        
    }
    //put in OnCollisionEnter2D()
    public void WallDetection(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Horizontal Wall"))
        {
            transform.position = new Vector3(-transform.position.x, transform.position.y, transform.position.z);


        }
        if (collision.gameObject.CompareTag("Vertical Wall"))
        {
            transform.position = new Vector3(transform.position.x, -transform.position.y, transform.position.z);
        }
    }
    public float[] CollectObservations(int rays, float rayLength, int layerMask)
    {
        int observationSize = dimensions[0];
        float[] observation = new float[observationSize];
        //this matches the vector dimensions of the input expected 
        for (int i = 0; i < rays; i++)
        {
            float angle = i * (360f / rays);
            //Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            Quaternion offsetDir = Quaternion.Euler(0, 0, angle);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, offsetDir*transform.up, rayLength, layerMask);
            float normDist = hit.distance / rayLength;
            if (hit.collider != null)
            {
                observation[i] = 1 - normDist;

            }
            else
            {
                observation[i] = -1; 
                //if observation is 1, it means it is very close, if 0, then it is just detected, if -1, no detection.

            }
            if (showRayCast)
            {
                Debug.DrawRay(transform.position, offsetDir*transform.up * rayLength, Color.green);

                // If hit, draw a red line to the hit point
                if (hit.collider != null)
                {
                    Debug.DrawLine(transform.position, hit.point, Color.red);
                }
            }
            

        }
        //observation[rays] = energy;
        //the ninth element (of index 8), representing energy      DELISTED

        return observation;
    }
    //Initialize NN function
    public (Matrix<float>[] weights, Vector<float>[] biases) InitializeNN(int[] dimensions)
    {
        Matrix<float>[] weights = new Matrix<float>[numLayers - 1];

        //creates an array of matrices for each set of weight between layers

        Vector<float>[] biases = new Vector<float>[numLayers - 1];

        //creates array of bias vectors for each set of layers excluding the input layer

        //e.g. if only 3 layers then weights.length =2
        //the for loop will only run from i=0 and i=1
        for (int i = 0; i < weights.Length; i++)

        {
            weights[i] = Matrix<float>.Build.Random(dimensions[i + 1], dimensions[i]);
            biases[i] = Vector<float>.Build.Dense(dimensions[i + 1]);
        }
        // in this case weights[0] represents the weights from input to 1st layer, biases[0] represent the biases on the 1st layer
        //biases will be all 0 vectors at the start while weights are initially random
        return (weights, biases);
    }
    public Vector<float> ForwardPropagation(Matrix<float>[] weights, Vector<float>[] biases, Vector<float> input)
    {
        for (int i = 0; i < numLayers - 1; i++)
        {
            input = (weights[i] * input) + biases[i];

            //input = input.Map(x => 1f / (1f + Mathf.Exp(-x)));
            //applies the sigmoid function (values will be between 0 and 1)         
            input = input.Map(x => (float)Math.Tanh(x));   
            //aplies the tanh function (between -1 and 1)
        }
        return input;
    }
    public void TakeAction(Vector<float> output)
    {
        //our outputs form a vector that the character takes. The first input is magnitude of that vector, second is the angle (from -90 to 90 degrees)
        if (energy > 0)
        {
            transform.Rotate(0, 0, output[1] * rotationSpeed * Time.deltaTime);
            transform.Translate(Vector2.up * linearSpeed * Time.deltaTime * output[0]);
            energy = energy - (linearSpeed * Time.deltaTime * Math.Abs(output[0]) + deathRate * Time.deltaTime);
            //Debug.Log(energy);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Reproduce()
    {
        //if (gameObject.CompareTag("Red"))
        //{
            GameObject child = Instantiate(gameObject, folder);
            Creature childScript = child.GetComponent<Creature>();
            childScript.isOriginal = false;
            childScript.weights = weights;
            childScript.biases = biases;
        //}
        //else if (gameObject.CompareTag("Green"))
        //{
            //GameObject child = Instantiate(gameObject, folder);
            //Green childScript = child.GetComponent<Green>();
            //childScript.isOriginal = false;
            //childScript.weights = weights;
            //childScript.biases = biases;
       // }
        
    }
    public (Matrix<float>[] weights, Vector<float>[] biases) Mutate(Matrix<float>[] weights, Vector<float>[] biases)
    {
        ; // Adjust this value for more/less mutation

        Matrix<float>[] newWeights = new Matrix<float>[weights.Length];
        Vector<float>[] newBiases = new Vector<float>[biases.Length];

        for (int i = 0; i < weights.Length; i++)
        {
            //only a 0.1 chance of a weight/bias changing
            // Clone the matrix to avoid modifying the original
            newWeights[i] = weights[i].Clone();
            // Mutate each element
            newWeights[i].MapInplace(x => x + GaussianSample() * mutationStrength);

        }

        for (int i = 0; i < biases.Length; i++)
        {

            newBiases[i] = biases[i].Clone();
            newBiases[i].MapInplace(x => x + GaussianSample() * mutationStrength);

        }

        return (newWeights, newBiases);
    }

    public float GaussianSample()
    {
        float u1 = 1.0f - UnityEngine.Random.value; // (0,1]
        float u2 = 1.0f - UnityEngine.Random.value;
        return Mathf.Sqrt(-2.0f * Mathf.Log(u1)) *
            Mathf.Sin(2.0f * Mathf.PI * u2);
    }





}
