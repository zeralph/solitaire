using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectBase : MonoBehaviour
{
    public GameObject m_container;
    public float m_cardDecal;
    protected GameMaster m_gameMaster;

    private void Awake()
    {
        m_container = new GameObject("Container");
        m_container.transform.parent = this.transform;
    }

    // Start is called before the first frame update
    public virtual void Start()
    {
        m_gameMaster = GameObject.Find("GameMaster").GetComponent<GameMaster>();
        if(m_gameMaster == null)
        {
            Debug.LogError("No gameMaster found");
        }
    }

    public virtual bool CanAdd(ObjectBase o)
    {
        return true;
    }

    public virtual bool Add(ObjectBase o)
    {
        o.transform.parent = m_container.transform;
        return true;
    }

    public virtual float GetCardDecal()
    {
        return m_cardDecal * GetNbChildCards();
    }

    public ObjectBase GetParent()
    {
        Transform t = transform.parent;
        if(t != null)
        {
            t = t.parent;
            if(t != null)
            {
                return t.GetComponent<ObjectBase>();
            }
        }
        return null;
    }

    public int GetNbChildCards()
    {
        return m_container.transform.childCount;
    }

    public virtual CardScript GetTopCard()
    {
        int count = m_container.transform.childCount;
        if (count > 0)
        {
            return m_container.transform.GetChild(count -1).GetComponent<CardScript>();
        }
        return null;
    }

    public virtual void Clear()
    {
        int count = m_container.transform.childCount;
        for(int i=0; i<count; i++)
        {
            ObjectBase o = GetChild(i);
            o.transform.parent = null;
        }
    }

    public ObjectBase GetChild(int i)
    {
        if (i < m_container.transform.childCount)
        {
            return m_container.transform.GetChild(i).GetComponent<ObjectBase>();
        }
        return null;
    }

    public virtual bool IsMoving()
    {
        return false;
    }

    public virtual bool IsEmpty()
    {
        return GetNbChildCards() == 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual Vector3 GetTargetPosition(ObjectBase c)
    {
        //int i = m_cards.IndexOf((CardScript)c);
        //if (i == -1)
        //{
        //    i = m_cards.Count;
        //}
        int i = GetNbChildCards() + 1;
        Vector3 p = this.transform.position;
        p.z -= i * m_gameMaster.m_cardSpace;
        return p;
    }
}
