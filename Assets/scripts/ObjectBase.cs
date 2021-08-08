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
        Transform t = this.transform.Find("Container");
        if(t == null)
        {
            m_container = new GameObject("Container");
            m_container.transform.parent = this.transform;
        }
        else
        {
            m_container = t.gameObject;
        }
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

    public bool IsCard()
    {
        return this.GetComponent<CardScript>() != null;
    }
    public bool IsDeck()
    {
        return this.GetComponent<DeckScript>() != null;
    }
    public bool IsDrawn()
    {
        return this.GetComponent<DeckScript>() != null && this.GetComponent<DeckScript>().name == "Drawn";
    }
    public bool IsDiscard()
    {
        return this.GetComponent<DeckScript>() != null && this.GetComponent<DeckScript>().name == "Discard";
    }
    public bool Istableau()
    {
        return this.GetComponent<Tableau>() != null;
    }
    public bool IsFamily()
    {
        return this.GetComponent<FamilyPile>() != null;
    }
}

[System.Serializable]
public class ObjectBaseSerialized
{
    public string name;
    public bool recto;
    public List<ObjectBaseSerialized> children;
    //public CardScript.Face m_face;
    public ObjectBaseSerialized(ObjectBase o)
    {
        name = o.name;
        CardScript cs = o.GetComponent<CardScript>();
        if(cs!=null)
        {   
            recto = cs.IsRecto();
        }
        else
        {
            recto = false;
        }
        
        children = new List<ObjectBaseSerialized>();
        int l = o.GetNbChildCards();
        for(int i=0; i<l; i++)
        {
            ObjectBase o2 = o.GetChild(i);
            ObjectBaseSerialized os = new ObjectBaseSerialized(o2);
            children.Add(os);
        }
    }

    public void Restore(ObjectBase parent)
    {
        GameObject go = GameObject.Find(this.name);
        Transform me = go.transform;
        ObjectBase b = me.GetComponent<ObjectBase>();
        CardScript s = me.GetComponent<CardScript>();
        if(s != null)
        {
            CardScript.Face f = this.recto ? CardScript.Face.recto : CardScript.Face.verso;
            s.MoveToParent(parent, f, 10);
            //s.RestoreTo(parent, f);
        }
        int l = children.Count;
        for (int i = 0; i < l; i++)
        {
            ObjectBaseSerialized o2 = children[i];
            o2.Restore(b);
        }
    }
}