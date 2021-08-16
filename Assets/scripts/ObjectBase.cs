using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectBase : MonoBehaviour
{
    private GameObject m_container;
    public float m_cardDecal;
    private GameMaster m_gameMaster;

    public virtual void Awake()
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
    }

    public GameMaster GetGameMaster()
    {
        if(m_gameMaster == null)
        {
            GameObject go = GameObject.Find("GameMaster");
            if (go)
            {
                m_gameMaster = go.GetComponent<GameMaster>();
            }
        }
        return m_gameMaster;
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
            CardScript cs = m_container.transform.GetChild(count -1).GetComponent<CardScript>();
            return cs;
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
    public virtual bool GetHitable()
    {
        return true;
    }
    public virtual bool IsEmpty()
    {
        return GetNbChildCards() == 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void OutLine(bool b)
    {

    }
    public virtual Vector3 GetTargetPosition(ObjectBase c)
    {
        int i = GetNbChildCards() + 1;
        //Vector3 p = this.transform.position;
        Vector3 p = new Vector3(0,0,0);
        p.z -= i * GetGameMaster().m_cardSpace;
        p.y -= m_cardDecal * GetNbChildCards();
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

    public bool Restore(ObjectBase parent)
    {
        GameObject go = GameObject.Find(this.name);
        Transform me = go.transform;
        ObjectBase b = me.GetComponent<ObjectBase>();
        CardScript s = me.GetComponent<CardScript>();
        if(s != null)
        {
            s.transform.rotation = parent.transform.rotation;
            CardScript.Face f = this.recto ? CardScript.Face.recto : CardScript.Face.verso;
            s.MoveToParent(parent, f, 10, true);
            s.SetHitable(true);
            //s.RestoreTo(parent, f);
        }
        int l = children.Count;
        bool bOK = true;
        for (int i = 0; i < l; i++)
        {
            ObjectBaseSerialized o2 = children[i];
            o2.Restore(b);
        }
        return bOK;
    }
}