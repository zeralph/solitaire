

using UnityEngine;
using System.Collections;

public class ScreenLog : MonoBehaviour
{
    string m_logString;
    Queue m_logQueue = new Queue();
    public bool m_enabled = false;
    void Start()
    {
        Debug.Log("Log1");
        Debug.Log("Log2");
        Debug.Log("Log3");
        Debug.Log("Log4");
    }

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if(m_enabled)
        {
            m_logString = logString;
            string newString = "\n [" + type + "] : " + m_logString;
            m_logQueue.Enqueue(newString);
            if (type == LogType.Exception)
            {
                newString = "\n" + stackTrace;
                m_logQueue.Enqueue(newString);
            }
            m_logString = string.Empty;
            foreach (string l in m_logQueue)
            {
                m_logString += l;
            }
        }
    }

    void OnGUI()
    {
        GUILayout.Label(m_logString);
    }
}

