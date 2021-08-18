using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FamilyPile : ObjectBase
{
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        //m_cards = new List<CardScript>();
    }

    public override bool CanAdd(ObjectBase c)
    {
        int nbChildren = GetNbChildCards();
        CardScript cs = c.GetComponent<CardScript>();
        if (cs != null)
        {
            if(nbChildren == 0)
            {
                if (cs.m_figure == CardScript.Figure.ace)
                {
                    return true;
                }
            }
            else
            {
                CardScript cp = GetTopCard();
                if (cs.m_symbol == cp.m_symbol && cs.m_value == (cp.m_value + 1))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool IsComplete()
    {
        return GetNbChildCards() == 13;
        //return m_cards.Count == 13;
    }

    void Update()
    {
        if (GetGameMaster().IsPaused())
        {
            return;
        }
    }
}
