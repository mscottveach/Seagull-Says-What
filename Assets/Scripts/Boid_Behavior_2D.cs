using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Boid_Behavior_2D : MonoBehaviour
{

    Rigidbody rigidBody;
    Camera viewCamera;

    public float speed;
    public float turn_speed;
    public float accumulatedTime;
    private bool alignmentOn, cohesionOn, avoidanceOn;
    private Collider[] colliderHits;

    bool avoidingWalls;
    bool turning;

    Quaternion reflectRotation;
    Quaternion randomRotation;    
    Quaternion currentRotation;
    Vector3 turn_amount;
    LayerMask just_boids;
    LayerMask just_env;
    Vector3 last_position;

    Collider boidCollider;
    public Collider TheCollider{ get { return boidCollider;}}

    void Start()
    {

        boidCollider = GetComponent<Collider>();
        speed = 10.0f;
        turn_speed = 180;
        accumulatedTime = 0;

        avoidanceOn = false;
        alignmentOn = false;
        cohesionOn = false;
        turning = false;
        turn_amount = new Vector3(0,0,0);
        last_position = transform.position;       
        rigidBody = GetComponent<Rigidbody>();
        viewCamera = Camera.main;
        avoidingWalls = false;
        reflectRotation = new Quaternion(0,0,0,0);
        just_boids = LayerMask.GetMask("a");
        just_env = LayerMask.GetMask("b");

    }


    // Update is called once per frame
    void Update()
    {
        //for future reference:
        //Vector3 mousePos = viewCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, viewCamera.transform.position.y));
            
    }

    float DTR(float inDegrees) {
        float answer = (inDegrees*Mathf.PI)/180.0f;
        Debug.Log(answer);
        return(answer);
    }


    private void FixedUpdate() {

        Orient();

        colliderHits = Physics.OverlapSphere(transform.position,10.0f,just_boids);
        List<Transform> context = new List<Transform>();
        foreach (Collider c in colliderHits) {
            if (c != this.TheCollider) {
                context.Add(c.transform);
            }
        }
       
        if ((alignmentOn) && (!avoidingWalls)) {
            Align();
        }                                                                                                                                                                                                          
        if (avoidanceOn) {
            Avoid();
        }
        if (cohesionOn) {
            Cohere();
        }

        


             
        AvoidWalls();
      //  RandomTurn();
     //   ExecutingTurn();
        Move();

    }

    void Orient() {
        
        Vector3 current_direction = transform.position - last_position;
        last_position = transform.position;
        Quaternion orientRotation = Quaternion.LookRotation(current_direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, orientRotation, turn_speed*Time.deltaTime);
        
    }



    void Align() {

        Vector3 averageDirection = new Vector3(0,0,0);
        int num_of_nearby_boids = 0;
        foreach (Collider a_collider in colliderHits) {
            Rigidbody boid_rb = a_collider.GetComponent<Rigidbody>();
            float dotProd = Vector3.Dot(transform.forward, boid_rb.transform.position - transform.position);
            if (dotProd > 0) {
                averageDirection += boid_rb.transform.forward;
                num_of_nearby_boids += 1;
            }
        }
        averageDirection = averageDirection / num_of_nearby_boids;
        if (averageDirection != new Vector3(0,0,0)) {
            Quaternion alignRotation = Quaternion.LookRotation(averageDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, alignRotation, turn_speed*Time.deltaTime);
        }
    }



    void Avoid() {
        Debug.Log("Avoiding!");
        RaycastHit hit;
        Vector3 rot_five_degrees_CCW = Vector3.zero;
        Vector3 rot_five_degrees_CW = Vector3.zero;
        int count = 0;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 10.0f, just_boids)) {
            //Debug.DrawRay(transform.position, (hit.point - transform.position)*10.0f,Color.green,5);
            rot_five_degrees_CCW = RotateFiveCCW(transform.forward);
            rot_five_degrees_CW = transform.forward;
            while (Physics.Raycast(transform.position, rot_five_degrees_CCW, out hit, 10.0f, just_boids)) {
                count += 1;
                rot_five_degrees_CW = RotateFiveCW(rot_five_degrees_CW);
                if (!(Physics.Raycast(transform.position, rot_five_degrees_CW, out hit, 10.0f, just_boids))){
                    break;
                }
                rot_five_degrees_CCW = RotateFiveCCW(rot_five_degrees_CCW);
                if (count > 10) {
                    break;
                }
            }

            Vector3 new_direction = hit.point - transform.position;
            Debug.Log(hit.point);
            //Vector3 new_direction = Vector3.Reflect(-1*hit.rigidbody.gameObject.transform.forward, transform.forward);
            Quaternion new_rotation = Quaternion.LookRotation(new_direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, new_rotation, turn_speed * Time.deltaTime);
        }
    }

    Vector3 RotateFiveCCW(Vector3 inVector) {
        Vector3 resVector = Vector3.zero;
        resVector.x = inVector.x*Mathf.Cos(Mathf.Deg2Rad) - inVector.z*Mathf.Sin(Mathf.Deg2Rad);
        resVector.z = inVector.x*Mathf.Sin(Mathf.Deg2Rad) + inVector.z*Mathf.Cos(Mathf.Deg2Rad);

        return(resVector);
    }

    Vector3 RotateFiveCW(Vector3 inVector) {
          Vector3 resVector = Vector3.zero;
        resVector.x = inVector.x*Mathf.Cos(-1*Mathf.Deg2Rad) - inVector.z*Mathf.Sin(-1*Mathf.Deg2Rad);
        resVector.z = inVector.x*Mathf.Sin(-1*Mathf.Deg2Rad) + inVector.z*Mathf.Cos(-1*Mathf.Deg2Rad);

        return(resVector);      
    }



    void Cohere() {

        Vector3 averagePosition = new Vector3(0,0,0);
        int num_of_nearby_boids = 0;
        foreach (Collider a_collider in colliderHits) {
            Rigidbody boid_rb = a_collider.GetComponent<Rigidbody>();
            float dotProd = Vector3.Dot(transform.forward, boid_rb.transform.position - transform.position);
            if (dotProd > 0) {
                averagePosition += boid_rb.transform.position;
                num_of_nearby_boids += 1;
            }
        }
        averagePosition = averagePosition / num_of_nearby_boids;
        Vector3 new_direction = averagePosition - transform.position;
        if (averagePosition != new Vector3(0,0,0)) {
            Quaternion cohereRotation = Quaternion.LookRotation(new_direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, cohereRotation, turn_speed*Time.deltaTime);
        }

    }


    void AvoidWalls() {
        RaycastHit hit;
        if (!avoidingWalls) {
            if (Physics.Raycast(transform.position, transform.forward, out hit, 10.0f, just_env)) {

                avoidingWalls = true;
                Vector3 the_ray = hit.point - transform.position; 
                Vector3 the_reflection = Vector3.Reflect(the_ray, hit.normal);
    
                Debug.DrawRay(transform.position, the_ray*10, Color.yellow,2);
                Debug.DrawRay(hit.point, the_reflection*10, Color.blue,2);
                reflectRotation = Quaternion.LookRotation(the_reflection);
              
            } 
        }
        if (avoidingWalls)  {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, reflectRotation, turn_speed * Time.deltaTime);
            if (transform.rotation == reflectRotation) {
                avoidingWalls = false;
            }
            accumulatedTime = 0;
        }
    }



    void RandomTurn() {
        accumulatedTime += Time.deltaTime;
        if (accumulatedTime > 1.5) {
            turn_amount = new Vector3(0.0f, Random.Range(-30,30), 0.0f);
            accumulatedTime = 0;
        }
        if ((!turning) && (!avoidingWalls)) {
            transform.Rotate(turn_amount*Time.deltaTime*3);
        }
    }



    void Move() {
        transform.Translate(transform.forward*Time.deltaTime*speed,Space.World);
    }



    public void UpdateToggles(bool inAvoidance, bool inCohesion, bool inAlignment) {
        avoidanceOn = inAvoidance;
        cohesionOn = inCohesion;
        alignmentOn = inAlignment;        
    }
}









/* holding stuff for legacy purposes */

    // void ExecutingTurn() {
    //     if (turning == true) {
    //          transform.rotation = Quaternion.RotateTowards(transform.rotation, currentRotation, turn_speed * Time.deltaTime);
    //          if (transform.rotation == currentRotation) {
    //              turning = false;
    //          }
    //     }
    // }
