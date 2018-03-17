using UnityEngine;
using System.Collections;

public class BoltLineUpdate : MonoBehaviour
{
    /* 
    Klaudijus Miseckas 
    SID:1334116 
    */

    private Vector3 startNodePos;
    private Vector3 endNodePos;
    private Vector3 defaultScale;

    private float length;

    void Start()
    {
        defaultScale = transform.localScale;

        transform.LookAt (endNodePos);

        length = Vector3.Distance (startNodePos, endNodePos);

        defaultScale.z = length / 2;
        transform.localScale = defaultScale;
    }

    public void SetBoltLine(Vector3 startNode, Vector3 endNode)
    {
        startNodePos = startNode;
        endNodePos = endNode;
    }

}
