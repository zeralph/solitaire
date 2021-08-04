using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

public class CardScript : ObjectBase
{
    public enum CardColor
    {
        red=0,
        black=1,
        __MAX__
    }
    public enum Symbol
    {
        spade=0,
        heart=1,
        diamond=2,
        club=3,
        __MAX__
    }
    public enum Face
    {
        recto=0,
        verso=1
    }
    public enum Name
    {
        ace=1,
        two=2,
        three=3,
        four=4,
        five=5,
        six=6,
        sevent=7,
        eight=8,
        nine=9,
        ten=10,
        jack=11,
        queen=12,
        king=13,
        __MAX__
    }
    private const int LAYER_CARD = 6;
    private const int LAYER_DUMMY = 0;
    private readonly string[] names = {"A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K"};
    private bool m_isFlipping = false;
    public Transform m_mesh;
    public Transform m_symbolImage;
    public Transform m_value1Image;
    public Transform m_value2Image;
    public Transform m_symbol1Image;
    public Transform m_symbol2Image;
    private Vector3 m_targetPos;
    private bool m_isMoving;
    public float m_boomFactor = 100.0f;
    public float m_flipSpeed = 100.0f;
    private float m_speed;
    //[ReadOnly]
    public int m_value;
    //[ReadOnly]
    public Name m_name;
    //[ReadOnly]
    public CardColor m_color;
    //[ReadOnly]
    public Symbol m_symbol;
    public Face m_face;
    private bool m_moveWithMouse;

    public override void Start()
    {
        base.Start();
        m_targetPos = transform.position;
        m_isMoving = false;
        m_moveWithMouse = false;
        m_speed = 0f;
    }


    void Update()
    {
        if(m_moveWithMouse)
        {
            MoveWithMouse();
        }
        UpdateMove();
        UpdateFlip();
        //when parent is moving
        if(!m_isMoving && transform.position != m_targetPos)
        {
            m_targetPos = transform.position;
        }
    }

    public void Boom()
    {
        Rigidbody rb = this.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        float x = Random.Range(-1f, 1f);
        float y = Random.Range(-1f, 1f);
        float z = Random.Range(-1f, 0f);
        float f = m_boomFactor;
        Vector3 t = new Vector3(x, y, z);
        Vector3 v = Camera.main.transform.position - transform.position;
        v.Normalize();
        v.x *= f;// * x;
        v.y *= f;// * y;
        v.z *= f;// * z;
        rb.AddForce(v);
        rb.AddTorque(t, ForceMode.Impulse);
    }

    public bool IsRecto()
    {
        return !m_isFlipping && m_face == Face.recto;
    }

    public void FlipRectoToVerso()
    {
        //will play the animation
    }

    public bool GetHitable()
    {
        return this.gameObject.layer == LAYER_CARD;
    }

    public bool IsInDrawn()
    {

        return GetParent().GetComponent<DeckScript>() != null && GetParent().GetComponent<DeckScript>().name == "Drawn";
    }

    public bool IsInDiscard()
    {
        return GetParent().GetComponent<DeckScript>() != null && GetParent().GetComponent<DeckScript>().name == "Discard";
    }


    public void SetHitable(bool b)
    {
        if(b)
        {
            this.gameObject.layer = LAYER_CARD;
        }
        else
        {
            this.gameObject.layer = LAYER_DUMMY;
        }     
    }

    public void StartMoveWithMouse()
    {
        if(!m_isMoving)
        {
            m_moveWithMouse = true;
            m_speed = m_gameMaster.m_mouseSpeed;
            SetHitable(false);
        } 
    }

    public override float GetCardDecal()
    {
        return m_cardDecal;
    }

    public override bool IsMoving()
    {
        return m_isMoving;
    }

    public void StopMoveWithMouse()
    {
        if(m_moveWithMouse)
        {
            m_moveWithMouse = false;
            Transform t = null;
            if (m_face == Face.recto)
            {
                t = GetObjectUnder();
            }
            bool bFailed = true;
            ObjectBase curP = GetParent();
            if (t != null)
            {
                CardScript s = t.GetComponent<CardScript>();
                Tableau tab = t.GetComponentInParent<Tableau>();
                FamilyPile fam = t.GetComponentInParent<FamilyPile>();
                DeckScript dek = t.GetComponentInParent<DeckScript>();
                if (s && CanAdd(s) && dek == null)
                {
                    MoveToParent(s, m_face, false, s.GetCardDecal(), m_gameMaster.m_speed);
                    bFailed = false;
                }
                else if (tab != null && tab.IsEmpty() && this.m_name == Name.king)
                {
                    MoveToParent(tab, m_face, false, tab.GetCardDecal(), m_gameMaster.m_speed);
                    bFailed = false;
                }
                else if (fam != null)
                {
                    if (fam.CanAdd(this))
                    {
                        this.MoveToParent(fam, CardScript.Face.recto, true, fam.GetCardDecal(), m_gameMaster.m_speed);
                        bFailed = false;
                    }
                }
            }
            if (bFailed)
            {
                //move back
                ObjectBase parentObject = GetParent().GetComponent<ObjectBase>();
                float d = parentObject.GetCardDecal();
                MoveToParent(curP, m_face, false, d, m_gameMaster.m_moveBackSpeed);
            }
            SetHitable(true);
        }      
    }

    private bool CanAdd(CardScript s)
    {
        return this.m_color != s.m_color && this.m_value == (s.m_value - 1) && s.m_face == Face.recto;
    }

    public override Vector3 GetTargetPosition(ObjectBase b)
    {
        return new Vector3(m_targetPos.x, m_targetPos.y, m_targetPos.z);
    }

