using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid_Behavior_2D : MonoBehaviour
{

    public float speed;

    void Start()
    {
        speed = 10.0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    private void FixedUpdate() {
        Move();

    }


    void Move() {
        transform.Translate(transform.forward*Time.deltaTime*speed);
    }


}
