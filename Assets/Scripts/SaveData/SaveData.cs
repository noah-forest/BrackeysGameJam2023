using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class SaveData : MonoBehaviour
{
    #region singleton

    public static SaveData singleton;

    private void Awake()
    {
        if (singleton)
        {
            Destroy(this.gameObject);
            return;
        }

        singleton = this;
        DontDestroyOnLoad(this.gameObject);
    }
    #endregion
    
    public UnlockMatrix unlockMatrix = new();

    //saves the current unlock matrix to the json file
    public void SaveIntoJson()
    {
        var matrix = JsonUtility.ToJson(unlockMatrix);
        File.WriteAllText(Application.persistentDataPath + "/unlockables.json", matrix);
    }

    //returns the unlock matrix from the json file
    public UnlockMatrix LoadFromJson()
    {
        var matrix = File.ReadAllText(Application.persistentDataPath + "/unlockables.json");
        return JsonUtility.FromJson<UnlockMatrix>(matrix);
    }

    public bool CheckForSaveData()
    {
        var saveData = File.ReadAllText(Application.persistentDataPath + "/unlockables.json");

        if (string.IsNullOrEmpty(saveData))
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}

[Serializable]
public class UnlockMatrix
{
    public List<string> unitsUnlocked = new();
    public List<string> bossesUnlocked = new();
    public List<string> hazardsUnlocked = new();
}
