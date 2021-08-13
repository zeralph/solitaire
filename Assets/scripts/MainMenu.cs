using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Button m_newGame;
    public Button m_Continue;
    public Button m_Scores;
    public Button m_Options;
    public Button m_Quit;

    // Start is called before the first frame update
    void Start()
    {
        m_newGame.onClick.AddListener(NewGame);
        m_Continue.onClick.AddListener(Continue);
        m_Scores.onClick.AddListener(OnScores);
        m_Options.onClick.AddListener(OnOptions);
        m_Quit.onClick.AddListener(Quit);

        m_Continue.gameObject.SetActive(GetComponent<StateRecorder>().HasSave());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void NewGame()
    {
        GetComponent<StateRecorder>().ClearSave();
        SceneManager.LoadScene("Solitaire");
    }
    private void Continue()
    {
        SceneManager.LoadScene("Solitaire");
    }
    private void Quit()
    {
        Application.Quit();
    }
    private void OnScores()
    {

    }
    private void OnOptions()
    {

    }
}