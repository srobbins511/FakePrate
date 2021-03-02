using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    static string curDirectory;
    static string saveDirectory;
    static string[] Saves;
    static bool saveDetected;
    const int DataLineSize = 3;
    // Start is called before the first frame update
    void Awake()
    {
        
        curDirectory = System.IO.Directory.GetCurrentDirectory();
        /*
        saveFileName = "Svae1.txt";
        Debug.Log(curDirectory);


        string[] testText;
        testText = new string[3];
        testText[0] = System.DateTime.Now.ToString();
        testText[1] = "Total Score:" + 0;
        testText[2] = "Test Test";


        if (System.IO.Directory.Exists(curDirectory + "\\Saves"))
        {
            Debug.Log("Save Direcory Exists");
            
        }
        else
        {
            Debug.Log("Save Directory Doesnt exist");
            System.IO.Directory.CreateDirectory(curDirectory + "\\Saves");
        }
        string[] fileInput;
        if (System.IO.File.Exists(curDirectory + "\\Saves\\" + saveFileName))
        {
            fileInput = System.IO.File.ReadAllLines(curDirectory + "\\Saves\\" + saveFileName);
            System.IO.File.WriteAllLines(curDirectory + "\\Saves\\" + saveFileName, testText);

        }
        else
        {
            System.IO.File.Create(curDirectory + "\\Saves\\" + saveFileName);
            System.IO.File.WriteAllLines(curDirectory + "\\Saves\\" + saveFileName, testText);
        }

        Debug.Log("End Start Method");
        */
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
            if(Saves.Length > 0)
            {
                foreach(string s in Saves)
                {
                    ReadSaveData(System.IO.File.ReadAllLines(s));
                    Debug.Log(s);
                }
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

    static private void ReadSaveData(string[] data)
    {
        if(data.Length != DataLineSize)
        {
            throw new System.Exception("Invalid File Read");
        }
        else
        {
            int score = int.Parse(data[1]);
            Debug.Log("Score: " + score);
            int level = int.Parse(data[2]);
            Debug.Log("Level: " + level);
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


    struct SavedData
    {
        int HighScore;
        int level;
        string PlayerName;
        Time gameDate;

        SavedData(int score, string name, int levelReached, Time time)
        {
            HighScore = score;
            level = levelReached;
            PlayerName = name;
            gameDate = time;
        }
    }
}
