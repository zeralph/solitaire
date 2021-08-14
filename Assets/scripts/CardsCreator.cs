using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardsCreator : ObjectBase
{
    public GameObject m_cardPrefab;
    public DeckScript m_deck;
    public TexturePack m_texturePack;
    public float m_distributionDeltaTime = 0.05f;
    private List<CardScript> m_cards;
    private int m_cardToDistributeIndex;
    private bool m_distributing;
    private bool m_cardsCreated;
    private float m_timer;
    
    // Start is called before the first frame update

    public override void Start()
    {
        Debug.LogWarning("START");
        base.Start();
        m_cardsCreated = false;
        m_cardToDistributeIndex = 0;
        m_distributing = false;
        m_timer = 0;
    }

    public bool CardCreated()
    {
        return m_cardsCreated;
    }

    // Update is called once per frame
    void Update()
    {
        if(m_distributing)
        {
            m_timer += Time.deltaTime;
            if (m_timer > m_distributionDeltaTime)
            {
                m_timer = 0;
                DistributeCard(m_cards[m_cardToDistributeIndex]);
                //Debug.Log("ditributing card " + m_cardToDistributeIndex);
                if(m_cardToDistributeIndex == m_cards.Count)
                {
                    m_distributing = false;
                    GetGameMaster().SaveState(true);
                    GetGameMaster().OnMovePlayed(GameMaster.eMoves.eDistributed);
                }
            }
        }
    }

    public void BoomCards()
    {
        for(int i=0; i<m_cards.Count; i++)
        {
            m_cards[i].Boom();
        }
    }

    public void DistributeCards()
    {
        //shuffle
        for (int i = 0; i < m_cards.Count; i++)
        {
            int r = (int)Random.Range(i, m_cards.Count);
            CardScript s = m_cards[i];
            m_cards[i] = m_cards[r];
            m_cards[r] = s;
        }  
        for (int i=0; i<m_cards.Count; i++)
        {
            m_cards[i].DisablePhysic();
            float mass = Random.Range(1f, 5f);
            m_cards[i].GetComponent<Rigidbody>().mass = mass;
            if(m_deck)
            {
                m_cards[i].transform.rotation = m_deck.transform.rotation;
                m_cards[i].transform.position = m_deck.transform.position;
                m_deck.Add(m_cards[i]);
            }
            m_cards[i].flipTo(CardScript.Face.verso, false);
        }
        //distribute
        this.GetComponent<GameMaster>().Clear();
        m_cardToDistributeIndex = 0;
        m_timer = 0;
        m_distributing = true;
    }

    public List<CardScript> GetCards()
    {
        return m_cards;
    }

    private void DistributeCard(CardScript c)
    {
        //Add to the tableaux
        GetComponent<StateRecorder>().Clear();
        bool bAdded = this.GetComponent<GameMaster>().DistributeCard(c);
        m_cardToDistributeIndex++;
    }
    private CardScript CreateCard(CardScript.Symbol s, CardScript.Name n, bool flip, TexturePack tp)
    {
        Vector3 v = new Vector3(0,0,0);
        if(m_deck != null)
        {
            v = m_deck.transform.position;
        }
        GameObject o = Instantiate(m_cardPrefab, v, Quaternion.identity);
        o.name = s.ToString() + "_" + n.ToString();
        CardScript card = o.GetComponent<CardScript>();
        card.Set(s, n, tp);
        //card.flipTo(CardScript.Face.verso, false);
        return card;
    }

    public void CreateCards()
    {
        if(!m_cardsCreated)
        {
            Debug.Log("CREATE CARDS");   
            m_cards = new List<CardScript>();
            for (CardScript.Symbol i = CardScript.Symbol.spade; i< CardScript.Symbol.__MAX__; i++)
            {
                for(CardScript.Name j = CardScript.Name.ace; j< CardScript.Name.__MAX__; j++)
                {
                    CardScript card = CreateCard(i, j, true, m_texturePack);
                    m_cards.Add(card);
                    if(m_deck != null)
                    {
                        m_deck.Add(card);
                        card.transform.position = m_deck.transform.position;
                    }
                }
            }
        }
        m_cardsCreated = true;
    }    
}
