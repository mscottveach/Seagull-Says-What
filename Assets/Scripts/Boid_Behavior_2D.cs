using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid_Behavior_2D : MonoBehaviour
{

    public float speed;
    public float turn_speed;
    public float accumulatedTime;
    public Vector3 turn_amount;

    void Start()
    {
        speed = 10.0f;
        turn_speed = 3;
        accumulatedTime = 0;
        turn_amount = new Vector3(0,0,0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    private void FixedUpdate() {
        Turn();
        Move();

    }



    void Turn() {
        accumulatedTime += Time.deltaTime;
        if (accumulatedTime > 3) {
            turn_amount = new Vector3(0.0f,Random.Range(-30,30),0.0f);
            accumulatedTime = 0;
        }
        transform.Rotate(turn_amount*Time.deltaTime*turn_speed);

    }

    void Move() {
        transform.Translate(transform.forward*Time.deltaTime*speed,Space.World);
    }


}
