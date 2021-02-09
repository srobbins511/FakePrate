using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ColorCode
{
    RED,
    BLUE,
    GREEN,
    MAGENTA,
    BLACK
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
    private bool isPaused;

    // Start is called before the first frame update
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        mRenderer = GetComponent<MeshRenderer>();
        Color targetColor;
        float randomFloat = Random.Range(0.0f, 1.0f);
        if(randomFloat <= 0.24f) {
            colorCode = ColorCode.RED;
        } else if(randomFloat <= 0.48f) {
            colorCode = ColorCode.BLUE;
        } else if(randomFloat <= 0.72f) {
            colorCode = ColorCode.GREEN;
        } else if (randomFloat <= 0.96f) {
            colorCode = ColorCode.MAGENTA;
        } else {
            colorCode = ColorCode.BLACK;
        }

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
            case ColorCode.BLACK:
                targetColor = Color.black;
                break;
            default:
                targetColor = Color.white;
                break;
        }
        mRenderer.material.SetColor("_Color", targetColor); //TODO: FIX because Doesn't work
        Generate();
        GameManager.Instance.OnGamePause += PauseTarget;
        GameManager.Instance.OnGameUnpause += UnpauseTarget;
        GameManager.Instance.DestroyAllTargets += SelfDestruct;
    }

    /// <summary>
    /// This Method is used to allow the player to click on Targets
    /// </summary>
    public void OnMouseDown()
    {
        if(!isPaused) {
            if(colorCode == ColorCode.BLACK) {
                GameManager.Instance.BlowUpEverything();
            } else {
                Activate();
            }
        }
    }

    public void SelfDestruct() {
        GameManager.Instance.Score(pointValue, GameManager.Instance.levelInfo.targetColor);  //TODO: Not yet implemented
        //throw new System.NotImplementedException();
        GameManager.Instance.DestroyAllTargets -= SelfDestruct;
        GameManager.Instance.OnGamePause -= PauseTarget;
        GameManager.Instance.OnGameUnpause -= UnpauseTarget;
        Destroy(gameObject);
    }

    public override void Activate()
    {
        GameManager.Instance.Score(pointValue, colorCode);  //TODO: Not yet implemented
                                                            //throw new System.NotImplementedException();
        GameManager.Instance.DestroyAllTargets -= SelfDestruct;
        GameManager.Instance.OnGamePause -= PauseTarget;
        GameManager.Instance.OnGameUnpause -= UnpauseTarget;
        Destroy(gameObject);
    }

    public override void Generate()
    {
        startLocation = new Vector3(Random.Range(-10f, 10f),-1f,0f);
        transform.position = startLocation;
        yBound = Random.Range(0.5f, 1.5f);
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
        if(!isPaused) {
            transform.position = new Vector3(transform.position.x + speed.x * Time.deltaTime, transform.position.y + 2 * speed.y * Time.deltaTime);
        }
    }

    void PauseTarget() {
        isPaused = true;
    }

    void UnpauseTarget() {
        isPaused = false;
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
            if(!isPaused) {
                speed.y = Mathf.LerpUnclamped(speed.y, targetSpeed, t / yBound);
                t += 0.0001f;
            }
            yield return new WaitForFixedUpdate();
        }
        speed.y = targetSpeed;
    }

    public void OnBecameInvisible()
    {
        GameManager.Instance.DestroyAllTargets -= SelfDestruct;
        GameManager.Instance.OnGamePause -= PauseTarget;
        GameManager.Instance.OnGameUnpause -= UnpauseTarget;
        Destroy(this.gameObject);
    }
}
