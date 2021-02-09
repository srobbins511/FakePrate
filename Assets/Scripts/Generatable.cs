using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof (Rigidbody))]
[RequireComponent(typeof (MeshRenderer))]
[RequireComponent(typeof (Collider))]


//This is the Abstract Class used as a template for all Generatable Objects created by the Factory
public abstract class Generatable : MonoBehaviour
{
    public float speed;
    protected Collider objCollider;
    protected MeshRenderer mRenderer;
    protected Rigidbody rBody;

    public abstract void Activate();
    public abstract void Generate();
}
