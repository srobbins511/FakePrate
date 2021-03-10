using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    static string curDirectory;
    static string saveDirectory;
    static string[] Saves;
    static SavedData[] Data;
    public static int HighestScore;
    public static int EnergyValue;
    static bool saveDetected;
    const int DataLineSize = 4;
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
                EnergyValue = SavedData.GetEnergyValue(Data);
            }
        }
        else
        {
            System.IO.Directory.CreateDirectory(saveDirectory);
            Saves = System.IO.Directory.GetFiles(saveDirectory);
        }
        saveDetected = false;
        return saveDetected;
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
            int energy = int.Parse(data[3]);
            Debug.Log("Energy read from file: " + energy);
            energy += System.DateTime.Now.Subtract(time).Duration().Minutes / 2;
            energy = energy > 100 ? 100 : energy;
            Data[SaveNumber] = new SavedData(score, "", level,time, energy);
        }
    }

    public static void SaveData(string[] data)
    {
        System.IO.File.WriteAllLines(saveDirectory + "\\" + Saves.Length , data);
    }

    public static string[] ConvertDataToSaveFormat(int totalScore, int level, int energyLevel)
    {
        string[] saveText;
        saveText = new string[DataLineSize];
        saveText[0] = System.DateTime.Now.ToString();
        saveText[1] = totalScore.ToString();
        saveText[2] = level.ToString();
        saveText[3] = GameManager.Instance.energyLevel.ToString();
        return saveText;
    }

    public static void Save()
    {
        int curScore = GameManager.Instance.totalScore;
        int curLev = GameManager.Instance.levelInfo.currentLevel;
        int energy = GameManager.Instance.energyLevel;
        SaveData(ConvertDataToSaveFormat(curScore, curLev, energy));
    }


    
}

public struct SavedData
{
    public int HighScore;
    public int level;
    public string PlayerName;
    public System.DateTime gameDate;
    public int Energy;

    public SavedData(int score, string name, int levelReached, System.DateTime time, int energy)
    {
        HighScore = score;
        level = levelReached;
        PlayerName = name;
        gameDate = time;
        Energy = energy;
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

    public static int GetEnergyValue(SavedData[] data)
    {
        System.DateTime latestSave = System.DateTime.MinValue;
        int newEnergy = 100;
        foreach(SavedData d in data)
        {
            latestSave = latestSave >= d.gameDate ? latestSave : d.gameDate;
            if(latestSave.Equals(d.gameDate))
            {
                newEnergy = d.Energy;
            }
        }
        return newEnergy;
    }
}
