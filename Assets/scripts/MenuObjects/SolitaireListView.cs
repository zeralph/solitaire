using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SolitaireListView : MonoBehaviour, IDragHandler, IEndDragHandler
{
    private HorizontalOrVerticalLayoutGroup m_layoutGroup;
    public SolitaireListItem m_itemPrefab;
    public Button m_previous;
    public Button m_next;
    private float m_containerWidth;
    private float m_containerHeight;

    //Drag system
    float[] m_points;
    List<string> m_values;
    private int m_screens = 0;
    [Tooltip("How quickly the GUI snaps to each panel")]
    public float snapSpeed;
    public float inertiaCutoffMagnitude;
    public float m_stepSize;
    ScrollRect m_scroll;
    bool LerpH;
    float targetH;
    [Tooltip("Snap horizontally")]
    public bool snapInH = true;
    bool LerpV;
    float targetV;
    [Tooltip("Snap vertically")]
    public bool snapInV = true;
    bool dragInit = true;
    int dragStartNearest;
    //End drag system

    void Awake()
    {
        m_layoutGroup = this.GetComponentInChildren<HorizontalOrVerticalLayoutGroup>();
        RectTransform parent = m_layoutGroup.GetComponent<RectTransform>();
        m_containerWidth = parent.rect.size.x;
        m_containerHeight = parent.rect.size.y;
        //Drag system
        m_scroll = this.GetComponent<ScrollRect>();
        m_scroll.inertia = true;
        m_stepSize = m_containerWidth;
        ComputePoints();
        m_values = new List<string>();
        //End drag system
    }

    public string GetValue()
    {
        int i = FindNearest(m_scroll.horizontalNormalizedPosition, m_points);
        if(i>=0 && i< m_values.Count)
        {
            return m_values[i];
        }
        return "";
    }

    public void AddItem(string text, bool fillH, bool fillV, Func<bool> func = null)
    {
        m_layoutGroup = this.GetComponentInChildren<HorizontalOrVerticalLayoutGroup>();
        RectTransform parent = m_layoutGroup.GetComponent<RectTransform>();
        Vector2 parentSize = parent.rect.size;
        GameObject o = Instantiate(m_itemPrefab.gameObject, parent);
        SolitaireListItem it = o.GetComponent<SolitaireListItem>();
        o.SetActive(true);
        it.SetText(text);
        RectTransform r = o.GetComponent<RectTransform>();
        r.SetParent(parent);
        Vector2 itemSize = r.rect.size;
        float ax = (parentSize.x - itemSize.x)/2;
        float ay = (parentSize.y - itemSize.y)/2;
        r.anchoredPosition = new Vector2(ax, ay);
        Vector2 size = parent.sizeDelta;
        size.x += r.sizeDelta.x;
        size.y += r.sizeDelta.y;
        parent.sizeDelta = size;
        m_stepSize = r.sizeDelta.x;
        m_values.Add(text);
        m_screens++;
        ComputePoints();
    }

    void Update()
    {
        if (LerpH)
        {
            m_scroll.horizontalNormalizedPosition = Mathf.Lerp(m_scroll.horizontalNormalizedPosition, targetH, snapSpeed * Time.deltaTime);
            if (Mathf.Approximately(m_scroll.horizontalNormalizedPosition, targetH))
                LerpH = false;
        }
        if (LerpV)
        {
            m_scroll.verticalNormalizedPosition = Mathf.Lerp(m_scroll.verticalNormalizedPosition, targetV, snapSpeed * Time.deltaTime);
            if (Mathf.Approximately(m_scroll.verticalNormalizedPosition, targetV))
                LerpV = false;
        }
    }

    void ComputePoints()
    {
        if (m_screens > 0)
        {
            m_points = new float[m_screens];
            float f = 1 / (float) (m_screens);
            for (int i = 0; i<m_screens; i++)
            {
                m_points[i] = i* f;
            }
        }
        else
        {
            m_points = new float[1];
            m_points[0] = 0;        
        }
    }


    void IEndDragHandler.OnEndDrag(PointerEventData data)
    {
        int target = FindNearest(m_scroll.horizontalNormalizedPosition, m_points);

        if (target == dragStartNearest && m_scroll.velocity.sqrMagnitude > inertiaCutoffMagnitude * inertiaCutoffMagnitude)
        {
            if (m_scroll.velocity.x < 0)
            {
                target = dragStartNearest + 1;
            }
            else if (m_scroll.velocity.x > 1)
            {
                target = dragStartNearest - 1;
            }
            target = Mathf.Clamp(target, 0, m_points.Length - 1);
        }

        if (m_scroll.horizontal && snapInH && m_scroll.horizontalNormalizedPosition > 0f && m_scroll.horizontalNormalizedPosition < 1f)
        {
            targetH = m_points[target];
            LerpH = true;
        }
        if (m_scroll.vertical && snapInV && m_scroll.verticalNormalizedPosition > 0f && m_scroll.verticalNormalizedPosition < 1f)
        {
            targetV = m_points[target];
            LerpV = true;
        }

        dragInit = true;
    }

    void IDragHandler.OnDrag(PointerEventData data)
    {
        if (dragInit)
        {
            dragStartNearest = FindNearest(m_scroll.horizontalNormalizedPosition, m_points);
            dragInit = false;
        }

        LerpH = false;
        LerpV = false;
    }

    int FindNearest(float f, float[] array)
    {
        float distance = Mathf.Infinity;
        int output = 0;
        for (int index = 0; index < array.Length; index++)
        {
            if (Mathf.Abs(array[index] - f) < distance)
            {
                distance = Mathf.Abs(array[index] - f);
                output = index;
            }
        }
        return output;
    }
}
