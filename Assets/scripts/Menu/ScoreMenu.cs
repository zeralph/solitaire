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
        m_quit.onClick.AddListener(QuitToMainMenu);
        ScoreListSerialized scoresList = GetComponent<StateRecorder>().GetScores();
        RectTransform parent = m_verticalLayoutGroup.GetComponent<RectTransform>();
        for (int i = 0; i < scoresList.scoreList.Length; ++i)
        {
            ScoreSerialized ss = scoresList.scoreList[i];
            string s = "";
            if (ss != null && ss.set)
            {
                s = $"{ss.score} pts {ss.turn} turns";
            }
            m_scoreList.AddItem(s, s, true, false);
        }
    }
    
    private void QuitToMainMenu()
    {
        //SceneManager.LoadScene("StartScene");
        SceneLoader sl = FindObjectsOfType<SceneLoader>()[0];
        sl.LoadScene(false, true, false);
    }
}
