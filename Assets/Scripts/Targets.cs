using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ColorCode {
    RED,
    BLUE,
    GREEN,
    MAGENTA,
    BLACK
};

public class Targets : Generatable {

    public int pointValue;
    //TODO: What is isBurned supposed to mean?
    public bool isBurned;
    public ColorCode colorCode;
    Vector3 startLocation;
    public float yBound;
    public float xDirection;
    public uint UID;

    private bool isGoingDown;
    private bool isPaused;
    private Coroutine arcMovement;

    public float SpeedVariance;
    public float SpeedConstant;

    // Start is called before the first frame update
    void Awake() {
        rBody = GetComponent<Rigidbody>();
        mRenderer = GetComponent<MeshRenderer>();
        GameManager.Instance.OnGamePause += PauseTarget;
        GameManager.Instance.OnGameUnpause += UnpauseTarget;
        GameManager.Instance.DestroyAllTargets += SelfDestruct;
    }

    /// <summary>
    /// This Method is used to allow the player to click on Targets
    /// </summary>
    public void OnMouseDown() {
        if(!isPaused) {
            if(colorCode == ColorCode.BLACK) {
                GameManager.Instance.BlowUpEverything();
            } else {
                Activate();
            }
            //NetworkManager.Instance.SendString("Destroyed: " + UID);
        }
    }

    public void SelfDestruct() {
        if (gameObject.activeSelf) {
            GameManager.Instance.Score(pointValue, ColorCode.BLACK);
            transform.position = new Vector3(0, -5, 0);
            gameObject.SetActive(false);
        }
    }

    public override void Activate() {
        GameManager.Instance.Score(pointValue, colorCode); 
        transform.position = new Vector3(0, -5, 0);
        gameObject.SetActive(false);
    }

    public override void Generate() {
        //TODO: Move trajectory/location randomization to spawnfactory.  Make speed adjustable as a float without breaking everything
        gameObject.SetActive(true);
        startLocation = new Vector3(GameManager.RandRange(-10f, 10f),-1f,0f);
        transform.position = startLocation;
        yBound = GameManager.RandRange(0.5f, 1.5f);
        xDirection = GameManager.RandRange(-6f, 6f);
        xDirection = xDirection + startLocation.x;
        if(xDirection < -9) {
            xDirection = -9;
        }
        else if(xDirection > 9) {
            xDirection = 9;
        }


        //This region of Code controlls the color selection of each target
        #region colorGeneration
        Color targetColor;
        float randomFloat = GameManager.RandFloat();
        //Chance of color based on GameManager
        if (randomFloat <= GameManager.Instance.levelInfo.goalTargetSpawnChance)
        {
            colorCode = GameManager.Instance.levelInfo.targetColor;
        }
        else if (randomFloat <= (1 - GameManager.Instance.levelInfo.goalTargetSpawnChance) / 3 + GameManager.Instance.levelInfo.goalTargetSpawnChance)
        {
            colorCode = (ColorCode)((int)(GameManager.Instance.levelInfo.targetColor + 1) % (int)(ColorCode.BLACK));
        }
        else if (randomFloat <= ((1 - GameManager.Instance.levelInfo.goalTargetSpawnChance) * 2) / 3 + GameManager.Instance.levelInfo.goalTargetSpawnChance)
        {
            colorCode = (ColorCode)((int)(GameManager.Instance.levelInfo.targetColor + 2) % (int)(ColorCode.BLACK));
        }
        else if (randomFloat <= (1 - GameManager.Instance.levelInfo.goalTargetSpawnChance) + GameManager.Instance.levelInfo.goalTargetSpawnChance)
        {
            colorCode = (ColorCode)((int)(GameManager.Instance.levelInfo.targetColor + 3) % (int)(ColorCode.BLACK));
        }

        randomFloat = GameManager.RandFloat();
        //Doing Black chance separately
        if (randomFloat <= 0.02f)
        {
            colorCode = ColorCode.BLACK;
        }

        switch (colorCode)
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
        mRenderer.material.SetColor("_Color", targetColor);
        #endregion


        CalculateTrajectoryEquation();
    }

    
    // Update is called once per frame
    void Update() {

    }

    /// <summary>
    /// This is the method that calculates the constants needed to create the parabolic arc of each target
    /// It then starts the movement coroutine for the targets
    /// </summary>
    void CalculateTrajectoryEquation()
    {
        bool isBackwards = GameManager.RandRange(0, 2) == 0;
        float Slope = GameManager.RandRange(2f, 5f);
        float xTranslation = GameManager.RandRange(-9f, 9f);
        float yIntercept = GameManager.RandRange(3f, 5f);
        float xRoot2 = Mathf.Sqrt(yIntercept / Slope) - xTranslation;
        float xRoot1 = -Mathf.Sqrt(yIntercept / Slope) - xTranslation;
        Debug.Log("FLOATS: " + xTranslation + yIntercept + xRoot1 + xRoot2);
        arcMovement = StartCoroutine(FollowPath(Slope, xTranslation, yIntercept, xRoot1,xRoot2, isBackwards));
    }

    /// <summary>
    /// The movement cooutine of the targets
    /// 
    /// It moves a target along a parabolic arc given by the different constants passed in along with a boolean determining which way to follow the arc and what root to start at
    /// 
    /// It uses two constants determined by the editor to adjust the movement speed of the target, one will be constant for every target created and one will depend on the x-distance the target
    /// will have to travel. This will allow targets that have to move in wider arcs to have a little bit of a speed boost in order to make it there in a timely, somewhat unison manner
    /// </summary>
    /// <param name="A"></param>
    /// <param name="B"></param>
    /// <param name="C"></param>
    /// <param name="xRoot1"></param>
    /// <param name="xRoot2"></param>
    /// <param name="isBackwards"></param>
    /// <returns></returns>
    IEnumerator FollowPath(float A, float B, float C, float xRoot1, float xRoot2, bool isBackwards)
    {
        bool isFinished = false;
        if(!isBackwards)
        {
            float t = xRoot1;
            while(!isFinished)
            {
               
                if(!isPaused)
                {
                    t += Time.deltaTime*SpeedConstant + Time.deltaTime * Mathf.Sqrt(Mathf.Abs(xRoot1 - xRoot2))*SpeedVariance;
                    transform.position = new Vector3(t, -A * Mathf.Pow(t + B, 2) + C, 0);
                }
                yield return new WaitForEndOfFrame();
                if (t > xRoot2 + 2)
                {
                    isFinished = true;
                }
            }
        }
        else
        {
            float t = xRoot2;
            while (!isFinished)
            {
                if (!isPaused)
                {
                    t -= Time.deltaTime * SpeedConstant + Time.deltaTime * Mathf.Sqrt(Mathf.Abs(xRoot1 - xRoot2)) * SpeedVariance;
                    transform.position = new Vector3(t, -A * Mathf.Pow(t + B, 2) + C, 0);
                }
                yield return new WaitForEndOfFrame();
                if (t < xRoot1 - 2)
                {
                    isFinished = true;
                }
            }
        }
    }

    void PauseTarget() {
        isPaused = true;
    }

    void UnpauseTarget() {
        isPaused = false;
    }


    public void OnBecameInvisible() {
        transform.position = new Vector3(0, -5);
        StopCoroutine(arcMovement);
        gameObject.SetActive(false);
    }
}
