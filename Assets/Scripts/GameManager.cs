﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//TODO: Add Functionality that changes targets attributes and Target Color when a Bonus Level Should be created, Thge isBonus variable should be correctly set based on accuracy but is untested
public class GameManager : MonoBehaviour {
    public int lives = 3;
    public bool isPaused;
    int totalScore;
    Coroutine spawnTimer;

    public delegate void SimpleEventHandler();
    public GameObject TargetPrefab;
    public GameObject DragonPrefab;
    public GameObject WavePrefab;
    public Text scoreText;
    public Image targetColorImage;

    public event SimpleEventHandler OnGamePause;
    public event SimpleEventHandler OnGameUnpause;
    public event SimpleEventHandler OnLevelWin;
    public event SimpleEventHandler OnLevelLose;
    public event SimpleEventHandler DestroyAllTargets;

    public static GameManager Instance;

    public LevelInfo levelInfo;

    public int numTargetsInPool = 55;

    private SpawnFactory Factory;

    private Targets[] TargetPool;
    private int targetPoolIndex;

    private bool reachedNextLevel;
    private bool bonusLevel;

    // Start is called before the first frame update
    void Awake() {
        if(Instance == null) {
            Instance = this;
        } else {
            Destroy(this);
        }
        setLevelInfo(new LevelInfo(1, 0, 1f, 0.8f, 5.0f, 2, 5, ColorCode.RED, 4f, 5f, false));
        Factory = gameObject.AddComponent<SpawnFactory>();
        bonusLevel = false;
        
        scoreText.text = "Current Score: " + totalScore + "\tCurrent Level: " + levelInfo.currentLevel + "\nTargets to next Level: " + levelInfo.numTargetsToWin + "\tLives: " + lives;
        
        StartCoroutine(SpawnTimer());
    }



    /// <summary>
    /// This method controls when the player hits the pause button in the UI the Game pauses and the pause screen is activated.
    /// It also invokes the correct Pause Events.
    /// </summary>
    public void Pause() {
        isPaused = !isPaused;
        if(isPaused)
        {
            OnGamePause?.Invoke();
        }
        else
        {
            OnGameUnpause?.Invoke();
        }
    }
    
    /// <summary>
    /// This Coroutine is used to evalute the times that new Targets and new attacks should be generated by the Factory
    /// If the Game is Paused this should get caught in the is paused condition
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnTimer() {
        float timeSinceTargetSpawnBurst = GameManager.Instance.levelInfo.timeBetweenBursts - 1;
        TargetPool = Factory.SpawnTargetBurst(numTargetsInPool);
        float timeSinceAttackSpawn = 0;
        targetPoolIndex = 0;
        while (true) {
            if(isPaused) {
                yield return new WaitUntil(() => isPaused == false);
            }

            //Spawns bursts of targets utilizing SpawnFactory
            if(timeSinceTargetSpawnBurst > GameManager.Instance.levelInfo.timeBetweenBursts)
            {
                for(int i = 0; i < Random.Range(levelInfo.targetSpawnMin,levelInfo.targetSpawnMax); ++i)
                {
                    TargetPool[(targetPoolIndex + 1) % TargetPool.Length].Generate();
                    targetPoolIndex += 1;
                }
                timeSinceTargetSpawnBurst = 0;
            }

            //Spawn attack utilizing SpawnFactory
            if(timeSinceAttackSpawn > GameManager.Instance.levelInfo.timeBetweenAttacks) {
                Factory.SpawnAtack();
                timeSinceAttackSpawn = 0;
            }

            yield return new WaitForEndOfFrame();
            timeSinceAttackSpawn += Time.deltaTime;
            timeSinceTargetSpawnBurst += Time.deltaTime;
        }
    }
   

    // Update is called once per frame
    void Update() {
        if(Input.GetKeyDown(KeyCode.P)) {
            Pause();
        }
        if(Input.GetKeyDown(KeyCode.D)) {
            //Debug to get to higher levels easier.
            BlowUpEverything();
        }
        if(Input.GetKeyDown(KeyCode.B))
        {
            for (int i = 0; i < Random.Range(levelInfo.targetSpawnMin, levelInfo.targetSpawnMax); ++i)
            {
                TargetPool[(targetPoolIndex + 1) % TargetPool.Length].Generate();
                targetPoolIndex += 1;
            }
        }
    }

