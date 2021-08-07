using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    public DeckScript m_StartDeck;
    public DeckScript m_deck;
    public Transform m_cardsStartPoint;
    public DeckScript m_discardPile;
    public Transform m_optionsMenu;
    public float m_zWhenMoving = -0.2f;
    public float m_cardSpace = 0.005f;
    public int m_nbDrawnCardsFromDeck = 3;
    public float m_speed = 3f;
    public float m_distributionSpeed = 10f;
    public float m_deckToDrawnSpeed = 2.5f;
    public float m_drawToDeckRefillSpeed = 20f;
    public float m_mouseSpeed = 50.0f;
    public float m_moveBackSpeed = 10f;
    public float m_automationSpeedDt = 0.2f;
    public float m_pickcardsFromdeckSpeedDt = 0.3f;
    public float m_refillSpeedDt = 0.1f;
    
    
    [SerializeField]
    public List<Tableau> m_tableaux;
    [SerializeField]
    public List<FamilyPile> m_familyPiles;
    private int m_tableauIndex;
    private float m_timer;
    private bool m_refillDrawn;
    private bool m_win;
    private bool m_canAutomate;
    private bool m_doAutomate;
    private bool m_pickfromDrawn;
    private int m_nbPickedCards = 0;
    private bool m_paused = false;
    // Start is called before the first frame update
    void Start()
    {
        m_tableauIndex = 0;
        m_refillDrawn = false;
        m_timer = 0;
        m_win = false;
        m_canAutomate = false;
        m_nbPickedCards = 0;
        m_pickfromDrawn = false;
        m_optionsMenu.gameObject.SetActive(false);
        m_paused = false;
        m_optionsMenu.GetComponent<OptionsMenu>().ReloadSettings();
    }

    // Update is called once per frame
    void Update()
    {
        if(IsPaused())
        {
            return;
        }
        m_timer += Time.deltaTime;
        if (m_refillDrawn)
        {          
            if (m_refillDrawn && m_timer > m_refillSpeedDt)
            {
                m_timer = 0;
                CardScript c = m_discardPile.GetTopCard();
                if(c)
                {
                    c.MoveToParent(m_deck, CardScript.Face.verso, false, c.GetCardDecal(), m_drawToDeckRefillSpeed);
                }
                else
                {
                    m_refillDrawn = false;
                }
            }
        }
        if (m_pickfromDrawn)
        {
            if (m_timer > m_pickcardsFromdeckSpeedDt)
            {
                m_timer = 0;
                if(m_deck.GetNbChildCards() > 0 && m_nbPickedCards < m_nbDrawnCardsFromDeck)
                {
                    CardScript c = m_deck.GetTopCard();
                    c.MoveToParent(m_discardPile, CardScript.Face.recto, false, m_discardPile.GetCardDecal(), m_deckToDrawnSpeed);
                    m_nbPickedCards++;
                }
                else
                {
                    m_pickfromDrawn = false;
                    m_nbPickedCards = 0;
                }
            }
        }
        if(m_doAutomate)
        {
            if(m_timer > m_automationSpeedDt)
            {
                m_doAutomate = Automate(true);
                m_timer = 0;
            }    
        }
        else
        {
            m_canAutomate = Automate(false);
        }
        if (!m_win)
        {
            ChekForWin();
        }
    }

    private void ChekForWin()
    {
        if (!m_win)
        {

            m_win = true;
            for (int i = 0; i < m_familyPiles.Count; i++)
            {
                m_win &= m_familyPiles[i].IsComplete();
            }
            if(m_win)
            {
                GetComponent<CardsCreator>().BoomCards();
            }
        }
    }

    public void SaveState()
    {
        List<CardScript> cl = GetComponent<CardsCreator>().GetCards();
        GetComponent<StateRecorder>().SaveState(cl);
    }

    public void OnClickOnDrawn()
    {
        if(m_deck.IsCardsMoving() || m_discardPile.IsCardsMoving() )
        {
            return;
        }
        if(m_deck.IsEmpty())
        {
            m_refillDrawn = true;
        }
        else
        {
            m_pickfromDrawn = true;
            m_nbPickedCards = 0;
        }
    }

    public bool IsPaused()
    {
        return m_paused;
    }

    public bool DistributeCard(CardScript c)
    {
        GetComponent<StateRecorder>().Clear();
        if (m_tableauIndex >= m_tableaux.Count)
        {
            m_tableauIndex = 0;
        }
        while(m_tableauIndex < m_tableaux.Count && !(m_tableaux[m_tableauIndex].CanAdd(c)))
        {
            m_tableauIndex++;
        }
        if(m_tableauIndex < m_tableaux.Count)
        {
            Tableau tab = m_tableaux[m_tableauIndex];
            CardScript.Face f = CardScript.Face.verso;
            if (tab.IsFull())
            {
                f = CardScript.Face.recto;
            }
            c.MoveToParent(tab, f, true, tab.GetCardDecal(), m_distributionSpeed);
            m_tableauIndex++;
            return true;
        }
        else
        {
            c.MoveToParent(m_deck, CardScript.Face.verso, true, 0, m_distributionSpeed);
            m_tableauIndex = 0;
            return false;
        }
    }

    public void Pause(bool pause)
    {
        m_paused = pause;
    }
    public void DrawFromDeck()
    {
        CardScript c = m_deck.GetTopCard();
        c.MoveToParent(m_discardPile, CardScript.Face.recto, true, m_discardPile.GetCardDecal(), m_speed);
    }

    public void Clear()
    {
        for(int i=0; i<m_tableaux.Count; i++)
        {
            m_tableaux[i].Clear();
        }
        for(int i=0; i< m_familyPiles.Count; i++)
        {
            m_familyPiles[i].Clear();
        }
        m_tableauIndex = 0;
        m_deck.Clear();
    }

    private bool Automate(bool bDo)
    {
        CardScript topDrawn = m_discardPile.GetTopCard();
        //check for discard top card
        if(topDrawn != null)
        {
            for(int i=0; i<m_familyPiles.Count; i++)
            {
                if(m_familyPiles[i].CanAdd(topDrawn))
                {
                    if(bDo)
                    {
                        topDrawn.MoveToParent(m_familyPiles[i], CardScript.Face.recto, false, m_familyPiles[i].GetCardDecal(), m_speed);
                    }
                    return true;
                }
            }
        }
        // check for tableau top cards
        for (int i = 0; i < m_tableaux.Count; i++)
        {
            //CardScript topTableauCard = m_tableaux[i].GetTopVisibleCard();
            CardScript topTableauCard = m_tableaux[i].GetTopVisibleCard();
            if (topTableauCard != null)
            {
                for (int j = 0; j < m_familyPiles.Count; j++)
                {
                    if (m_familyPiles[j].CanAdd(topTableauCard))
                    {
                        if (bDo)
                        {
                            topTableauCard.MoveToParent(m_familyPiles[j], CardScript.Face.recto, false, m_familyPiles[j].GetCardDecal(), m_speed);
                        }
                        return true;
                    }
                }
            }
        }
        return false;
    }

    void OnGUI()
    {
        CardsCreator cc = this.GetComponent<CardsCreator>();
        float w = Screen.width;
        float h = Screen.height;
        float btnW = w / 6;
        float btnH = h / 10;
        float wStep = w / 20 + btnW;
        if (cc.CardCreated() && GUI.Button(new Rect(0, h-btnH, btnW, btnH), "SHUFFLE"))
        {
            cc.DistributeCards(m_cardsStartPoint.transform.position);
            //Shuffle();
        }
        if(GUI.Button(new Rect(wStep, h-btnH, btnW, btnH), "QUIT") || Input.GetKey("escape"))
        {
            Application.Quit();
        }
        if (GUI.Button(new Rect(wStep*2, h - btnH, btnW, btnH), "BOOM"))
        {
            GetComponent<CardsCreator>().BoomCards();
        }
        if (GUI.Button(new Rect(wStep * 3, h - btnH, btnW, btnH), "OPTIONS"))
        {
            if(!m_paused)
            {
                m_optionsMenu.gameObject.SetActive(true);
                m_paused = true;
            }
            else
            {
                m_optionsMenu.gameObject.SetActive(false);
                m_paused = false;
            }
        }
        if (m_canAutomate && GUI.Button(new Rect(wStep * 4, h - btnH, btnW, btnH), "AUTOMATE"))
        {
            m_doAutomate = true;
        }
        if (m_win)
        {
            GUI.Label(new Rect(130, 10, 50, 20), "YOU WIN");
        }
        /*
        string[] names = QualitySettings.names;
        GUILayout.BeginVertical();
        for (int i = 0; i < names.Length; i++)
        {
            if (GUILayout.Button(names[i]))
            {
                QualitySettings.SetQualityLevel(i, true);
            }
        }
        GUILayout.EndVertical();
        */
    }
}
