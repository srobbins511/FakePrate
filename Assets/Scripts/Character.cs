using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    private float yOffset;
    // Start is called before the first frame update
    void Start()
    {
        yOffset = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.S)) {
            transform.parent.localScale = new Vector3(transform.parent.localScale.x, 0.5f, transform.parent.localScale.z);
            yOffset = -0.5f;
        } else {
            transform.parent.localScale = new Vector3(transform.parent.localScale.x, 1f, transform.parent.localScale.z);
            yOffset = 0;
        }

        if (Input.GetKey(KeyCode.Space)) {
            transform.position = new Vector3(transform.position.x, 3.0f + yOffset, transform.position.z);
        } else {
            transform.position = new Vector3(transform.position.x, 1f + yOffset, transform.position.z);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.GetComponent<Attacks>()) {
            Debug.Log("HIT BY ATTACK!");
            GameManager.Instance.loseLife();
            if (GameManager.Instance.lives <= 0) {
                Destroy(gameObject);
            }
        }
    }
}
