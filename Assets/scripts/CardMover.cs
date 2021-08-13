using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class CardMover : ObjectBase
{
    private CardScript m_card;
    private ObjectBase m_lastObject;
    private const int MOUSE = 0;
    private bool pressed = false;
    
    public override void Start()
    {
        base.Start();
        pressed = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
    }

    void Update()
    {
        if (GetGameMaster().IsPaused())
        {
            return;
        }
        if(m_lastObject != null)
        {
            m_lastObject.OutLine(false);
        }
        m_lastObject = GetClickedObject();
        if (Input.GetMouseButtonDown(MOUSE)) //CLICK
        {
            DeckScript dek = null;
            if (m_lastObject != null)
            {
                m_card = (m_lastObject != null) ? m_lastObject.GetComponent<CardScript>() : null;
                dek = (m_lastObject != null) ? m_lastObject.GetComponent<DeckScript>() : null;
            }    
            if ((m_card && m_card.IsInDrawn()) || dek != null)
            {
                GetGameMaster().OnClickOnDrawn();
                pressed = false;
            }
        }
        else //DRAG
        {
            if (Input.GetMouseButton(MOUSE))
            {
                if (m_card != null && !pressed)
                {
                    pressed = true;
                    if(m_card.IsMoving())
                    {
                        return;
                    }
                    if (m_card && !m_card.IsInDrawn() && !(m_card.IsInTableau() && !m_card.IsRecto()))
                    {
                        m_card.StartMoveWithMouse();
                    }
                    else if (m_card.IsInDrawn())
                    { 
                        {
                            GetGameMaster().OnClickOnDrawn();
                            pressed = false;
                            m_card = null;
                        }
                    }
                }
            }
            else if (pressed && m_card)
            {
                pressed = false;
                m_card.StopMoveWithMouse();
                m_card = null;
            }
            else
            {
                pressed = false;
            }
        }    
    }

    void OnGUI()
    {
        if(m_card != null)
        {
            GUI.Label(new Rect(10, 10, 100, 20), m_card.name);
            GUI.Label(new Rect(10, 20, 100, 20), "HIT : "+ m_card.GetHitable());
        }
        if (m_lastObject != null)
        {
            GUI.Label(new Rect(10, 50, 100, 20), "LAST CARD");
            GUI.Label(new Rect(10, 60, 100, 20), m_lastObject.name);
            GUI.Label(new Rect(10, 70, 100, 20), "HIT : " + m_lastObject.GetHitable());
        }
    }

    private ObjectBase GetClickedObject()
    {
        int layerMask = 1 << 6;
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            ObjectBase cs = hit.transform.GetComponent<ObjectBase>();
            if(cs != null)
            {
                return cs;
            }
        }
        return null;
    }
}
