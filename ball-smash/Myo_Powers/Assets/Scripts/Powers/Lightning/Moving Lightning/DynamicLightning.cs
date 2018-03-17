using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DynamicLightning : MonoBehaviour
{

    /* 
    Klaudijus Miseckas 
    SID:1334116 
    */

    [Header ("Lightning Mesh")]
    public GameObject lightningBoltMesh;
    public Transform parent;

    [Header ("Lightning Target")]
    public Transform target;

    [Header ("Lightning Bolt Settings")]
    public int numberOfSplits = 3;
    public float maxSplitOffsetZ = 0.2f;
    public float maxSplitOffsetXY = 0.6f;
    public float destroyTime = 0.5f;

    [Header ("Fork Settings")]
    public int forkNumberOfSplits = 4;
    public int maxForks = 8;
    public int minForks = 3;
    public float maxForwardTravel = 1;
    public float maxForkSplitOffsetXY = 0.05f;

    private float maxSavedForkSplitOffsetXY = 0.05f;
    private float maxSavedSplitOffsetXY = 0.05f;

    [Header ("Safe Guard For While Loop")]
    public int maxWhileLoops = 1000;

    private int maxLines;
    private int maxForksLines;

    private bool hasEmited = false;

    private GameObject parentClone;

    private List<BoltLine> newLines = new List<BoltLine> ();
    private List<BoltLine> newForks = new List<BoltLine> ();
    private List<List<BoltLine>> newAllForks = new List<List<BoltLine>> ();

    void Start()
    {
        maxSavedForkSplitOffsetXY = maxForkSplitOffsetXY;
        maxSavedSplitOffsetXY = maxSplitOffsetXY;

        maxLines = (int)Mathf.Pow (2f, numberOfSplits);
        maxForksLines = (int)Mathf.Pow (2f, forkNumberOfSplits);
        //Debug.Log (maxLines);

    }

    /*void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            StartLightning ();
        }
    }*/

    public void StartLightning()
    {
        if (!hasEmited)
        {
            hasEmited = true;
            StartCoroutine (EmitLightning ());

            float destroyTimeTemp = 0;

            destroyTimeTemp = destroyTime + Random.Range ((-destroyTimeTemp / 2), (destroyTimeTemp / 2));

            Invoke ("DestroyLightning", destroyTimeTemp);
        }
    }

    void DestroyLightning()
    {
        hasEmited = false;

        newLines.Clear ();
        newForks.Clear ();
        newAllForks.Clear ();

        Destroy (parentClone);
    }

    IEnumerator EmitLightning()
    {
        parentClone = Instantiate (parent.gameObject, Vector3.zero, Quaternion.identity) as GameObject;

        maxSplitOffsetXY = maxSavedSplitOffsetXY;

        newLines.Add (new BoltLine (transform.position, target.position));

        int loops = 0;

        while(newLines.Count < maxLines)
        {
            //Safe guard, incase something goes wrong and while loop would starts looping endlessly
            loops++;
            if(loops > maxWhileLoops)
            {
                Debug.LogWarning ("Infinite While Loop During Lightning Bolt Creation !!!!");
                break;
            }

            if(loops > 1)
            {
                maxSplitOffsetXY /= 2;
            }

            List<BoltLine> tempNewLines = new List<BoltLine> ();

            foreach (BoltLine line in newLines)
            {
                //Get middle point node of the current line start and end nodes
                Vector3 midNode = (line.GetStartNode () + line.GetEndNode ()) / 2;

                //Move the middle point node by a random offset value
                midNode.x += Random.Range (-maxSplitOffsetXY, maxSplitOffsetXY);
                midNode.y += Random.Range (-maxSplitOffsetXY, maxSplitOffsetXY);
                midNode.z += Random.Range (-maxSplitOffsetZ, maxSplitOffsetZ);


                //Create 2 new boltlines out of this single split and add to list of newLines
                tempNewLines.Add (new BoltLine (line.GetStartNode (), midNode));
                tempNewLines.Add (new BoltLine (midNode, line.GetEndNode ()));
            }

            newLines.Clear ();
            newLines = tempNewLines;

            //Debug.Log ("Looping bolt generation");
        }

        int randomForkAmount = Random.Range (minForks, maxForks);

        for(int forkAmount = 0; forkAmount < randomForkAmount; forkAmount++)
        {
            int randomLine = Random.Range (0, newLines.Count - 1);

            maxForkSplitOffsetXY = maxSavedForkSplitOffsetXY;

            Vector3 forkTargetPos = newLines[randomLine].GetStartNode ();

            if (-1 == Mathf.Sign (newLines[randomLine].GetStartNode ().z - newLines[randomLine].GetEndNode ().z))
            {
                forkTargetPos.z += maxForwardTravel;
            }
            else
            {
                forkTargetPos.z -= maxForwardTravel;
            }

            forkTargetPos.x += Random.Range (-0.5f, 0.5f);
            forkTargetPos.y += Random.Range (-0.5f, 0.5f);

            newForks.Clear ();
            newForks.Add (new BoltLine (newLines[randomLine].GetStartNode (), forkTargetPos));

            loops = 0;

            while (newForks.Count < maxForksLines)
            {
                //Safe guard, incase something goes wrong and while loop would starts looping endlessly
                loops++;
                if (loops > maxWhileLoops)
                {
                    Debug.LogWarning ("Infinite While Loop During Lightning Bolt Fork Creation !!!!");
                    break;
                }

                if (loops > 1)
                {
                    maxForkSplitOffsetXY /= 2;
                }

                List<BoltLine> tempNewForks = new List<BoltLine> ();

                foreach (BoltLine line in newForks)
                {
                    //Get middle point node of the current line start and end nodes
                    Vector3 midNode = (line.GetStartNode () + line.GetEndNode ()) / 2;

                    //Move the middle point node by a random offset value
                    midNode.x += Random.Range (-maxForkSplitOffsetXY, maxForkSplitOffsetXY);
                    midNode.y += Random.Range (-maxForkSplitOffsetXY, maxForkSplitOffsetXY);

                    //Create 2 new boltlines out of this single split and add to list of newLines
                    tempNewForks.Add (new BoltLine (line.GetStartNode (), midNode));
                    tempNewForks.Add (new BoltLine (midNode, line.GetEndNode ()));
                }

                newForks.Clear ();
                newForks = tempNewForks;

                //Debug.Log ("Looping bolt fork generation");
            }

            newAllForks.Add (new List<BoltLine>(newForks));
        }

        int forLoops = 0;

        foreach (BoltLine line in newLines)
        {
            forLoops++;

            GameObject newBoltLine = Instantiate (lightningBoltMesh, line.GetStartNode (), Quaternion.identity) as GameObject;
            newBoltLine.transform.parent = parentClone.transform;

            newBoltLine.GetComponent<BoltLineUpdate> ().SetBoltLine (line.GetStartNode (), line.GetEndNode ());
        }

        forLoops = 0;

        foreach(List<BoltLine> forkList in newAllForks)
        {
            yield return new WaitForEndOfFrame ();
            foreach (BoltLine line in forkList)
            {
      

                GameObject newForkLine = Instantiate (lightningBoltMesh, line.GetStartNode (), Quaternion.identity) as GameObject;
                newForkLine.transform.parent = parentClone.transform;

                newForkLine.GetComponent<BoltLineUpdate> ().SetBoltLine (line.GetStartNode (), line.GetEndNode ());
            }
        }

        //Debug.Log (newLines.Count + "   " + maxLines);

    }
}
