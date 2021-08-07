using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateRecorder : MonoBehaviour
{
    public List<string> m_states;
    // Start is called before the first frame update
    void Start()
    {
        m_states = new List<string>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Clear()
    {
        m_states.Clear();
    }

    public void SaveState(List<CardScript> cards)
    {
        string s = "";
        for(int i=0; i<cards.Count; i++)
        {
            s += cards[i].name + ";" + cards[i].GetParent().name + ";" + cards[i].m_face + "|";
        }
        m_states.Add(s);
    }
}

public class Record
{
}
