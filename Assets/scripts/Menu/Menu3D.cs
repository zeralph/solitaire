using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu3D : MonoBehaviour
{
    public WorldMoveScript m_world;
    private List<CardScript> m_cards;
    private Vector3 m_lastCamPosition;
    public float m_cardSpawnHorizontalRadius = 3;

    // Start is called before the first frame update
    void Start()
    {
        m_cards = null;
        m_lastCamPosition = Camera.main.transform.position;
        m_world.StartMove();
    }

    // Update is called once per frame
    void Update()
    {
        CardsCreator cc = GetComponent<CardsCreator>();
        if (!cc.CardCreated())
        {
            //cc.CreateCards();
            cc.CreateRandomCards(200);
            m_cards = cc.GetCards();
            for (int i = 0; i < m_cards.Count; i++)
            {
                InitializeCard(m_cards[i]);
            }
        }
        if (m_cards != null)
        {
            for (int i = 0; i < m_cards.Count; i++)
            {
                if (m_cards[i].transform.position.y < Camera.main.transform.position.y)
                {
                    if (!m_cards[i].IsInFrustrum())
                    {
                        InitializeCard(m_cards[i]);
                    }
                }
            }
        }
    }

    private void InitializeCard(CardScript c)
    {
        c.DisablePhysic();
        float x = Random.Range(-m_cardSpawnHorizontalRadius, m_cardSpawnHorizontalRadius);
        float y = Random.Range(-0f, 5f);
        float z = Random.Range(-m_cardSpawnHorizontalRadius, m_cardSpawnHorizontalRadius);
        Vector3 v = new Vector3(x, y, z);
        c.transform.position = c.GetParent().transform.position + v;
        x = Random.Range(-180.0f, 180.0f);
        y = Random.Range(-180.0f, 180.0f);
        z = Random.Range(-180.0f, 180.0f);
        Quaternion r = Quaternion.Euler(x, y, z);
        c.transform.rotation = r;
        x = Random.Range(-1f, 1f);
        y = Random.Range(-50f, -20f);
        z = Random.Range(-1f, 1f);
        Vector3 f = new Vector3(x, y, z);
        x = Random.Range(-1f, 1f);
        y = Random.Range(-1f, 1f);
        z = Random.Range(-1f, 1f);
        Vector3 t = new Vector3(x, y, z);
        c.EnablePhysic(false);
        c.AddForceAndTorque(f, t);
    }
}
