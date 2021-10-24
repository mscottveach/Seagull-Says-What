using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manage_Spawn : MonoBehaviour
{
    // Start is called before the first frame update


    public static Manage_Spawn Instance {get; private set;}

    public GameObject a_boid;
    static private int NUM_OF_BOIDS = 5;
    static private int SPAWN_SQUARE_SIZE = 20;
    public GameObject[] theBoids;
    bool somethingToggled, alignmentOn, cohesionOn, avoidanceOn;

    [Range(0.0f, 10.0f)]
    public float alignWeight;
    [Range(0.0f, 10.0f)]
    public float cohereWeight;
    [Range(0.0f, 10.0f)]
    public float avoidWeight;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);  
        }
    }

    void Start()
    {
        Random.InitState(19720701);
        theBoids = new GameObject[NUM_OF_BOIDS];
        somethingToggled = false;
        Boid_Behavior_2D aScript;

        for (int idx = 0; idx < NUM_OF_BOIDS; idx++) {
            int x = Random.Range(-SPAWN_SQUARE_SIZE,SPAWN_SQUARE_SIZE);
            int y = 0;
            int z = Random.Range(-SPAWN_SQUARE_SIZE,SPAWN_SQUARE_SIZE);
            theBoids[idx] = Instantiate(a_boid, new Vector3(x,y,z), Quaternion.identity);
            //theBoids[idx] = Instantiate(a_boid, new Vector3(0.0f,y,0.0f), Quaternion.identity);

            int random_angle = Random.Range(0,360);
            theBoids[idx].transform.eulerAngles = new Vector3(0.0f,random_angle,0.0f);
            aScript = theBoids[idx].GetComponent<Boid_Behavior_2D>();
            aScript.UpdateWeights(avoidWeight, cohereWeight, alignWeight);
        
        }


        Debug.Log(Physics.queriesHitTriggers);
    }

    public void ToggleAlignment() {
        if (alignmentOn) {
            alignmentOn = false;
        }
        else {
            alignmentOn = true;
        }
        somethingToggled = true;
    }
    


    public void ToggleCohesion() {
        if (cohesionOn) {
            cohesionOn = false;
        }
        else {
            cohesionOn = true;
        }
        somethingToggled = true;
    }


    public void ToggleAvoidance() {
        if (avoidanceOn) {
            avoidanceOn = false;
        }
        else {
            avoidanceOn = true;
        }
        somethingToggled = true;
    }
    



    void Update()
    {
        
        
        Boid_Behavior_2D aScript;
        if (somethingToggled) {

            foreach (GameObject aBoid in theBoids) {
                aScript = aBoid.GetComponent<Boid_Behavior_2D>();
                aScript.UpdateToggles(avoidanceOn, cohesionOn, alignmentOn);
            }
            somethingToggled = false;
        }


    }
}
