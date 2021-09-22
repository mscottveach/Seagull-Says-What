using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid_Behavior_2D : MonoBehaviour
{

    public float speed;
    public float turn_speed;
    public float accumulatedTime;
    public Vector3 turn_amount;
    private bool alignmentOn, cohesionOn, avoidanceOn;

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
        RaycastHit hit;
 
        if (Physics.Raycast(transform.position, transform.forward, out hit, 10.0f))
        {

            if (hit.collider.tag == "Environment") {
                Debug.DrawRay(transform.position, transform.forward*10, Color.yellow,2);
                Vector3 the_ray = hit.point - transform.position; 
                Vector3 the_reflection = Vector3.Reflect(the_ray, hit.normal);
       
                Debug.DrawRay(transform.position, transform.forward*10, Color.yellow,2);
                Debug.DrawRay(transform.position, (the_reflection - hit.point)*25, Color.blue,2);
                float turn_angle = Vector3.Angle(transform.forward,the_reflection - hit.point);
                TurnThisWay(turn_angle);

            }
           
        }
        
        
        
        Turn();
        Move();

    }

    void TurnThisWay(float turn_angle) {

        float avoidance_turn_speed = turn_speed * 10;
        Vector3 turn_vector = new Vector3(0.0f,turn_angle,0.0f);
        transform.Rotate(turn_vector*Time.deltaTime*turn_speed);
//        transform.rotation = Quaternion.Euler(this_way*Time.deltaTime*avoidance_turn_speed);

    }



    void Turn() {
        accumulatedTime += Time.deltaTime;
        if (accumulatedTime > 1.5) {
            turn_amount = new Vector3(0.0f,Random.Range(-30,30),0.0f);
            accumulatedTime = 0;
        }
        transform.Rotate(turn_amount*Time.deltaTime*turn_speed);

    }

    void Move() {
        transform.Translate(transform.forward*Time.deltaTime*speed,Space.World);
    }

    public void UpdateToggles(bool inAvoidance, bool inCohesion, bool inAlignment) {
        avoidanceOn = inAvoidance;
        cohesionOn = inCohesion;
        alignmentOn = inAlignment;
        Debug.Log("Button was pressed!!");
        
    }


}
