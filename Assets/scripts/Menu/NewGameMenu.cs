using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewGameMenu : MonoBehaviour
{
    public MainMenu m_mainMenu;
    public Button m_start;
    public Button m_back;
    public SolitaireListView m_level;
    public SolitaireListView m_deckPack;
    public SolitaireListView m_difficulty;
    public SolitaireListItem m_listItem;

    void Awake()
    {
        m_start.onClick.AddListener(OnStart);
        m_back.onClick.AddListener(OnBack);
    }

    void Start()
    {
        m_deckPack.AddItem("Standart", true, true);
        m_difficulty.AddItem("1", true, true);
        m_difficulty.AddItem("3", true, true);
        m_level.AddItem("tree", true, true);
    }

    private void Update()
    {
    }

    void OnStart()
    {
        GetComponent<StateRecorder>().ClearSave();
        string l = m_level.GetValue();
        SceneManager.LoadScene("Solitaire");
    }

    void OnBack()
    {
        this.gameObject.SetActive(false);
        m_mainMenu.gameObject.SetActive(true);
    }
}
