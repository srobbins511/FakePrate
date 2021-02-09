﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int lives;
    public bool isPaused;
    int totalScore;
    Coroutine spawnTimer;

    public delegate void SimpleEventHandler();
    public GameObject TargetPrefab;
    public Text scoreText;
    public Image targetColorImage;

    public event SimpleEventHandler OnGamePause;
    public event SimpleEventHandler OnGameUnpause;
    public event SimpleEventHandler OnLevelWin;
    public event SimpleEventHandler OnLevelLose;
    public event SimpleEventHandler DestroyAllTargets;

    public static GameManager Instance;

    public LevelInfo levelInfo;

    private SpawnFactory Factory;

    // Start is called before the first frame update
    void Start()
    {
        if(Instance == null)
        {
            Instance = this;
        } 
        else
        {
            Destroy(this);
        }
        setLevelInfo(new LevelInfo(1, 0, 1f, 0.8f, 5, 15, new List<float>(), new List<float>(), ColorCode.RED, 10));
        Factory = gameObject.AddComponent<SpawnFactory>();
        StartCoroutine(SpawnTimer());
    }



    /// <summary>
    /// This method controls when the player hits the pause button in the UI the Game pauses and the pause screen is activated.
    /// It also invokes the correct Pause Events.
    /// </summary>
    public void Pause()
    {
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
    IEnumerator SpawnTimer()
    {
        float timeSinceTargetSpawnBurst = 0;
        int targetSpawnBurstIndexPos = 0;
        int attackSpawnIndexPos = 0;
        float timeSinceAttackSpawn = 0;
        while (true)
        {
            if(isPaused)
            {
                yield return new WaitUntil(() => isPaused == false);
            }

            //this should contain the call to factory for spawning targets
            if(timeSinceTargetSpawnBurst > 2f /*levelInfo.TargetSpawnTimes[targetSpawnBurstIndexPos]*/)
            {
                /*
                targetSpawnBurstIndexPos++;
                if(levelInfo.TargetSpawnTimes.Count <= targetSpawnBurstIndexPos)
                {
                    targetSpawnBurstIndexPos = 0;
                }
                */
                //Update factory Call to Use autogenerated Variable numbers
                //TODO: Replace this with gameManager references, and probably swap Burst size for upper/lower bounds
                Factory.SpawnTargetBurst(Random.Range(5,15));
                timeSinceTargetSpawnBurst = 0;
            }
            /*
            //this should contain the call to factory for spawning attacks
            if(timeSinceAttackSpawn > levelInfo.AttackSpawnTimes[attackSpawnIndexPos])
            {
                attackSpawnIndexPos++;
                if(levelInfo.AttackSpawnTimes.Count <= attackSpawnIndexPos)
                {
                    attackSpawnIndexPos = 0;
                }

                //factory call here
                Factory.SpawnAtack();
            }
            */

            yield return new WaitForEndOfFrame();
            timeSinceAttackSpawn += Time.deltaTime;
            timeSinceTargetSpawnBurst += Time.deltaTime;
        }
    }
   

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P)) {
            Pause();
        }
    }

    public void Score(float pointValue, ColorCode colorCode)
    {
        if(colorCode == levelInfo.targetColor) {
            totalScore += (int)(pointValue * levelInfo.scoreMultiplier);
            levelInfo.numTargetsToWin -= 1;
        } else {
            totalScore -= (int)(pointValue * levelInfo.scoreMultiplier);
        }
        scoreText.text = "Current Score: " + totalScore + "\nCurrent Level: " + levelInfo.currentLevel + "\nTargets to next Level: " + levelInfo.numTargetsToWin;
        if(levelInfo.numTargetsToWin <= 0) {
            //TODO: Stop this from being a mess
            ColorCode nextColor = (ColorCode)((int)(levelInfo.targetColor + 1)%4);
            int nextLevel = levelInfo.currentLevel + 1;
            float nextScoreMult = levelInfo.scoreMultiplier + 0.1f;
            //TODO: Use a better formula for nextColorSpawnChance
            float nextLevelColorSpawnChance = Mathf.Lerp(levelInfo.goalTargetSpawnChance, 0.4f, 0.5f); //Lerps towards 0.4f spawn chance
            LevelInfo nextLevelInfo = new LevelInfo(nextLevel, 0, nextScoreMult, nextLevelColorSpawnChance, 5, 15, new List<float>(), new List<float>(), nextColor, 10);
            setLevelInfo(nextLevelInfo);
        }
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
}

[System.Serializable]
public struct LevelInfo
{
    
    public int currentLevel;
    public int currentScore;
    public float scoreMultiplier;
    public int targetSpawnBurstSize;
    public float goalTargetSpawnChance;
    public List<float> TargetSpawnTimes;
    public List<float> AttackSpawnTimes;
    public ColorCode targetColor;
    public float numTargetsToWin;

    //TODO: Evaluate whether or not the Lists of floats are really necessary.  Remove them if not
    //TODO: Evaluate whether or not currentScore is required.  If not remove it
    public LevelInfo(int curLevel, int curScore, float scoreMult, float tarSpawnChance, int spawnSizeLowBound, int spawnSizeUpperBound, List<float> tarSpawnTimes, List<float> attackSpawnTimes, ColorCode targetColor, float numTargetsToWin)
    {
        this.currentLevel = curLevel;
        this.currentScore = curScore;
        this.scoreMultiplier = scoreMult;
        this.goalTargetSpawnChance = tarSpawnChance;
        this.targetSpawnBurstSize = Random.Range(spawnSizeLowBound, spawnSizeUpperBound);
        this.TargetSpawnTimes = tarSpawnTimes;
        this.AttackSpawnTimes = attackSpawnTimes;
        this.targetColor = targetColor;
        this.numTargetsToWin = numTargetsToWin;
    }
}
