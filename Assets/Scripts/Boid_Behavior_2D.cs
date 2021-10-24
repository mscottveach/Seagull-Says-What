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
    
    public float collisionRadiusMultiplier;
    public float nearBoidRadius;
    public float squareNearBoidRadius;
    public float squareCollisionRadius;
    public float cohereWeight;
    public float alignWeight;
    public float avoidWeight;
    public float maxSpeed;
    public float sqrMaxSpeed;

    public Vector3 velocity;

    Vector3 acceleration;
    Vector3 cohereMove;
    Vector3 alignMove;
    Vector3 avoidMove;
    public float[] behaviorWeights;
    List<Transform> context;

    private bool alignmentOn, avoidAndCohereOn;
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
        speed = 5.0f;
        maxSpeed = 10.0f;
        turn_speed = 180;
        accumulatedTime = 0;
        collisionRadiusMultiplier = 0.5f;
        nearBoidRadius = 10.0f;
        squareNearBoidRadius = nearBoidRadius * nearBoidRadius;
        squareCollisionRadius = squareNearBoidRadius * collisionRadiusMultiplier * collisionRadiusMultiplier;
        sqrMaxSpeed = maxSpeed * maxSpeed;

        behaviorWeights = new float[3];

        velocity = new Vector3(4,0,4);
        //velocity = new Vector3(Random.Range(-5,5),0.0f,Random.Range(-5,5));
        //velocity.Normalize();
        // Debug.Log("Velocity");
        // Debug.Log(velocity);
        acceleration = new Vector3(0,0,0);

        avoidAndCohereOn = true;
        alignmentOn = false;
        turning = false;
        turn_amount = new Vector3(0,0,0);
        last_position = transform.position;       
        rigidBody = GetComponent<Rigidbody>();
        viewCamera = Camera.main;
        avoidingWalls = false;
        reflectRotation = new Quaternion(0,0,0,0);
        just_boids = LayerMask.GetMask("a");
        just_env = LayerMask.GetMask("b");
        alignMove = new Vector3(0,0,0);
        cohereMove = new Vector3(0,0,0);
        avoidMove = new Vector3(0,0,0);

        avoidAndCohereOn = true;
        

    }


    // Update is called once per frame
    void Update()
    {
        //for future reference:
        //Vector3 mousePos = viewCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, viewCamera.transform.position.y));
            
    }


    private void FixedUpdate() {

        //This figures out the direction the boid is moving in and points it in that direction.


        //This checks to see which boids are nearby and builds a list of their transforms in the list "context"
        colliderHits = Physics.OverlapSphere(transform.position,nearBoidRadius,just_boids);
       
        if (alignmentOn) {
            Align();
        }                                                                                                                                                                                                          
        if (avoidAndCohereOn) {
        //    AvoidAndCohere(context);
        }

        Move();
        //Orient();
    }

    void Orient() {
        
        Vector3 current_direction = transform.position - last_position;
        last_position = transform.position;

        if (current_direction != Vector3.zero) {
            Quaternion orientRotation = Quaternion.LookRotation(current_direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, orientRotation, turn_speed*Time.deltaTime);
        }
        
    }

    void AvoidAndCohere(List<Transform> context) {
        avoidMove = new Vector3(0,0,0);
        int avoidCount = 0;

        cohereMove = new Vector3(0,0,0);
        int cohereCount = 0;
    
        foreach (Transform t in context) {
            Vector3 vectorToNeighbor = t.position - this.transform.position;
            
            if (vectorToNeighbor.sqrMagnitude < squareCollisionRadius) {
                //AVOID
                avoidMove += this.transform.position - t.position;
                avoidCount += 1;
            }
            else {
                //COHERE
                cohereMove += t.position;
                cohereCount += 1;
            }
        }

        if (avoidCount > 0) {
            avoidMove /= avoidCount;
        }

        if (cohereCount > 0) {
            cohereMove /= cohereCount;
            cohereMove = cohereMove - this.transform.position;
        }
    }

    
    void Align() {

        alignMove = new Vector3(0,0,0);
        int alignCount = 0;

        foreach (Collider c in colliderHits) {
            alignMove += c.gameObject.GetComponent<Boid_Behavior_2D>().velocity;
            alignCount += 1;
        }

        if (alignCount > 0) {
            alignMove /= alignCount;
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
        Seek(new Vector3(-10.0f,0.0f,-10.0f));
        velocity += acceleration;
        if (velocity.sqrMagnitude > sqrMaxSpeed) {
            velocity.Normalize();
            velocity *= maxSpeed;
        }
        transform.position += velocity*Time.deltaTime;

        //transform.position += steeringForce*Time.deltaTime*speed;
        //velocity += acceleration;
        acceleration = Vector3.zero;


        // Vector3 theMove = avoidMove*avoidWeight + cohereMove*cohereWeight + alignMove*alignWeight;

        // if (theMove != Vector3.zero) {
        //     transform.Translate(theMove*Time.deltaTime*speed,Space.World);
        // }
    }

    public void Seek(Vector3 target) {
        Vector3 desired = target - this.transform.position;
        desired.Normalize();
        desired *= maxSpeed;
        Vector3 steeringForce = desired - this.velocity;

        ApplyForce(steeringForce);
    }

    public void ApplyForce(Vector3 inForce) {
        acceleration += inForce;
    }

    public void UpdateToggles(bool inAvoidance, bool inCohesion, bool inAlignment) {
        avoidAndCohereOn = inAvoidance;
        alignmentOn = inAlignment;        
    }

    public void UpdateWeights(float inAvoidWeight, float inCohereWeight, float inAlignWeight) {
        avoidWeight = inAvoidWeight;
        cohereWeight = inCohereWeight;
        alignWeight = inAlignWeight;

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
