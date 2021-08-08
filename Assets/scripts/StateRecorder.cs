using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateRecorder : MonoBehaviour
{
    public List<string> m_states;
    private int m_curStateIndex;
    // Start is called before the first frame update
    void Start()
    {
        m_states = new List<string>();
        m_curStateIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool CanUndo()
    {
        return m_states.Count > 1;
    }

    public bool CanRedo()
    {
        return m_curStateIndex < m_states.Count-1;
    }

    public void Clear()
    {
        m_states.Clear();
        m_curStateIndex = 0;
        Debug.Log("states cleared");
    }

    public void AddState(ObjectBase o)
    {
        /*
        string s = "";
        for(int i=0; i<cards.Count; i++)
        {
            s += cards[i].name + ";" + cards[i].GetParent().name + ";" + cards[i].m_face + "|";
        }
        m_states.Add(s);
        */
        ObjectBaseSerialized os = new ObjectBaseSerialized(o);
        string s = JsonUtility.ToJson(os);
        for(int i = m_states.Count-1; i> m_curStateIndex; i--)
        {
            m_states.RemoveAt(i);
        }
        m_states.Add(s);
        m_curStateIndex = m_states.Count-1;
        //Debug.Log("state saved");
    }

    public void LoadPreviousState(ObjectBase b)
    {
        GameMaster gm = this.GetComponent<GameMaster>();
        m_curStateIndex--;
        if(m_curStateIndex <0)
        {
            m_curStateIndex = 0;
        }
        gm.PutCardsInStartDeck();
        string s = m_states[m_curStateIndex];
        ObjectBaseSerialized o = JsonUtility.FromJson<ObjectBaseSerialized>(s);
        o.Restore(b);
        /*
        string[] ls = s.Split('|');
        for(int i=0; i<ls.Length; i++)
        {
            string[] data = ls[i].Split(';');
            CardScript c = (CardScript)gm.Find(data[0]);
            ObjectBase p = gm.Find(data[1]);
            if(c != null)
            {
                CardScript.Face f = (data[2] == "recto") ? CardScript.Face.recto : CardScript.Face.verso;
                c.MoveToParent(p, f, true, 0, 100);
            }
        }
        */
        Debug.Log("state loaded");
    }

    public void LoadNextState(ObjectBase b)
    {
        m_curStateIndex++;
        Debug.Log("state loaded");
    }
}

public class Record
{
}
