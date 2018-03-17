using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grip : MonoBehaviour
{
    /* 
    Klaudijus Miseckas 
    SID:1334116 
    */

    [Header ("Grip Settings")]
    public GameObject powerStart;
    public GameObject pointer;
    public float gripRange;
    public float gripRadius;
    public LayerMask layersToCheckForGripPower;

    private bool isGripActive = false;

    private List<GameObject> heldObjects = new List<GameObject> ();

    private GameObject centre;

    void Start()
    {
        MyoPoseCheck.onUseGrip += UseGrip;
        MyoPoseCheck.onStopGrip += StopUsingGrip;

        //Create empty object to use as a centre of attraction for gripped objects
        centre = new GameObject ();
        centre.name = "Grip Centre";

        //Assign a parent for the centre gameobject
        centre.transform.parent = pointer.transform;
    }

    void FixedUpdate()
    {
        if(isGripActive)
        {
            foreach(GameObject obj in heldObjects)
            {
                //Force of pull towards centre
                float force = 4f;

                if(Vector3.Distance(obj.transform.position,centre.transform.position) < 0.2)
                {
                    force = 2f;
                }

                obj.GetComponent<Rigidbody> ().AddForce (GetDirection (centre.transform.position, obj.transform.position) * force,ForceMode.Impulse);
            }
        }
    }

    void UseGrip()
    {
        if(!isGripActive)
        {
            //Debug.Log ("GRIP ACTIVATED -------------------");

            RaycastHit hit;

            if(Physics.SphereCast(powerStart.transform.position,gripRadius,GetDirection(), out hit, gripRange, layersToCheckForGripPower))
            {

                centre.transform.position = hit.point;

                Collider[] cols = Physics.OverlapSphere (hit.point, 0.6f, layersToCheckForGripPower);

                foreach(Collider col in cols)
                {
                    GameObject hitObj = col.gameObject;
                    heldObjects.Add (hitObj);

                    hitObj.GetComponent<Rigidbody> ().useGravity = false;
                }

                if(heldObjects.Count != 0)
                {
                    isGripActive = true;
                }
            }
        }
    }

    void StopUsingGrip()
    {
        //Debug.Log ("GRIP STOPPED ---------------------");

        isGripActive = false;

        foreach (GameObject obj in heldObjects)
        {
            obj.GetComponent<Rigidbody> ().useGravity = true;
        }

        heldObjects.Clear ();
    }

    Vector3 GetDirection()
    {
        Vector3 dir = pointer.transform.position - powerStart.transform.position;
        return dir;
    }

    Vector3 GetDirection(Vector3 s, Vector3 t)
    {
        Vector3 dir = s - t;
        return dir;
    }
}
