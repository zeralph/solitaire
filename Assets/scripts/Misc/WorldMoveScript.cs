using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMoveScript : MonoBehaviour
{
    private float m_rotY = 0;
    private Quaternion m_startAngle;
    private Vector3 m_startPosition;
    public bool m_startMove = false;
    public float m_speed = 0.07f;

    // Start is called before the first frame update
    void Awake()
    {
        m_startMove = false;
        m_startAngle = this.transform.rotation;
        m_startPosition = this.transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (m_startMove)
        {
            
            Vector3 a = this.transform.eulerAngles;
            a.y = m_rotY;
            this.transform.eulerAngles = a;
            
            //RenderSettings.skybox.SetFloat("_Rotation", m_rotY);
            m_rotY += m_speed;
        }
    }

    public void StartMove()
    {       
        m_startMove = true;
    }

    public void Reset()
    {
        m_startMove = false;
        this.transform.position = m_startPosition;
        this.transform.rotation = m_startAngle;

    }
}
