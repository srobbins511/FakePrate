using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFactory : MonoBehaviour {
    public GameObject[] prefabsToGenerate = new GameObject[3];


    public void Start() {
        prefabsToGenerate[0] = GameManager.Instance.TargetPrefab;
        prefabsToGenerate[1] = GameManager.Instance.DragonPrefab;
        prefabsToGenerate[2] = GameManager.Instance.WavePrefab;
        //SpawnTargetBurst(GameManager.Instance.levelInfo.targetSpawnBurstSize);
    }
    public void SpawnAtack() {
        if(Random.value > .5f) {
            //Spawn a Dragon
            GameObject.Instantiate(prefabsToGenerate[1]);
        } else {
            //Spawn a Wave
            GameObject.Instantiate(prefabsToGenerate[2]);
        }
    }

    public void SpawnTargetBurst(int numTargets) {
        for (int i = 0; i < numTargets; ++i) {
            GameObject.Instantiate(prefabsToGenerate[0]);
        }
    }
}
