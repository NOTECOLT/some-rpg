using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils {
    /// <summary>
    /// Wrapper function for Debug.Log(), also outputs logs to Debugger Screen
    /// </summary>
    public static void Log(string str) {
        Debug.Log(str);
    }

    /// <summary>
    /// Wrapper function for Debug.LogWarning(), also outputs logs to Debugger Screen
    /// </summary>
    public static void LogWarning(string str) {
        Debug.LogWarning(str);
    }

    /// <summary>
    /// Wrapper function for Debug.LogError(), also outputs logs to Debugger Screen
    /// </summary>
    public static void LogError(string str) {
        Debug.LogError(str);
    } 
}