    public void MoveToParent(ObjectBase newparent, Face f, bool flipImmediate, float decal, float speed)
    {
        ObjectBase curP = GetParent();
        //Debug.Log("Move " + this.name + " from " + curP.name + " to " + newparent.name + ", decal " + decal);
        if (curP == null)
        {
            Debug.LogError(m_name + " has no parent. strange ?");
        }
        else
        {
            Vector3 v = curP.GetTargetPosition(this);
            {
                Tableau tab = curP.GetComponent<Tableau>();
                newparent.Add(this);
                v = newparent.GetTargetPosition(this);
                flipTo(f, flipImmediate);
                if (tab != null)
                {
                    tab.FlipTopcard();
                }
            }
            decal = newparent.GetCardDecal();
            v.y -= decal;
            m_targetPos.x = v.x;
            m_targetPos.y = v.y;
            m_targetPos.z = v.z - m_gameMaster.m_cardSpace;
            m_isMoving = true;
            m_speed = speed;
        }
    }


    private void MoveWithMouse()
    {
        Plane plane = new Plane(Vector3.forward, 0);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float point = 0f;
        if (plane.Raycast(ray, out point))
        {
            m_targetPos = ray.GetPoint(point);
            m_targetPos.z = m_gameMaster.m_zWhenMoving;
            m_isMoving = true;
            SetHitable(false);
        }
    }

    private void UpdateMove()
    {
        if(m_isMoving)
        {
            SetHitable(false);
            this.transform.position = Vector3.MoveTowards(this.transform.position, m_targetPos, m_speed * Time.deltaTime);
            CardScript child = this.transform.GetComponentInChildren<CardScript>();
            if (transform.position == m_targetPos)
            {
                m_isMoving = false;
                SetHitable(true);
            }
        }
    }

    public void flipTo(Face f, bool immediate)
    {
        if(!m_isFlipping && f != m_face)
        {
            m_face = f;
            Animation a = m_mesh.GetComponent<Animation>();
            immediate = true;
            if (immediate && m_face == Face.recto)
            {
                a.Play("cardFlipVersoToRecto", PlayMode.StopAll);
                //m_mesh.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            }
            else if (immediate && m_face == Face.verso)
            {
                a.Play("cardFlipRectoToVerso", PlayMode.StopAll);
                //m_mesh.transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
            }
            else
            {
                
                //a.Play("cardFlipVersoToRecto", PlayMode.StopAll);
                a.Play("cardFlipRectoToVerso", PlayMode.StopAll);
            }               
        }      
    }

    public void EndAnimation()
    {
    }

    public void Set(CardScript.Symbol symbol, CardScript.Name name)
    {
        m_face = Face.recto;
        m_name = name;
        m_symbol = symbol;
        m_value = (int)name;
        m_color = (symbol == Symbol.club || symbol == Symbol.spade)?CardColor.black:CardColor.red;
        Material m;
        if (m_color != CardColor.black)
        {
            m = Resources.Load("redMaterial", typeof(Material)) as Material;
        }
        else
        {
            m = Resources.Load("blackMaterial", typeof(Material)) as Material;
        }
        string valueTextureName = "cards/" + m_value + "";
        string symboltextureName = "";
        if(m_symbol == Symbol.diamond)
        {
            symboltextureName = "cards/diamond";
        }
        else if (m_symbol == Symbol.club)
        {
            symboltextureName = "cards/club";
        }
        else if (m_symbol == Symbol.spade)
        {
            symboltextureName = "cards/spade";
        }
        else if (m_symbol == Symbol.heart)
        {
            symboltextureName = "cards/heart";
        }
        Texture valueTexture = Resources.Load(valueTextureName, typeof(Texture)) as Texture;
        m_value1Image.GetComponent<Renderer>().material.CopyPropertiesFromMaterial(m);
        m_value1Image.GetComponent<Renderer>().material.SetTexture("_MainTex", valueTexture);
        m_value2Image.GetComponent<Renderer>().material.CopyPropertiesFromMaterial(m);
        m_value2Image.GetComponent<Renderer>().material.SetTexture("_MainTex", valueTexture);

        Texture symbolTexture = Resources.Load(symboltextureName, typeof(Texture)) as Texture;
        m_symbolImage.GetComponent<Renderer>().material.CopyPropertiesFromMaterial(m);
        m_symbolImage.GetComponent<Renderer>().material.SetTexture("_MainTex", symbolTexture);
        m_symbol1Image.GetComponent<Renderer>().material.CopyPropertiesFromMaterial(m);
        m_symbol1Image.GetComponent<Renderer>().material.SetTexture("_MainTex", symbolTexture);
        m_symbol2Image.GetComponent<Renderer>().material.CopyPropertiesFromMaterial(m);
        m_symbol2Image.GetComponent<Renderer>().material.SetTexture("_MainTex", symbolTexture);
    }

    private void UpdateFlip()
    {
        if (m_isFlipping)
        {
            Quaternion targetRot = Quaternion.Euler(0.0f, 180.0f, 0.0f);
            if (m_face == Face.recto)
            {
                targetRot = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            }
            m_mesh.transform.rotation = Quaternion.RotateTowards(m_mesh.transform.rotation, targetRot, m_flipSpeed * Time.deltaTime);
            if (m_mesh.transform.rotation == targetRot)
            {
                m_isFlipping = false;
            }
        }
    }

    private Transform GetObjectUnder()
    {
        int layerMask = 1 << 6;
        Ray ray = new Ray(this.transform.position, Vector3.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            return hit.transform;
        }
        return null;
    }
}
