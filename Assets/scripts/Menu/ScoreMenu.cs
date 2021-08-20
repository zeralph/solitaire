using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreMenu : MonoBehaviour
{
    public Button m_quit;
    public SolitaireListItem m_ListItem;
    public UnityEngine.UI.VerticalLayoutGroup m_verticalLayoutGroup;
    public SolitaireListView m_scoreList;


    // Start is called before the first frame update
    void Start()
    {
        ScoreListSerialized scoresList = GetComponent<StateRecorder>().GetScores();
        int w = 0;
        int h = 0;
        RectTransform parent = m_verticalLayoutGroup.GetComponent<RectTransform>();
        for (int i = 0; i < scoresList.scoreList.Length; ++i)
        {
            GameObject g = Instantiate(m_ListItem.gameObject, new Vector3(0, 0, 0), Quaternion.identity);
            SolitaireListItem l = g.GetComponent<SolitaireListItem>();
            ScoreSerialized ss = scoresList.scoreList[i];
            if (ss != null && ss.set)
            {
                l.SetText($"{ss.score} pts {ss.turn} turns");
            }
            else
            {
                l.SetText($"");
            }
            m_scoreList.AddItem(l);
        }
    }
    /*
    // Start is called before the first frame update
    void Start()
    {
        ScoreListSerialized scoresList = GetComponent<StateRecorder>().GetScores();
        int w=0;
        int h=0;
        RectTransform parent = m_verticalLayoutGroup.GetComponent<RectTransform>();
        for (int i = 0; i < scoresList.scoreList.Length; ++i)
        {
            GameObject g = Instantiate(m_ListItem.gameObject, new Vector3(0, 0, 0), Quaternion.identity);
            SolitaireListItem l = g.GetComponent<SolitaireListItem>();
            RectTransform r = l.GetComponent<RectTransform>();
            r.SetParent(parent);
            w = (int)r.sizeDelta.x;
            h += (int)r.sizeDelta.y;
            ScoreSerialized ss = scoresList.scoreList[i];
            if (ss !=null && ss.set)
            {
                l.SetText($"{ss.score} pts {ss.turn} turns");
            }
            else
            {
                l.SetText($"");
            }

        }
        RectTransform r2 = m_verticalLayoutGroup.GetComponent<RectTransform>();
        r2.sizeDelta = new Vector2(r2.sizeDelta.x, h);
        m_quit.onClick.AddListener(QuitToMainMenu);
    }
    */
    private void QuitToMainMenu()
    {
        SceneManager.LoadScene("StartScene");
    }
}
