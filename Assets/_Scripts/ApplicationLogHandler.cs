using UnityEngine;
using System.Collections;
using Charlie;
using System;

public class ApplicationLogHandler : MonoBehaviour
{

    public CharlieSlackLog charlieSlackLog;

    // TODO [hide]-->private
    public string timeStamp = "";
    public string output = "";
    public string stack = "";
    public string logType = "";

    void OnEnable()
    {
        timeStamp = System.DateTime.UtcNow.ToLocalTime().ToString("HH:mm dd MMMM, yyyy");
        if (charlieSlackLog != null) Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        output = logString;
        stack = stackTrace;

        if (type == LogType.Error)
        {
            logType = "Error";
        }
        else if (type == LogType.Exception)
        {
            logType = "Exception";
        }
        else if (type == LogType.Assert)
        {
            logType = "Assert";
        }
        else if (type == LogType.Warning)
        {
            logType = "Warning";
        }
        else
        {
            logType = "Log";
        }

//#if !UNITY_EDITOR
        //charlieSlackLog.SlackApplicationLog("Unity_Console", stack + output, logType, timeStamp); // too many characters in stack
        charlieSlackLog.SlackApplicationLog("Unity_Console", output, type, logType, timeStamp);
//#endif
    }
}
