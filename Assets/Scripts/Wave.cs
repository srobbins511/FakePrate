using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : Attacks
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() {
        if(!GameManager.Instance.isPaused) {
            transform.position = transform.position + new Vector3(1f, 0f, 0f) * Vector3.Magnitude(speed) * Time.deltaTime;
        }
    }

    public void OnBecameInvisible() {
        Destroy(this.gameObject);
    }
}