    public void Score(float pointValue, ColorCode colorCode) {
        if (colorCode == levelInfo.targetColor) {
            totalScore += (int)(pointValue * levelInfo.scoreMultiplier);
            levelInfo.numTargetsToWin -= 1;
            levelInfo.Accuracy.x += 1;
            levelInfo.Accuracy.y += 1;
        } else if (colorCode == ColorCode.BLACK) {
            totalScore += (int)(pointValue * levelInfo.scoreMultiplier);
            levelInfo.numTargetsToWin -= 1;
        } else {
            totalScore -= (int)(pointValue * levelInfo.scoreMultiplier);
            levelInfo.Accuracy.y += 1;
        }
        if(levelInfo.numTargetsToWin <= 0 && !reachedNextLevel) {
            //TODO: Stop this from being a mess
            ColorCode nextColor = (ColorCode)((int)(levelInfo.targetColor + 1)%4);
            int nextLevel = levelInfo.currentLevel + 1;
            float nextScoreMult = levelInfo.scoreMultiplier + 0.1f;
            float nextTimeBetweenBursts = levelInfo.timeBetweenBursts - 0.05f;
            nextTimeBetweenBursts = (nextTimeBetweenBursts <= 1.0f) ? 1.0f : nextTimeBetweenBursts;
            int nextTargetSpawnMin = (levelInfo.targetSpawnMin >= 11) ? 11 : levelInfo.targetSpawnMin + 1;
            int nextTargetSpawnMax = (levelInfo.targetSpawnMax >= 15) ? 15 : levelInfo.targetSpawnMax + 1;
            
            int nextTargetsNeeded = Mathf.CeilToInt(3 + (50 - 3) * Mathf.Pow(1f / (1f + Mathf.Exp(-0.1f*levelInfo.currentLevel)), 5f));
            //TODO: Use a better formula for nextColorSpawnChance
            float nextLevelColorSpawnChance = Mathf.Lerp(levelInfo.goalTargetSpawnChance, 0.4f, 0.5f); //Lerps towards 0.4f spawn chance
            bonusLevel = !bonusLevel && levelInfo.GetAccuracy() == 1f;
            nextLevelColorSpawnChance = bonusLevel ? 1 : nextLevelColorSpawnChance;
            reachedNextLevel = true;
            BlowUpEverything();
            LevelInfo nextLevelInfo = new LevelInfo(nextLevel, 0, nextScoreMult, nextLevelColorSpawnChance, nextTimeBetweenBursts, nextTargetSpawnMin, nextTargetSpawnMax, nextColor, nextTargetsNeeded, 5f, bonusLevel);
            setLevelInfo(nextLevelInfo);
            reachedNextLevel = false;
        }
        scoreText.text = "Current Score: " + totalScore + "\tCurrent Level: " + levelInfo.currentLevel + "\nTargets to next Level: " + levelInfo.numTargetsToWin + "\tLives: " + lives + "\nAccuracy: " + levelInfo.GetAccuracy();
    }

    void setLevelInfo(LevelInfo levelInfo) {
        this.levelInfo = levelInfo;
        switch (levelInfo.targetColor) {
            case ColorCode.RED:
                targetColorImage.color = Color.red;
                break;
            case ColorCode.BLUE:
                targetColorImage.color = Color.blue;
                break;
            case ColorCode.GREEN:
                targetColorImage.color = Color.green;
                break;
            case ColorCode.MAGENTA:
                targetColorImage.color = Color.magenta;
                break;
            default:
                targetColorImage.color = Color.white;
                break;
        }
    }

    public void BlowUpEverything() {
        DestroyAllTargets?.Invoke();
    }

    public void loseLife() {
        lives--;
        scoreText.text = "Current Score: " + totalScore + "\tCurrent Level: " + levelInfo.currentLevel + "\nTargets to next Level: " + levelInfo.numTargetsToWin + "\tLives: " + lives;
    }
}

[System.Serializable]
public struct LevelInfo {
    public int currentLevel;
    public int currentScore;
    public float scoreMultiplier;
    public float timeBetweenBursts;
    public int targetSpawnMin;
    public int targetSpawnMax;
    public float goalTargetSpawnChance;
    public ColorCode targetColor;
    public float numTargetsToWin;
    public float timeBetweenAttacks;
    [Tooltip("The x field stores the numebr of successful hits and the y field stores the total number of hits, to get percentage as a float use GetAccuracy")]
    public Vector2Int Accuracy;
    public bool isBonus;

    //TODO: Evaluate whether or not the Lists of floats are really necessary.  Remove them if not
    //TODO: Evaluate whether or not currentScore is required.  If not remove it
    public LevelInfo(int curLevel, int curScore, float scoreMult, float tarSpawnChance, float timeBetweenBursts, int spawnSizeLowBound, int spawnSizeUpperBound, ColorCode targetColor, float numTargetsToWin, float timeBetweenAttacks, bool isBonus)
    {
        this.currentLevel = curLevel;
        this.currentScore = curScore;
        this.scoreMultiplier = scoreMult;
        this.goalTargetSpawnChance = tarSpawnChance;
        this.timeBetweenBursts = timeBetweenBursts;
        this.targetSpawnMin = spawnSizeLowBound;
        this.targetSpawnMax = spawnSizeUpperBound;
        this.targetColor = targetColor;
        this.numTargetsToWin = numTargetsToWin;
        this.timeBetweenAttacks = timeBetweenAttacks;
        Accuracy = new Vector2Int(0, 0);
        this.isBonus = isBonus;
    }

    /// <summary>
    /// This method returns the float ratio of the number of correct hits over total number of hits
    /// If no hits have been recorded it returns a 1 for 100% accuracy
    /// </summary>
    /// <returns></returns>
    public float GetAccuracy()
    {
        float temp;
        if (Accuracy.y == 0)
        {
            temp = 1f;
        }
        else
        {
            temp = ((float)Accuracy.x) / ((float)Accuracy.y); 
        }
        return temp;
    }
}
