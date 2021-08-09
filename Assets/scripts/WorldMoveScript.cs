using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMoveScript : MonoBehaviour
{
    private float m_rotY = 0;
    public bool m_startMove = false;
    public float m_speed = 0.07f;
    // Start is called before the first frame update
    void Start()
    {
        m_startMove = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (m_startMove)
        {
            this.transform.eulerAngles = new Vector3(0, m_rotY, 0);
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
        this.transform.position = new Vector3(0, 0, 0);
        this.transform.rotation = Quaternion.identity;

    }
}
