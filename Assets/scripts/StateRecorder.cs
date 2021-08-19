using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateRecorder : MonoBehaviour
{
    public static readonly int SAVE_VERSION = 1;
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
    public ScoreSerialized[] GetScores()
    {
        if (PlayerPrefs.HasKey("Scores"))
        {
            string s = PlayerPrefs.GetString("Scores");
            ScoreListSerialized sl = JsonUtility.FromJson<ScoreListSerialized>(s);
            Debug.Log("SCORE LIST : " + s);
            return sl.scoreList;
        }
        return new ScoreSerialized[100];
    }

    public void AddScore(int score, int turn)
    {
        ScoreSerialized[] scoreList = GetScores();
        int minScore = -10000000;
        int idxMinScore = 0;
        bool bInserted = false;
        for (int i = 0; i < scoreList.Length; i++)
        {
            if (!scoreList[i].set)
            {
                scoreList[i].set = true;
                scoreList[i].turn = turn;
                scoreList[i].score = score;
                bInserted = true;
                break;
            }
            else if(minScore> scoreList[i].score)
            {
                minScore = scoreList[i].score;
                idxMinScore = i;
            }
        }
        if(!bInserted)
        {
            scoreList[idxMinScore].set = true;
            scoreList[idxMinScore].turn = turn;
            scoreList[idxMinScore].score = score;
        }
        string s = JsonUtility.ToJson(scoreList);
        PlayerPrefs.SetString("Scores", s);
    }

    public bool CanUndo()
    {
        return m_curStateIndex > 0;
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
        GameMaster gm = this.GetComponent<GameMaster>();
        StateSerialized ss = new StateSerialized(StateRecorder.SAVE_VERSION, gm.m_score, gm.m_turn, o);
        string state = JsonUtility.ToJson(ss);
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
        StateSerialized ss = JsonUtility.FromJson<StateSerialized>(s);
        if(ss.version != StateRecorder.SAVE_VERSION)
        {
            Debug.LogWarning("[StateRecorder] : load state failes because of version mismatch");
        }
        else
        {
            gm.m_turn = ss.turn;
            gm.m_score = ss.score;
            ss.o.Restore(b);
            Debug.Log("state loaded");
        } 
    }

    public void LoadNextState(ObjectBase b)
    {
        GameMaster gm = this.GetComponent<GameMaster>();
        m_curStateIndex++;
        if (m_curStateIndex == m_states.Count)
        {
            return;
        }
        gm.PutCardsInStartDeck();
        string s = m_states[m_curStateIndex];
        ObjectBaseSerialized o = JsonUtility.FromJson<ObjectBaseSerialized>(s);
        o.Restore(b);
        Debug.Log("state loaded");
    }
}

[System.Serializable]
public class ScoreListSerialized
{
    public ScoreSerialized[] scoreList = new ScoreSerialized[100]; 
}

[System.Serializable]
public class ScoreSerialized
{
    public int score = 0;
    public int turn = 0;
    public bool set = false;
    //public date;
}

[System.Serializable]
public class StateSerialized
{
    public int score;
    public int version;
    public int turn;
    public ObjectBaseSerialized o;
    public StateSerialized(int version, int score, int turn, ObjectBase startObj)
    {
        this.version = version;
        this.score = score;
        this.turn = turn;
        o = new ObjectBaseSerialized(startObj);
    }
    public bool Restore(ref int score, ref int turn, ObjectBase startObj)
    {
        if(this.version != StateRecorder.SAVE_VERSION)
        {
            return false;
        }
        score = this.score;
        turn = this.turn;
        return o.Restore(startObj);
    }
}
