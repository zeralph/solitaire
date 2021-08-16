using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tableau : ObjectBase
{
    public int m_nbHiddenCards;

    public override void Awake()
    {
        base.Awake();
    }

    void Update()
    {
        if (GetGameMaster().IsPaused())
        {
            return;
        }
        GetComponentInChildren<TextMesh>().text = GetNbHiddenCards().ToString();
    }

    public int GetNbHiddenCards()
    {
        return Mathf.Max(0, GetNbChildCards() - 1);
    }

    public void FlipTopcard()
    {
        CardScript c = GetTopCard();
        if(c!=null && !c.IsRecto())
        {
            c.flipTo(CardScript.Face.recto, false);
 //           m_currentNbHiddenCards--;
        }
    }

    public CardScript GetAntHiddenCard()
    {
        if(GetNbHiddenCards() > 0)
        {
            int i = Random.Range(0, GetNbHiddenCards());
            ObjectBase o = GetChild(i);
            CardScript cs = o.GetComponent<CardScript>();
            if (cs && !cs.IsRecto())
            {
                return cs;
            }
        }
        return null;
    }

    public override void Clear()
    {
        base.Clear();
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

}
