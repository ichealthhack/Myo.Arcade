using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour {

    //We are not using forces, so we can use "Update" rather than "FixedUpdate"
    void Update ()
    {
        transform.Rotate(new Vector3(15, 30, 45) * Time.deltaTime);
	}
}
