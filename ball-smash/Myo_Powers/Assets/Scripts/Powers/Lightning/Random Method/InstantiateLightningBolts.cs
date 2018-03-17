using UnityEngine;
using System.Collections;

public class InstantiateLightningBolts : MonoBehaviour
{

    /* 
    Klaudijus Miseckas 
    SID:1334116 
    Not Used Class In The Demo
    */

    public GameObject bolt;

    public float boltSpawnRate = 1.5f;
    public int boltsPerSpawn = 1;

    private float nextBoltSpawn = 0;

    private bool canSpawn = true;

    void Update()
    {
        if (canSpawn)
        {
            if (Time.time > nextBoltSpawn)
            {
                for (int i = 0; i < boltsPerSpawn; i++)
                {
                    Instantiate (bolt, transform.position, Quaternion.identity);
                }

                nextBoltSpawn = Time.time + boltSpawnRate;
            }
        }
    }
}
