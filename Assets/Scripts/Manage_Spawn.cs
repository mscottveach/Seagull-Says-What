using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manage_Spawn : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject a_boid;
    static private int NUM_OF_BOIDS = 20;
    static private int SPAWN_SQUARE_SIZE = 20;


    public GameObject[] theBoids;  
    void Start()
    {
        Random.InitState(767676132);
        theBoids = new GameObject[NUM_OF_BOIDS];

        for (int idx = 0; idx < NUM_OF_BOIDS; idx++) {
            int x = Random.Range(-SPAWN_SQUARE_SIZE,SPAWN_SQUARE_SIZE);
            int y = 0;
            int z = Random.Range(-SPAWN_SQUARE_SIZE,SPAWN_SQUARE_SIZE);
            theBoids[idx] = Instantiate(a_boid, new Vector3(x,y,z), Quaternion.identity);
        
            int random_angle = Random.Range(0,360);
            theBoids[idx].transform.eulerAngles = new Vector3(0.0f,random_angle,0.0f);
        }


//        holdObj.GetComponent<Boid>().leader = true;

  //      for (idx = 0; idx < NUM_O)
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
