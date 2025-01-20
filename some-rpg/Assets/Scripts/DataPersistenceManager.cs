using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Manages save data stored in the game's persistent datapath. Saved Object may be of any type.
/// </summary>
/// <typeparam name="T"></typeparam>
public class DataPersistenceManager<T> {
    private static string SAVE_PATH = Application.persistentDataPath + "/saves";
    public void NewData(string filename, T obj) {
        string savefile = Path.Combine(SAVE_PATH, $"{filename}.json");
        if (!Directory.Exists(SAVE_PATH))
            Directory.CreateDirectory(SAVE_PATH);
        
        File.Create(savefile).Dispose();
        File.WriteAllText(savefile, JsonUtility.ToJson(obj));
    }

    public bool Exists(string filename) {
        string savefile = Path.Combine(SAVE_PATH, $"{filename}.json");
        return File.Exists(savefile);
    }

    /// <summary>
    /// Saves the obj parameter to an existing json file
    /// </summary>
    /// <param name="filename">Name of the file, without the extension</param>
    /// <param name="obj">Object to be saved</param>
    /// <returns>Function returns -1 in case of failure to save data, 0 in case of success.</returns>
    public int SaveData(string filename, T obj) {
        string savefile = Path.Combine(SAVE_PATH, $"{filename}.json");
        if (!File.Exists(savefile)) {
            Debug.LogWarning($"[DataPeristence] Error loading save file. {savefile} does not exist!");
            return -1;
        }

        File.WriteAllText(savefile, JsonUtility.ToJson(obj));
        return 0;
    }

    /// <summary>
    /// Loads an object and passes the result to the obj parameter
    /// </summary>
    /// <param name="filename">Name of the file, without the extension</param>
    /// <param name="obj">Object to take on the information from the file</param>
    /// <returns>Function returns -1 in case of failure to load data, 0 in case of success.</returns>
    public int LoadData(string filename, ref T obj) {
        string savefile = Path.Combine(SAVE_PATH, $"{filename}.json");
        if (!File.Exists(savefile)) {
            Debug.LogError($"[DataPeristence] Error loading save file. {savefile} does not exist!");
            return -1;
        }

        try {  
            obj = JsonUtility.FromJson<T>(File.ReadAllText(savefile));
            return 0;
        } catch (FormatException) {
            Debug.LogError($"[DataPersistence] Error loading save file. {savefile} is not of type {typeof(T)}!");
            return -1;
        }
    }
}
