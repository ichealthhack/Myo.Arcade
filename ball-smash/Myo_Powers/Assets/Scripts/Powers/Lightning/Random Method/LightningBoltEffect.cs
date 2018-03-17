using UnityEngine;
using System.Collections;

public class LightningBoltEffect : MonoBehaviour
{
    /* 
    Klaudijus Miseckas 
    SID:1334116 
    Not Used Class In The Demo
    */

    /*
    [Header ("Bolt Settings")]
    public int numberOfSegments = 8;

    LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer> ();


    }*/

    private LineRenderer lineRenderer;
    private float maxZ = 8f;
    private int numOfSegments = 12;
    private Color colour = Color.white;
    private float PosRange = 0.1f;
    private float radius = 0.4f;
    private Vector2 midPoint;

    void Start()
    { //With quadratic mid point calc
        lineRenderer = GetComponent<LineRenderer> ();
        lineRenderer.SetVertexCount (numOfSegments);


        midPoint = new Vector2 (Random.Range (-radius, radius), Random.Range (-radius, radius));

        for (int i = 1; i < numOfSegments - 1; i++)
        {
            float z = ((float)i) * (maxZ) / (float)(numOfSegments - 1);

            float x = -midPoint.x * z * z / 16f + z * midPoint.x / 2f;

            float y = -midPoint.x * z * z / 16f + z * midPoint.x / 2f;

            lineRenderer.SetPosition (i, new Vector3 (x + Random.Range (-PosRange, PosRange),y + Random.Range (-PosRange, PosRange), z));
        }

        lineRenderer.SetPosition (0, new Vector3 (0f, 0f, 0f));
        lineRenderer.SetPosition (numOfSegments - 1, new Vector3 (Random.Range (-PosRange, PosRange),Random.Range (-PosRange, PosRange), maxZ));
    }




    void Update()
    {
        colour.a -=5f * Time.deltaTime;

        lineRenderer.SetColors (colour, colour);
        if (colour.a <= 0f)
        {
            Destroy (this.gameObject);
        }
    }
}
