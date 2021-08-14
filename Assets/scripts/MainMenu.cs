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
    public WorldMoveScript m_world;
    private List<CardScript> m_cards;
    private Vector3 m_lastCamPosition;
    public float m_cardSpawnHorizontalRadius = 3;
    // Start is called before the first frame update
    void Start()
    {
        m_newGame.onClick.AddListener(NewGame);
        m_Continue.onClick.AddListener(Continue);
        m_Scores.onClick.AddListener(OnScores);
        m_Options.onClick.AddListener(OnOptions);
        m_Quit.onClick.AddListener(Quit);
        m_Continue.gameObject.SetActive(GetComponent<StateRecorder>().HasSave());
        m_cards = null;
        m_lastCamPosition = Camera.main.transform.position;
        m_world.StartMove();

        //Physics.gravity
    }

    // Update is called once per frame
    void Update()
    {
        CardsCreator cc = GetComponent<CardsCreator>();
        if(!cc.CardCreated())
        {
            cc.CreateCards();
            m_cards = cc.GetCards();
            for (int i = 0; i < m_cards.Count; i++)
            {
                InitializeCard( m_cards[i]);
            }
        }
        if(m_cards != null)
        {
            for (int i = 0; i < m_cards.Count; i++)
            {
                if(m_cards[i].transform.position.y < Camera.main.transform.position.y)
                {
                    if (!m_cards[i].IsInFrustrum())
                    {
                        InitializeCard(m_cards[i]);
                    }
                }  
            }
        }
    }

    private void InitializeCard(CardScript c)
    {
        c.DisablePhysic();
        float x = Random.Range(-m_cardSpawnHorizontalRadius, m_cardSpawnHorizontalRadius);
        float y = Random.Range(-0f, 5f);
        float z = Random.Range(-m_cardSpawnHorizontalRadius, m_cardSpawnHorizontalRadius);
        Vector3 v = new Vector3(x, y, z);
        c.transform.position = c.GetParent().transform.position + v;
        x = Random.Range(-180.0f, 180.0f);
        y = Random.Range(-180.0f, 180.0f);
        z = Random.Range(-180.0f, 180.0f);
        Quaternion r = Quaternion.Euler(x, y, z);
        c.transform.rotation = r;
        x = Random.Range(-1f, 1f);
        y = Random.Range(-50f, -20f);
        z = Random.Range(-1f, 1f);
        Vector3 f = new Vector3(x, y, z);
        x = Random.Range(-1f, 1f);
        y = Random.Range(-1f, 1f);
        z = Random.Range(-1f, 1f);
        Vector3 t = new Vector3(x, y, z);
        c.EnablePhysic(false);
        c.AddForceAndTorque(f, t);    
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
