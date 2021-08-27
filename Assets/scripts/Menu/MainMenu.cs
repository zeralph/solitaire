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
    public NewGameMenu m_newGameMenu;
    public OptionsMenu m_optionsMenu;

    void Start()
    {
        m_newGameMenu.gameObject.SetActive(false);
        m_optionsMenu.gameObject.SetActive(false);
        m_newGame.onClick.AddListener(NewGame);
        m_Continue.onClick.AddListener(Continue);
        m_Scores.onClick.AddListener(OnScores);
        m_Options.onClick.AddListener(OnOptions);
        m_Quit.onClick.AddListener(Quit);
        m_Continue.gameObject.SetActive(GetComponent<StateRecorder>().HasSave());
        m_optionsMenu.BindCloseButton(OnCloseOptions);
    }
    private void NewGame()
    {
        this.gameObject.SetActive(false);
        m_newGameMenu.gameObject.SetActive(true);
    }
    private void Continue()
    {
        //SceneManager.LoadScene("Solitaire");
        SceneLoader sl = FindObjectsOfType<SceneLoader>()[0];
        sl.LoadScene(true, false, false);
    }
    private void Quit()
    {
        Application.Quit();
    }
    private void OnScores()
    {
        //SceneManager.LoadScene("ScoresMenu");
        SceneLoader sl = FindObjectsOfType<SceneLoader>()[0];
        sl.LoadScene(false, false, true);
    }
    public void OnCloseOptions()
    {
        this.gameObject.SetActive(true);
        m_optionsMenu.gameObject.SetActive(false);
    }
    private void OnOptions()
    {
        this.gameObject.SetActive(false);
        m_optionsMenu.gameObject.SetActive(true);
    }
}