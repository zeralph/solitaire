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
    // Start is called before the first frame update
    void Start()
    {
        m_start.onClick.AddListener(OnStart);
        m_back.onClick.AddListener(OnBack);
        for (int i=0; i < 6; i++)
        {
            GameObject g = Instantiate(m_listItem.gameObject, new Vector3(0, 0, 0), Quaternion.identity);
            SolitaireListItem l = g.GetComponent<SolitaireListItem>();
            l.SetText($"Item {i}");
            m_level.AddItem(l);
        }

    }

    void OnStart()
    {
        GetComponent<StateRecorder>().ClearSave();
        SceneManager.LoadScene("Solitaire");
    }

    void OnBack()
    {
        this.gameObject.SetActive(false);
        m_mainMenu.gameObject.SetActive(true);
    }
}
