using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataPersistenceManager {
    private static string SAVE_PATH = Application.persistentDataPath + "/saves";
    public void NewGame(string filename) {
        string savefile = SAVE_PATH + $"{filename}.json";
        if (!Directory.Exists(SAVE_PATH))
            Directory.CreateDirectory(SAVE_PATH);
        
        File.Create(savefile);
        File.WriteAllText(savefile, JsonUtility.ToJson(new PlayerDataManager()));
    }

    public void SaveGame() {

    }

    public void LoadGame() {
        
    }
}
