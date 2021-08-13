using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tableau : ObjectBase
{
    public int m_nbHiddenCards;

    // Update is called once per frame
    void Update()
    {
        if (GetGameMaster().IsPaused())
        {
            return;
        }
    }

    public void FlipTopcard()
    {
        CardScript c = GetTopCard();
        if(c!=null)
        {
            c.flipTo(CardScript.Face.recto, false);
        }
    }
    
    public CardScript GetTopVisibleCard()
    {
        CardScript c = GetTopCard();
        if(c != null && c.IsRecto())
        {
            while (c.GetChild(0))
            {
                c = (CardScript)c.GetChild(0);
            }
            return c;
        }
        return null;
    }
 
    public bool IsFull()
    {
        return GetNbChildCards() == m_nbHiddenCards;
    }

    public override bool CanAdd(ObjectBase c)
    {
        int count = GetNbChildCards();
        CardScript cs = c.GetComponent<CardScript>();
        if(cs != null)
        {
            if(count < m_nbHiddenCards)
            {
                return true;
            }
            else if(count == m_nbHiddenCards)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    public int GetNbHiddenCards()
    {
        return m_nbHiddenCards;
    }
}
