using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFactory : MonoBehaviour
{
    public GameObject[] prefabsToGenerate = new GameObject[3];


    public void Start()
    {
        prefabsToGenerate[0] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        prefabsToGenerate[1] = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        prefabsToGenerate[2] = GameObject.CreatePrimitive(PrimitiveType.Cube);
    }
    public void SpawnAtack()
    {
        if(Random.value > .5f)
        {
            //Spawn a Dragon
            GameObject.Instantiate(prefabsToGenerate[1]);
        }
        else
        {
            //Spawn a Wave
            GameObject.Instantiate(prefabsToGenerate[2]);
        }
    }

    public void SpawnTargetBurst(int numTargets)
    {
        GameObject.Instantiate(prefabsToGenerate[0]);
    }
}
