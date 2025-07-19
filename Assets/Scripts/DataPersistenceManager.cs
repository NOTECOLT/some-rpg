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
    
    #region Return Signals
    public static int RET_SAVE_FAIL = 0;
    public static int RET_SAVE_SUCCESS = 1;
    public static int RET_SAVE_WRONG_VERSION = 2;
    #endregion

    /// <summary>
    /// Corresponds to the currently used save version of the game. This value is updated as the savedata goes through new iterations.
    /// Helps prevent data corruption or game crashing from outdate savefiles.
    /// </summary>
    private static int SAVE_VERSION = 2;

    /// <summary>
    /// Wrapper that is serialized in to json. Holds external metadata about the save.
    /// </summary>
    private class MetaDataWrapper {
        public T Data;
        public int Version;

        public MetaDataWrapper(T data, int version) {
            Data = data;
            Version = version;
        }
    }

    public void NewData(string filename, T obj) {
        string savefile = Path.Combine(SAVE_PATH, $"{filename}.json");
        if (!Directory.Exists(SAVE_PATH))
            Directory.CreateDirectory(SAVE_PATH);
        
        File.Create(savefile).Dispose();
        File.WriteAllText(savefile, JsonUtility.ToJson(new MetaDataWrapper(obj, SAVE_VERSION)));
        Debug.Log($"[DataPersistence] Created new data to {savefile}. (Save Version = {SAVE_VERSION})");
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
            Debug.LogWarning($"[DataPeristence] Error loading save file. {savefile} does not exist! (Save Version = {SAVE_VERSION})");
            return RET_SAVE_FAIL;
        }

        File.WriteAllText(savefile, JsonUtility.ToJson(new MetaDataWrapper(obj, SAVE_VERSION)));
        Debug.Log($"[DataPersistence] Successfully saved data to {savefile}. (Save Version = {SAVE_VERSION})");
        return RET_SAVE_SUCCESS;
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
            Debug.LogError($"[DataPeristence] Error loading save file. {savefile} does not exist! (Save Version = {SAVE_VERSION})");
            return RET_SAVE_FAIL;
        }

        try {  
            MetaDataWrapper dataWrapper = JsonUtility.FromJson<MetaDataWrapper>(File.ReadAllText(savefile));

            if (dataWrapper.Version != SAVE_VERSION) {
                Debug.LogWarning($"[DataPersistence] SaveData in {savefile} is of Save Version {dataWrapper.Version} and does not match the current save version! (Save Version = {SAVE_VERSION})");
                return RET_SAVE_WRONG_VERSION;
            }

            obj = dataWrapper.Data;
            Debug.Log($"[DataPersistence] Successfully loaded data from {savefile}. (Save Version = {SAVE_VERSION})");
            return RET_SAVE_SUCCESS;
        } catch (FormatException) {
            Debug.LogError($"[DataPersistence] Error loading save file. {savefile} is not of type {typeof(T)}! (Save Version = {SAVE_VERSION})");
            return RET_SAVE_FAIL;
        }
    }
}