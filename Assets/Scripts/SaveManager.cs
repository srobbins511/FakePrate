using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    static string curDirectory;
    static string saveDirectory;
    static string[] Saves;
    static SavedData[] Data;
    static int HighestScore;
    static bool saveDetected;
    const int DataLineSize = 3;
    // Start is called before the first frame update
    public static void StartUp()
    {
        curDirectory = System.IO.Directory.GetCurrentDirectory();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// This Method detects if there is a save file directory, if there is not it creates one
    /// </summary>
    /// <returns></returns>
    public static bool DetectSaves()
    {
        saveDirectory = curDirectory + "\\Saves";
        if (System.IO.Directory.Exists(saveDirectory))
        {
            Saves = System.IO.Directory.GetFiles(saveDirectory);
            Data = new SavedData[Saves.Length];
            for(int i = 0; i < Saves.Length; ++i)
            {
                ReadSaveData(System.IO.File.ReadAllLines(Saves[i]),i);
            }
            if(Saves.Length > 0)
            {
                saveDetected = true;
                HighestScore = SavedData.getHighScore(Data);
                return true;
            }
        }
        else
        {
            System.IO.Directory.CreateDirectory(saveDirectory);
            Saves = System.IO.Directory.GetFiles(saveDirectory);
        }
        saveDetected = false;
        return false;
    }

    static private void ReadSaveData(string[] data, int SaveNumber)
    {
        if(data.Length != DataLineSize)
        {
            throw new System.Exception("Invalid File Read");
        }
        else
        {
            int score = int.Parse(data[1]);
            int level = int.Parse(data[2]);
            System.DateTime time = System.DateTime.Parse(data[0]);
            Data[SaveNumber] = new SavedData(score, "", level,time);
        }
    }

    public static void SaveData(string[] data)
    {
        System.IO.File.WriteAllLines(saveDirectory + "\\" + Saves.Length , data);
    }

    public static string[] ConvertDataToSaveFormat(int totalScore, int level)
    {
        string[] saveText;
        saveText = new string[3];
        saveText[0] = System.DateTime.Now.ToString();
        saveText[1] = "" + totalScore;
        saveText[2] = "" + level;
        return saveText;
    }

    public static void Save()
    {
        int curScore = GameManager.Instance.totalScore;
        int curLev = GameManager.Instance.levelInfo.currentLevel;
        SaveData(ConvertDataToSaveFormat(curScore, curLev));
    }


    
}

public struct SavedData
{
    public int HighScore;
    public int level;
    public string PlayerName;
    public System.DateTime gameDate;

    public SavedData(int score, string name, int levelReached, System.DateTime time)
    {
        HighScore = score;
        level = levelReached;
        PlayerName = name;
        gameDate = time;
    }

    public static int getHighScore(SavedData[] data)
    {
        int max = 0;
        foreach(SavedData d in data)
        {
            max = max>=d.HighScore ? max: d.HighScore;
        }
        return max;
    }
}
