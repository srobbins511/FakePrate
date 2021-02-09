using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ColorCode
{
    RED,
    BLUE,
    GREEN,
    MAGENTA
};

public class Targets : Generatable
{

    public int pointValue;
    public bool isElectric;
    public bool isBurned;
    public ColorCode colorCode;

    // Start is called before the first frame update
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        mRenderer = GetComponent<MeshRenderer>();
        Color targetColor;
        switch(colorCode)
        {
            case ColorCode.RED:
                targetColor = Color.red;
                break;
            case ColorCode.BLUE:
                targetColor = Color.blue;
                break;
            case ColorCode.GREEN:
                targetColor = Color.green;
                break;
            case ColorCode.MAGENTA:
                targetColor = Color.magenta;
                break;
            default:
                targetColor = Color.white;
                break;
        }
        mRenderer.material.SetColor("_Color", targetColor); //TODO: FIX because Doesn't work
    }

    //Here create a method that allows the system to detect when the user has clicked on a target

    public void OnMouseDown()
    {
        Activate();
    }
    //--------------------------------------------------------------------------------------

    public override void Activate()
    {
        Debug.Log("Clicked");
        GameManager.Instance.Score(pointValue);  //TODO: Not yet implemented
        //throw new System.NotImplementedException();
        Destroy(gameObject);
    }

    public override void Generate()
    {
        //TODO: Spawn -10 to 10 x, negative y, 0 z, and start movement upwards
        //NOTE: Prototype movement to just be go up and down in y
        throw new System.NotImplementedException();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
