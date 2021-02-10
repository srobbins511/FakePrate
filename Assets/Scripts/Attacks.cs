using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacks : Generatable
{

    public override void Generate() {
        throw new System.NotImplementedException();
    }

    public override void Activate() {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBecameInvisible() {
        Destroy(this.gameObject);
    }
}
