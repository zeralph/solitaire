using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public enum QUALITY  
    { 
        Low=0, 
        Medium=1, 
        High=2
    };
    public enum RESOLUTION
    { 
        High = 1,
        medium=2,
        Low =4,
    };
    public enum FRAMERATE 
    { 
        Low=30, 
        High=60
    };

    public bool m_visible;

    public QUALITY m_quality;
    public RESOLUTION m_resolution;
    public FRAMERATE m_framerate;

    // Start is called before the first frame update
    void Awake()
    {
        PlayerPrefs.DeleteAll();
        ReloadSettings();
        SetButtonText("ButtonResolution", m_resolution.ToString() );
        GetButton("ButtonResolution").onClick.AddListener(OnClickResolution);
        SetButtonText("ButtonQuality", m_quality.ToString());
        GetButton("ButtonQuality").onClick.AddListener(OnClickQuality);
    }

    private void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    private Button GetButton(string btnName)
    {
        Canvas c = this.transform.GetComponent<Canvas>();
        Transform t = this.transform.Find(btnName);
        if (t != null)
        {
            Button b = t.GetComponent<Button>();
            return b;
        }
        return null;
    }

    private void SetButtonText(string btnName, string text)
    {
        Button b = GetButton(btnName);
        if (b != null)
        {
            Text txt = b.GetComponentInChildren<Text>();
            if (txt != null)
            {
                txt.text = text;
            }
        }
    }

    public void ReloadSettings()
    {
        LoadPrefs();

    }

    private void OnGUI()
    {
    }

    public void SavePrefs()
    {
        PlayerPrefs.SetInt("Quality", (int)m_quality);
        PlayerPrefs.SetInt("Resolution", (int)m_resolution);
        PlayerPrefs.SetInt("Framerate", (int)m_framerate);
        PlayerPrefs.Save();
    }

    public void LoadPrefs()
    {   
        m_quality = PlayerPrefs.HasKey("Quality")?(QUALITY)PlayerPrefs.GetInt("Quality"): QUALITY.Medium;
        m_resolution = PlayerPrefs.HasKey("Resolution") ? (RESOLUTION)PlayerPrefs.GetInt("Resolution") : RESOLUTION.Low;
        m_framerate = PlayerPrefs.HasKey("Framerate") ? (FRAMERATE)PlayerPrefs.GetInt("Framerate") : FRAMERATE.Low;

        QualitySettings.SetQualityLevel((int)m_quality, true);
        float h = (float)Display.main.systemHeight / (float)m_resolution; 
        float w = (float)Display.main.systemWidth / (float)m_resolution;
        Screen.SetResolution((int)w, (int)h, true, 30);
        //Application.targetFrameRate = (int)m_framerate;

       // QualitySettings.vSyncCount = 0;
    }


    //------------
    void OnClickResolution()
    {
        int n = Enum.GetValues(typeof(RESOLUTION)).Length;
        int i = Array.IndexOf(Enum.GetValues(typeof(RESOLUTION)), m_resolution);
        i++;
        if(i == n)
        {
            i = 0;
        }
        m_resolution = (RESOLUTION)Enum.GetValues(typeof(RESOLUTION)).GetValue(i);
        SetButtonText("ButtonResolution", m_resolution.ToString());
        SavePrefs();
        LoadPrefs();
    }

    void OnClickQuality()
    {
        int n = Enum.GetValues(typeof(QUALITY)).Length;
        int i = Array.IndexOf(Enum.GetValues(typeof(QUALITY)), m_quality);
        i++;
        if (i == n)
        {
            i = 0;
        }
        m_quality = (QUALITY)Enum.GetValues(typeof(QUALITY)).GetValue(i);
        SetButtonText("ButtonQuality", m_quality.ToString());
        SavePrefs();
        LoadPrefs();
    }

}
