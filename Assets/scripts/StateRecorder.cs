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

    public bool HasSave()
    {
        return PlayerPrefs.HasKey("Save");
    }

    public void ClearSave()
    {
        PlayerPrefs.DeleteKey("Save");
    }

    public void LoadFromSave(ObjectBase b)
    {
        if (HasSave())
        {
            string save = PlayerPrefs.GetString("Save");
            m_states.Clear();
            m_states.Add(save);
            m_curStateIndex = 1;
            LoadPreviousState(b);
        }
    }

    public void Clear()
    {
        m_states.Clear();
        m_curStateIndex = 0;
        Debug.Log("states cleared");
    }

    public void AddState(ObjectBase o, bool bSave)
    {
        ObjectBaseSerialized os = new ObjectBaseSerialized(o);
        string state = JsonUtility.ToJson(os);
        for(int i = m_states.Count-1; i> m_curStateIndex; i--)
        {
            m_states.RemoveAt(i);
        }
        m_states.Add(state);
        m_curStateIndex = m_states.Count-1;
        if(bSave)
        {
            PlayerPrefs.SetString("Save", state);
        }
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
