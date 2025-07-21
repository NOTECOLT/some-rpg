using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ScreenLogger : MonoBehaviour {
    [SerializeField] private int _logSize = 15;
    private Queue<string> _displayLog = new Queue<string>();
    private Queue<string> _fullLog = new Queue<string>();

    private GUIStyle _style = null;
    private GUIStyle _buttonStyle = null;

    private string logsPath;

    void Awake() {
        logsPath = Application.persistentDataPath + "/logs";
        // Previously just OnEnable
        Application.logMessageReceived += HandleLog;
    }

    void OnDestroy() {
        // Previously just OnDisable
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type) {
        string log = $"[{type}]: {logString}";
        _displayLog.Enqueue(log);
        _fullLog.Enqueue($"[{DateTime.Now:HH:mm:ss}]{log}");

        if (type == LogType.Exception) {
            _displayLog.Enqueue(stackTrace);
            _fullLog.Enqueue(stackTrace);
        }

        while (_displayLog.Count > _logSize)
            _displayLog.Dequeue();
    }

    void OnGUI() {
        if (_style == null) {
            _style = new GUIStyle() {
                fontSize = 25,
                normal = {
                    textColor = Color.white
                },
                wordWrap = true
            };
        }

        if (_buttonStyle == null) {
            _buttonStyle = new GUIStyle(GUI.skin.button);
            _buttonStyle.fontSize = 25;
        }

        { // Title
            int startX = 20; int startY = 20;
            int endX = 220; int endY = 70;
            GUILayout.BeginArea(new Rect(startX, startY, endX, endY));
            GUILayout.Label("ScreenLogger", _style);
            GUILayout.EndArea();
        }

        { // Export Button
            int startX = 220; int startY = 20;
            int endX = 420; int endY = 70;
            GUILayout.BeginArea(new Rect(startX, startY, endX, endY));
            bool onExport = GUI.Button(new Rect(0, 0, endX-startX, endY-startY), "Export Log", _buttonStyle);
            if (onExport) OnExportLog();
            GUILayout.EndArea();
        }

        { // ScreenLogger
            int startX = 20; int startY = 75;
            int endX = Screen.width-startX; int endY = Screen.height-startY;     
            GUILayout.BeginArea(new Rect(startX, startY, endX, endY));
            GUILayout.Label("\n" + string.Join("\n", _displayLog.ToArray()), _style);
            GUILayout.EndArea();
        }

    }


    private void OnExportLog() {
        string logfile = Path.Combine(logsPath, $"log_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.txt");
        Debug.Log($"Exporting Log to {logfile}");

        if (!Directory.Exists(logsPath))
            Directory.CreateDirectory(logsPath);
        
        File.Create(logfile).Dispose();
        
        File.WriteAllText(logfile, $"Date Produced: {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}\n"
                            + $"Game Version: {Application.version}\n"
                            + $"Unity Version: {Application.unityVersion}\n"
                            + $"Device Info: {SystemInfo.deviceType}, {SystemInfo.deviceModel}\n"
                            + "Log:\n"
                            + string.Join("\n", _fullLog.ToArray()));
    }
}
