using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class megaSunScript : MonoBehaviour
{
    public Transform m_water;
    private float m_x;
    private float m_y;
    // Start is called before the first frame update
    void Start()
    {
        m_x = 0;
        m_y = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Material m = m_water.GetComponent<Renderer>().material;
        Vector2 v = new Vector2(m_x, m_y);
        m.SetTextureOffset("_BumpMap", v);
        m_x += 0.1f;
    }
}
