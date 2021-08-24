using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public bool m_visible;

    public GameMaster m_gameMaster;
    public Button m_back;

    public SolitaireListView m_resolutionListView;
    public SolitaireListView m_qualityListView;
    public SolitaireListView m_framerateListView;

    // Start is called before the first frame update
    void Awake()
    {
        //SetButtonText("ButtonFramerate", m_framerate.ToString());
        //GetButton("ButtonFramerate").onClick.AddListener(OnClickFramerate);
        //SetButtonText("ButtonResolution", m_resolution.ToString() );
        //GetButton("ButtonResolution").onClick.AddListener(OnClickResolution);
        //SetButtonText("ButtonQuality", m_quality.ToString());
        //GetButton("ButtonQuality").onClick.AddListener(OnClickQuality);
        //m_back.onClick.AddListener(m_gameMaster.CloseOptionsMenu);
    }

    private void Start()
    {

        m_resolutionListView.AddOnChangeFunction(OnChangeResolution);
        m_qualityListView.AddOnChangeFunction(OnChangeQuality);
        m_framerateListView.AddOnChangeFunction(OnChangeFramerate);
        LoadPrefs();
    }

    public void BindCloseButton(UnityAction closeFunction)
    {
        if (closeFunction is null)
        {
            throw new ArgumentNullException(nameof(closeFunction));
        }
        m_back.onClick.AddListener(closeFunction);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void DeleteAllSave()
    {
        PlayerPrefs.DeleteAll();
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

    private void OnGUI()
    {
    }

    public void SavePrefs()
    {
        PlayerPrefs.SetString("Quality", m_qualityListView.GetValue());
        PlayerPrefs.SetString("Resolution", m_resolutionListView.GetValue());
        PlayerPrefs.SetString("Framerate", m_framerateListView.GetValue());
        PlayerPrefs.Save();
    }

    public void LoadPrefs()
    {
        string quality = PlayerPrefs.GetString("Quality"); 
        string resolution = PlayerPrefs.GetString("Resolution");
        string framerate = PlayerPrefs.GetString("Framerate");
        if(string.IsNullOrEmpty(quality))
        {
            quality = "High";
        }
        if(string.IsNullOrEmpty("Resolution"))
        {
            resolution = "High";
        }
        if (string.IsNullOrEmpty("Framerate"))
        {
            framerate = "High";
        }
        m_qualityListView.SetValue(quality);
        m_resolutionListView.SetValue(resolution);
        m_framerateListView.SetValue(framerate);
    }


    //------------
    void OnChangeResolution(string s)
    {
        PlayerPrefs.SetString("Resolution", s);
        SavePrefs();
        GetComponent<StateRecorder>().LoadQualitySettings();
        LoadPrefs();
    }

    void OnChangeQuality(string s)
    {
        PlayerPrefs.SetString("Quality", s);
        SavePrefs();
        GetComponent<StateRecorder>().LoadQualitySettings();
        LoadPrefs();
    }

    void OnChangeFramerate(string s)
    {
        PlayerPrefs.SetString("Framerate", s);
        SavePrefs();
        GetComponent<StateRecorder>().LoadQualitySettings();
        LoadPrefs();
    }
}
