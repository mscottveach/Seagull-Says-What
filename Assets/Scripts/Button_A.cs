using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_A : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AlignmentToggle() {
        Debug.Log("Alignment was pressed.");
        Manage_Spawn.Instance.ToggleAlignment();
    }

    public void CohesionToggle() {
        Debug.Log("Cohesion Was pressed");
        Manage_Spawn.Instance.ToggleCohesion();
    }

    public void AvoidanceToggle() {
        Debug.Log("Avoidance was pressed.");
        Manage_Spawn.Instance.ToggleAvoidance();
    }
}
