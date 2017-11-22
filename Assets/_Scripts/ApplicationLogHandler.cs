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

    private void Awake()
    {
        timeStamp = System.DateTime.UtcNow.ToLocalTime().ToString("HH:mm dd MMMM, yyyy");
        if (charlieSlackLog != null) Application.logMessageReceived += HandleLog;
    }

    //void OnEnable()
    //{
    //    timeStamp = System.DateTime.UtcNow.ToLocalTime().ToString("HH:mm dd MMMM, yyyy");
    //    if (charlieSlackLog != null) Application.logMessageReceived += HandleLog;
    //}

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        // avoid same logs in every frame
        if (logString != output)
        {
            output = logString;
            stack = stackTrace;
            string typeEmoji = "";

            if (type == LogType.Error)
            {
                logType = "Error";
                typeEmoji = ":rage:[Error]";
            }
            else if (type == LogType.Exception)
            {
                logType = "Exception";
                typeEmoji = ":question:[Exception]";
            }
            else if (type == LogType.Assert)
            {
                logType = "Assert";
                typeEmoji = ":question:[Assert]";
            }
            else if (type == LogType.Warning)
            {
                logType = "Warning";
                typeEmoji = ":warning:[Warning]";
            }
            else
            {
                logType = "Log";
                typeEmoji = ":package:[Log]";
            }

            //#if !UNITY_EDITOR
            charlieSlackLog.SlackApplicationLog("Unity_Console", output, typeEmoji, timeStamp);
            //#endif
        }
    }
}
