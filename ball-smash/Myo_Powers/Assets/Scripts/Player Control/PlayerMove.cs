using UnityEngine;
using System.Collections;

public class PlayerMove : MonoBehaviour
{
    /* 
    Klaudijus Miseckas 
    SID:1334116 
    */

    [Header ("Movement Settings")]
    public float moveSpeed = 5f;

    private CharacterController cc;

    void Start()
    {
        cc = GetComponent<CharacterController> ();
    }

    void Update()
    {
        float z = Input.GetAxis("Vertical") * moveSpeed;
        float x = Input.GetAxis ("Horizontal") * moveSpeed;

        Move (new Vector3 (x, 0, z));
    }

    void Move(Vector3 dir)
    {
        dir.y = -9.8f;

        dir = transform.TransformDirection (dir);

        cc.Move (dir * Time.deltaTime);
    }
	
}
