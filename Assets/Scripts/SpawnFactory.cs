using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFactory : MonoBehaviour {
    public GameObject[] prefabsToGenerate = new GameObject[3];


    public void Awake() {
        prefabsToGenerate[0] = GameManager.Instance.TargetPrefab;
        prefabsToGenerate[1] = GameManager.Instance.DragonPrefab;
        prefabsToGenerate[2] = GameManager.Instance.WavePrefab;
        //SpawnTargetBurst(GameManager.Instance.levelInfo.targetSpawnBurstSize);
    }
    public void SpawnAtack() {
        if(GameManager.RandFloat() > .5f) {
            //Spawn a Dragon
            GameObject.Instantiate(prefabsToGenerate[1]);
        } else {
            //Spawn a Wave
            GameObject.Instantiate(prefabsToGenerate[2]);
        }
    }

    public Targets[] SpawnTargetBurst(int numTargets) {
        Targets[] temp = new Targets[numTargets];
        for (int i = 0; i < numTargets; ++i) {
            temp[i] = GameObject.Instantiate(prefabsToGenerate[0]).GetComponent<Targets>();
            temp[i].transform.position = new Vector3(0, -5, 0);
            temp[i].gameObject.SetActive(false);
            temp[i].UID = (uint)i;
        }
        return temp;
    }
}
