using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manage_Spawn : MonoBehaviour
{
    // Start is called before the first frame update


    public static Manage_Spawn Instance {get; private set;}

    public GameObject a_boid;
    static private int NUM_OF_BOIDS = 200;
    static private int SPAWN_SQUARE_SIZE = 20;
    public GameObject[] theBoids;
    bool somethingToggled, alignmentOn, cohesionOn, avoidanceOn;



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
        Random.InitState(767676132);
        theBoids = new GameObject[NUM_OF_BOIDS];
        somethingToggled = false;

        for (int idx = 0; idx < NUM_OF_BOIDS; idx++) {
            int x = Random.Range(-SPAWN_SQUARE_SIZE,SPAWN_SQUARE_SIZE);
            int y = -8;
            int z = Random.Range(-SPAWN_SQUARE_SIZE,SPAWN_SQUARE_SIZE);
            theBoids[idx] = Instantiate(a_boid, new Vector3(x,y,z), Quaternion.identity);
        
            int random_angle = Random.Range(0,360);
            theBoids[idx].transform.eulerAngles = new Vector3(0.0f,random_angle,0.0f);
        
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
