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
    Vector3 startLocation;
    public float yBound;
    public float xDirection;

    private bool isGoingDown;

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
        Generate();

    }

    /// <summary>
    /// This Method is used to allow the player to click on Targets
    /// </summary>
    public void OnMouseDown()
    {
        Activate();
    }

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

        startLocation = new Vector3(Random.Range(-10f, 10f),-1f,0f);
        transform.position = startLocation;
        yBound = Random.Range(2f, 5f);
        xDirection = Random.Range(-6f, 6f);
        xDirection = xDirection + startLocation.x;
        if(xDirection < -9)
        {
            xDirection = -9;
        }
        else if(xDirection > 9)
        {
            xDirection = 9;
        }

        speed.x = (xDirection - startLocation.x)/10;
        StartCoroutine(LerpTo(-speed.y));
        //speed.x = Vector3.Normalize(speed).x;
        //throw new System.NotImplementedException();
    }

    
    // Update is called once per frame
    void Update()
    {

        if(transform.position.y > yBound && !isGoingDown)
        {
            isGoingDown = true;
            
        }
        
        
        transform.position = new Vector3(transform.position.x + speed.x*Time.deltaTime, transform.position.y + speed.y * Time.deltaTime);
    }

    /// <summary>
    /// This function lerps the speed of a target in the y direction to its negative mirror, causing the target to fall
    /// </summary>
    /// <param name="targetSpeed"></param>
    /// <returns></returns>
    IEnumerator LerpTo(float targetSpeed)
    {
        float t = 0;
        targetSpeed = -Mathf.Pow(targetSpeed, 2);
        while(t < 1)
        {
            speed.y = Mathf.LerpUnclamped(speed.y, targetSpeed, t/yBound);
            t += 0.0001f;
            yield return new WaitForFixedUpdate();
        }
        speed.y = targetSpeed;
    }

    public void OnBecameInvisible()
    {
        Destroy(this.gameObject);
    }
}
