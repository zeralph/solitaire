using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

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
    ///List<string> m_values;
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
    UnityAction<string> m_onChange;
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
        //m_values = new List<string>();
        for(int i=0; i< m_layoutGroup.transform.childCount; i++)
        {
            SolitaireListItem it = m_layoutGroup.transform.GetChild(i).GetComponent<SolitaireListItem>();
            AddItemlInternal(it, true, true, null);
        }
        m_onChange = null;
    }

    public void AddOnChangeFunction(UnityAction<string> onChange)
    {
        m_onChange = onChange;
    }

    public string GetValue()
    {
        int i = FindNearest(m_scroll.horizontalNormalizedPosition, m_points);
        /*
        if(i>=0 && i< m_values.Count)
        {
            return m_values[i];
        }
        return "";
        */
        SolitaireListItem it = m_layoutGroup.transform.GetChild(i).GetComponent<SolitaireListItem>();
        return it.GetValue();
    }
    
    public bool SetValue(string v)
    {
        int l = m_layoutGroup.transform.childCount;
        for(int i=0; i<l; i++)
        {
            SolitaireListItem it = m_layoutGroup.transform.GetChild(i).GetComponent<SolitaireListItem>();
            if(it.GetValue() == v)
            {
                m_scroll.horizontalNormalizedPosition = m_points[i];
                return true;
            }
        }
        return false;
    }
    
    public void AddItem(string text, string value, bool fillH, bool fillV, Func<bool> func = null)
    {
        RectTransform parent = m_layoutGroup.GetComponent<RectTransform>();
        GameObject o = Instantiate(m_itemPrefab.gameObject, parent);
        SolitaireListItem it = o.GetComponent<SolitaireListItem>();
        o.SetActive(true);
        it.Set(text, value);
        AddItemlInternal(it, fillH, fillV);

    }
    void AddItemlInternal(SolitaireListItem it, bool fillH, bool fillV, Func<bool> func = null)
    {
        m_layoutGroup = this.GetComponentInChildren<HorizontalOrVerticalLayoutGroup>();
        RectTransform parent = m_layoutGroup.GetComponent<RectTransform>();
        Vector2 parentSize = parent.rect.size;
        RectTransform r = it.GetComponent<RectTransform>();
        r.SetParent(parent);
        Vector2 itemSize = r.rect.size;
        float ax = (parentSize.x - itemSize.x) / 2;
        float ay = (parentSize.y - itemSize.y) / 2;
        r.anchoredPosition = new Vector2(ax, ay);
        Vector2 size = parent.sizeDelta;
        size.x += r.sizeDelta.x;
        size.y += r.sizeDelta.y;
        parent.sizeDelta = size;
        m_stepSize = r.sizeDelta.x;
        //m_values.Add(it.GetValue());
        m_screens++;
        ComputePoints();
    }
    void Update()
    {
        if (LerpH)
        {
            m_scroll.horizontalNormalizedPosition = Mathf.Lerp(m_scroll.horizontalNormalizedPosition, targetH, snapSpeed * Time.deltaTime);
            //if (Mathf.Approximately(m_scroll.horizontalNormalizedPosition, targetH))
            if(Mathf.Abs(m_scroll.horizontalNormalizedPosition - targetH) < 0.01f)
            {
                m_scroll.horizontalNormalizedPosition = targetH;
                CallOnChange();
                LerpH = false;
            }     
        }
        if (LerpV)
        {
            m_scroll.verticalNormalizedPosition = Mathf.Lerp(m_scroll.verticalNormalizedPosition, targetV, snapSpeed * Time.deltaTime);
            if (Mathf.Approximately(m_scroll.verticalNormalizedPosition, targetV))
            {
                m_scroll.verticalNormalizedPosition = targetV;
                CallOnChange();
                LerpV = false;
            }
        }
    }

    void CallOnChange()
    {
        string v = GetValue();
        Debug.Log($"[CallOnChange] : calling {m_onChange}, parameter {v} ");
        if (m_onChange != null)
        {
            m_onChange(v);
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
