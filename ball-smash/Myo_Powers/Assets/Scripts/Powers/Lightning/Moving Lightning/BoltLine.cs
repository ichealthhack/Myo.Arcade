using UnityEngine;
using System.Collections;

public class BoltLine
{
    /* 
    Klaudijus Miseckas 
    SID:1334116 
    */

    private Vector3 startNode;
    private Vector3 endNode;

    /// <summary>
    /// Bolt line, individual bolt line which make up the whole lightning bolt, this line contains start and end Vector3 node
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    public BoltLine(Vector3 start, Vector3 end)
    {
        startNode = start;
        endNode = end;
    }

    /// <summary>
    /// Get starting line node
    /// </summary>
    /// <returns></returns>
    public Vector3 GetStartNode()
    {
        return startNode;
    }

    /// <summary>
    /// Get ending line node
    /// </summary>
    /// <returns></returns>
    public Vector3 GetEndNode()
    {
        return endNode;
    }

    /// <summary>
    /// Get distance between start and end node of the line
    /// </summary>
    /// <returns></returns>
    public float GetDistanceBetweenNodes()
    {
        return Vector3.Distance (startNode, endNode);
    }
}
