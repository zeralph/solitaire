using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    public enum eMoves
    {
        eNotSet=0,
        eImpossibleMove,
        eDrawnToDiscard, //draw
        eDiscardToDrawn, //refill
        eDiscardToTableau,
        eDiscardToFamily,
        eDiscardToCard,
        eTableauToTableau,
        eTableauToFamily,
        eTableauToCard,
        eFamilyToCard,
        eCardToCard,
        eCardToTableau,
        eCardToFamily,
        eFamilyToFamily,
    }
    public int[] SCORE =
    {
        0,//eNotSet=0,
        0,//eImpossibleMove,
        0,//eDrawnToDiscard,
        -50,//eDiscardToDrawn,
        10,//eDiscardToTableau,
        10,//eDiscardToFamily,
        10,//eDiscardToCard,
        0,//eTableauToTableau,
        10,//eTableauToFamily,
        0,//eTableauToCard,
        -20,//eFamilyToCard,
        0,//eCardToCard,
        0,//eCardToTableau,
        10,//eCardToFamily,
        0,//eFamilyToFamily,
    };
    public WorldMoveScript m_world; 
    public ObjectBase m_board;
    public DeckScript m_StartDeck;
    public DeckScript m_deck;
    public DeckScript m_discardPile;
    public OptionsMenu m_optionsMenu;
    public IngameMenu m_inGameMenu;
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
    public float m_boomFactor = 100.0f;
    public TexturePack m_texturePack;

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
    private bool m_doLoadSave;
    private int m_score;
    private int m_turn;

    public void OnMovePlayed(eMoves move)
    {
        Debug.Log(move.ToString());
        m_score += SCORE[(int)move];
        m_turn ++;
        SaveState(true);
        m_canAutomate = Automate(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        m_tableauIndex = 0;
        m_score = 0;
        m_turn = 0;
        m_refillDrawn = false;
        m_timer = 0;
        m_win = false;
        m_canAutomate = false;
        m_nbPickedCards = 0;
        m_pickfromDrawn = false;
        m_optionsMenu.gameObject.SetActive(false);
        m_paused = false;
        m_optionsMenu.gameObject.SetActive(false);
        m_optionsMenu.GetComponent<OptionsMenu>().ReloadSettings();
        m_world.Reset();
        m_world.StartMove();
        m_inGameMenu.gameObject.SetActive(true);
        m_doLoadSave = false;
        if (HasSave())
        {
            m_doLoadSave = true;
        }
        else
        {
            NewGame();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(m_doLoadSave)
        {
            m_doLoadSave = false;
            LoadFromSave();
        }
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
                    c.MoveToParent(m_deck, CardScript.Face.verso, m_drawToDeckRefillSpeed);
                }
                else
                {
                    m_refillDrawn = false;
                    OnMovePlayed(eMoves.eDiscardToDrawn);
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
                    c.MoveToParent(m_discardPile, CardScript.Face.recto, m_deckToDrawnSpeed);
                    m_nbPickedCards++;
                }
                else
                {
                    m_pickfromDrawn = false;
                    m_nbPickedCards = 0;
                    OnMovePlayed(eMoves.eDrawnToDiscard);
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

    public void SaveState(bool bSave)
    {
        //List<CardScript> cl = GetComponent<CardsCreator>().GetCards();
        GetComponent<StateRecorder>().AddState(m_board, bSave);
    }

    public ObjectBase Find(string name)
    {
        if(name == "Drawn ")
        {
            return m_deck;
        }
        if (name == "Discard ")
        {
            return m_discardPile;
        }
        for (int i = 0; i < m_tableaux.Count; i++)
        {
            if (m_tableaux[i].name == name)
            {
                return m_tableaux[i];
            }
        }
        for(int i=0; i< m_familyPiles.Count; i++)
        {
            if(m_familyPiles[i].name == name)
            {
                return m_familyPiles[i];
            }
        }
        List<CardScript> cl = GetComponent<CardsCreator>().GetCards();
        return cl.Find(card => (card.name == name));
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
            c.MoveToParent(tab, f, m_distributionSpeed);
            m_tableauIndex++;
            return true;
        }
        else
        {
            c.MoveToParent(m_deck, CardScript.Face.verso, m_distributionSpeed);
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
        c.MoveToParent(m_discardPile, CardScript.Face.recto, m_speed);
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
        m_score = 0;
        m_turn = 0;
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
                        topDrawn.MoveToParent(m_familyPiles[i], CardScript.Face.recto, m_speed);
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
                            topTableauCard.MoveToParent(m_familyPiles[j], CardScript.Face.recto, m_speed);
                        }
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public void PutCardsInStartDeck()
    {
        List<CardScript> cards = GetComponent<CardsCreator>().GetCards();
        for(int i=0; i<cards.Count; i++)
        {
            m_StartDeck.Add(cards[i]);
            //cards[i].RestoreTo(m_StartDeck, cards[i].m_face);
        }
    }

    #region score

    public int GetScrore()
    {
        return m_score;
    }
    public int GetTurn()
    {
        return m_turn;
    }
    #endregion
    #region menu controls
    public void NewGame()
    {
        CardsCreator cc = this.GetComponent<CardsCreator>();
        if(cc.CardCreated())
        {
            cc.DistributeCards();
        }
    }

    public void OpenOptionsMenu()
    {
        m_optionsMenu.gameObject.SetActive(true);
        m_inGameMenu.gameObject.SetActive(false);
        m_paused = true;
    }

    public void CloseOptionsMenu()
    {
        m_optionsMenu.gameObject.SetActive(false);
        m_inGameMenu.gameObject.SetActive(true);
        m_paused = false;
    }

    public void Automate()
    {
        m_doAutomate = true;
    }

    public bool CanAutomate()
    {
        return m_canAutomate;
    }

    public bool CanUndo()
    {
        return GetComponent<StateRecorder>().CanUndo();
    }

    public bool CanRedo()
    {
        return GetComponent<StateRecorder>().CanRedo();
    }

    public void Undo()
    {
        GetComponent<StateRecorder>().LoadPreviousState(m_board);
    }

    public void Redo()
    {
        GetComponent<StateRecorder>().LoadNextState(m_board);
    }
    #endregion

    #region save

    public void LoadFromSave()
    {
        GetComponent<StateRecorder>().LoadFromSave(m_board);
    }

    public bool HasSave()
    {
        return GetComponent<StateRecorder>().HasSave();
    }
    #endregion

}