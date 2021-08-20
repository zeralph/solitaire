using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SolitaireListView : MonoBehaviour
{
    private HorizontalOrVerticalLayoutGroup m_layoutGroup;

    void Start()
    {
        m_layoutGroup = this.GetComponentInChildren<HorizontalOrVerticalLayoutGroup>();
    }

    public void AddItem(SolitaireListItem it)
    {
        int w = 0;
        int h = 0;
        RectTransform parent = m_layoutGroup.GetComponent<RectTransform>();
        it.GetComponent<RectTransform>().SetParent(parent);
        /*
        for (int i = 0; i < scoresList.scoreList.Length; ++i)
        {
            GameObject g = Instantiate(m_ListItem.gameObject, new Vector3(0, 0, 0), Quaternion.identity);
            SolitaireListItem l = g.GetComponent<SolitaireListItem>();
            RectTransform r = l.GetComponent<RectTransform>();
            r.SetParent(parent);
            w = (int)r.sizeDelta.x;
            h += (int)r.sizeDelta.y;
            ScoreSerialized ss = scoresList.scoreList[i];
            if (ss != null && ss.set)
            {
                l.SetText($"{ss.score} pts {ss.turn} turns");
            }
            else
            {
                l.SetText($"");
            }

        }
        RectTransform r2 = m_layoutGroup.GetComponent<RectTransform>();
       
        r2.sizeDelta = new Vector2(r2.sizeDelta.x, h);
        */
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
