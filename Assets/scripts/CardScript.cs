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
    public Transform m_mesh;
    public Transform m_recto;
    public Transform m_verso;
    public Transform m_symbolImage;
    public Transform m_value1Image;
    public Transform m_value2Image;
    public Transform m_symbol1Image;
    public Transform m_symbol2Image;
    private Vector3 m_targetPos;
    private bool m_isMoving;
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
        if (m_gameMaster.IsPaused())
        {
            return;
        }
        if (m_moveWithMouse)
        {
            MoveWithMouse();
        }
        //when parent is moving
        UpdateMove();
        if (!m_isMoving && transform.position != m_targetPos)
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
        float f = m_gameMaster.m_boomFactor;
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
        return m_face == Face.recto;
    }

    public void FlipRectoToVerso()
    {
        //will play the animation
    }

    public override bool GetHitable()
    {
        return this.gameObject.layer == LAYER_CARD;
    }

    public bool IsInDrawn()
    {

        return GetParent().GetComponent<DeckScript>() != null && GetParent().GetComponent<DeckScript>().name == "Drawn";
    }

    public bool IsInTableau()
    {
        return GetParent().GetComponent<Tableau>() != null;
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
            ObjectBase newP = (t!=null)?t.GetComponentInParent<ObjectBase>():null;
            //GameMaster.eMoves move = GameMaster.eMoves.eNullMove;
            if (newP != null)
            {
                CardScript s = newP.GetComponentInParent<CardScript>();
                Tableau tab = newP.GetComponentInParent<Tableau>();
                FamilyPile fam = newP.GetComponentInParent<FamilyPile>();
                DeckScript dek = newP.GetComponentInParent<DeckScript>();
                if (s && CanAdd(s) && dek == null)
                {
                    MoveToParent(s, m_face, m_gameMaster.m_speed);
                    bFailed = false;
                    newP = s;
                }
                else if (tab != null && tab.IsEmpty() && this.m_name == Name.king)
                {
                    MoveToParent(tab, m_face, m_gameMaster.m_speed);
                    bFailed = false;
                    newP = tab;
                }
                else if (fam != null)
                {
                    if (fam.CanAdd(this))
                    {
                        this.MoveToParent(fam, CardScript.Face.recto, m_gameMaster.m_speed);
                        bFailed = false;
                        newP = fam;
                    }
                }
            }
            if (bFailed)
            {
                //move back
                ObjectBase parentObject = GetParent().GetComponent<ObjectBase>();
                MoveToParent(curP, m_face, m_gameMaster.m_moveBackSpeed);
                newP = curP;
            }
            else
            {
                GameMaster.eMoves m = GameMaster.eMoves.eNotSet;
                if(curP.IsDrawn())
                {
                    if(newP.IsDiscard())
                    {
                        m = GameMaster.eMoves.eDrawnToDiscard;
                    }
                    else
                    {
                        m = GameMaster.eMoves.eImpossibleMove;
                    }
                }
                else if(curP.IsCard())
                {
                    if(newP.IsCard() )
                    {
                        m = GameMaster.eMoves.eCardToCard;
                    }
                    else if(newP.Istableau())
                    {
                        m = GameMaster.eMoves.eCardToTableau;
                    }
                    else if (newP.IsFamily())
                    {
                        m = GameMaster.eMoves.eCardToFamily;
                    }
                }
                else if(curP.IsDiscard())
                {
                    if (newP.Istableau())
                    {
                        m = GameMaster.eMoves.eDiscardToTableau;
                    }
                    else if (newP.IsCard())
                    {
                        m = GameMaster.eMoves.eDiscardToCard;
                    }
                    else if (newP.IsFamily())
                    {
                        m = GameMaster.eMoves.eDiscardToFamily;
                    }
                }
                else if(curP.IsFamily())
                {
                    if(newP.IsCard())
                    {
                        m = GameMaster.eMoves.eFamilyToCard;
                    }
                    if (newP.IsFamily())
                    {
                        m = GameMaster.eMoves.eFamilyToFamily;
                    }
                }
                else if (curP.Istableau())
                {
                    if (newP.IsCard())
                    {
                        m = GameMaster.eMoves.eTableauToCard;
                    }
                    else if (newP.Istableau())
                    {
                        m = GameMaster.eMoves.eTableauToTableau;
                    }
                    else if (newP.IsFamily())
                    {
                        m = GameMaster.eMoves.eTableauToFamily;
                    }
                }
                else
                {
                    m = GameMaster.eMoves.eImpossibleMove;
                }
                m_gameMaster.OnMovePlayed(m);
            }
            SetHitable(true);
        }      
    }

    private bool CanAdd(CardScript s)
    {
        return this.m_color != s.m_color && this.m_value == (s.m_value - 1) && s.m_face == Face.recto;
    }
    /*
    public override Vector3 GetTargetPosition(ObjectBase b)
    {
        return new Vector3(0, 0, - m_gameMaster.m_cardSpace);
        //return new Vector3(m_targetPos.x, m_targetPos.y, m_targetPos.z - m_gameMaster.m_cardSpace);
    }
    */
    public void RestoreTo(ObjectBase newparent, Face f)
    {
        newparent.Add(this);
        m_targetPos = newparent.GetTargetPosition(this);
        this.transform.position =  GetParent().transform.position + m_targetPos;
        
        if(f != Face.verso)
        {
            m_mesh.transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
        }
        else
        {
            m_mesh.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        }
        m_face = f;
        
        //flipTo(f, true);
    }

    public void MoveToParent(ObjectBase newparent, Face f, float speed)
    {
        ObjectBase curP = GetParent();
        if (curP == null)
        {
            Debug.LogError(m_name + " has no parent. strange ?");
        }
        else
        {
            Tableau tab = curP.GetComponent<Tableau>();
            newparent.Add(this);
            m_targetPos = newparent.GetTargetPosition(this);
            flipTo(f, false);
            if (tab != null)
            {
                tab.FlipTopcard();
            }
            m_isMoving = true;
            m_speed = speed;      
        }
    }


    private void MoveWithMouse()
    {
        Vector3 f = m_gameMaster.transform.forward;
        Plane plane = new Plane(f, m_gameMaster.transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float point = 0f;
        if (plane.Raycast(ray, out point))
        {
            m_targetPos = ray.GetPoint(point);
            //m_targetPos = m_targetPos - GetParent().transform.position;
            //m_targetPos.z = m_gameMaster.m_zWhenMoving;
            m_targetPos = GetParent().transform.InverseTransformPoint(m_targetPos);
            m_isMoving = true;
            SetHitable(false);
        }
    }

    private void UpdateMove()
    {
        if(m_isMoving)
        {
            SetHitable(false);
            
            Vector3 parentPos = GetParent().transform.position + GetParent().transform.TransformVector(m_targetPos); ;
            //this.transform.position = Vector3.MoveTowards(this.transform.position, parentPos, m_speed * Time.deltaTime);
            this.transform.position = Vector3.MoveTowards(this.transform.position, parentPos, m_speed * Time.deltaTime);
            CardScript child = this.transform.GetComponentInChildren<CardScript>();
            //if( parentPos.Equals(this.transform.position) )

            if( Mathf.Abs(Vector3.SqrMagnitude(transform.position - parentPos)) < 0.0005)
            //if (transform.position == parentPos)
            {
                transform.position = parentPos;
                m_isMoving = false;
                SetHitable(true);
            }
        }
    }

    public void flipTo(Face f, bool force)
    {
        if(force || f != m_face)
        {
            m_face = f;
            Animation a = m_mesh.GetComponent<Animation>();
            if (m_face == Face.recto)
            {
                a.Play("cardFlipVersoToRecto", PlayMode.StopAll);
                //m_mesh.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            }
            else if (m_face == Face.verso)
            {
                a.Play("cardFlipRectoToVerso", PlayMode.StopAll);
                //m_mesh.transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
            }          
            m_symbolImage.gameObject.SetActive(m_face == Face.recto);
            m_value1Image.gameObject.SetActive(m_face == Face.recto);
            m_value2Image.gameObject.SetActive(m_face == Face.recto);
            m_symbol1Image.gameObject.SetActive(m_face == Face.recto);
            m_symbol2Image.gameObject.SetActive(m_face == Face.recto);
            m_recto.gameObject.SetActive(m_face == Face.recto);
            //m_verso.gameObject.SetActive(m_face == Face.verso);
            //GetParent().GetTargetPosition();
        }      
    }

    public void EndAnimation()
    {
    }

    public void Set(CardScript.Symbol symbol, CardScript.Name name, TexturePack tp)
    {
        m_face = Face.recto;
        m_name = name;
        m_symbol = symbol;
        m_value = (int)name;
        m_color = (symbol == Symbol.club || symbol == Symbol.spade) ? CardColor.black : CardColor.red;
        SetTexturePack(tp);
    }

    public void SetTexturePack(TexturePack tp)
    { 
        Material m;
        if (m_color != CardColor.black)
        {
            m = tp.m_redCardMaterial;
        }
        else
        {
            m = tp.m_blackCardMaterial;
        }
        Texture valueT = null;
        Texture symbolT = null;
        switch (m_value)
        {
            case 1:
                valueT = tp.m_texture1;
                break;
            case 2:
                valueT = tp.m_texture2;
                break;
            case 3:
                valueT = tp.m_texture3;
                break;
            case 4:
                valueT = tp.m_texture4;
                break;
            case 5:
                valueT = tp.m_texture5;
                break;
            case 6:
                valueT = tp.m_texture6;
                break;
            case 7:
                valueT = tp.m_texture7;
                break;
            case 8:
                valueT = tp.m_texture8;
                break;
            case 9:
                valueT = tp.m_texture9;
                break;
            case 10:
                valueT = tp.m_texture10;
                break;
            case 11:
                valueT = tp.m_textureJ;
                break;
            case 12:
                valueT = tp.m_textureQ;
                break;
            case 13:
                valueT = tp.m_textureK;
                break;

        }
        switch(m_symbol)
        {
            case Symbol.spade:
                symbolT = tp.m_textureSpade;
                break;
            case Symbol.heart:
                symbolT = tp.m_textureHeart;
                break;
            case Symbol.diamond:
                symbolT = tp.m_textureDiamond;
                break;
            case Symbol.club:
                symbolT = tp.m_textureClub;
                break;
        }
        m_value1Image.GetComponent<Renderer>().material = m;//.CopyPropertiesFromMaterial(m);
        m_value1Image.GetComponent<Renderer>().material.SetTexture("_MainTex", valueT);
        m_value2Image.GetComponent<Renderer>().material = m;//.CopyPropertiesFromMaterial(m);
        m_value2Image.GetComponent<Renderer>().material.SetTexture("_MainTex", valueT);

        m_symbolImage.GetComponent<Renderer>().material = m;//.CopyPropertiesFromMaterial(m);
        m_symbolImage.GetComponent<Renderer>().material.SetTexture("_MainTex", symbolT);
        m_symbol1Image.GetComponent<Renderer>().material = m;//.CopyPropertiesFromMaterial(m);
        m_symbol1Image.GetComponent<Renderer>().material.SetTexture("_MainTex", symbolT);
        m_symbol2Image.GetComponent<Renderer>().material = m;//.CopyPropertiesFromMaterial(m);
        m_symbol2Image.GetComponent<Renderer>().material.SetTexture("_MainTex", symbolT);

        m_recto.GetComponent<Renderer>().material = tp.m_rectoMaterial;
        m_verso.GetComponent<Renderer>().material = tp.m_versoMaterial;
    }

    private Transform GetObjectUnder()
    {
        int layerMask = 1 << 6;
        //Vector3 f = Vector3.forward;
        Vector3 f = m_gameMaster.transform.forward;
        Ray ray = new Ray(this.transform.position, f);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            return hit.transform;
        }
        return null;
    }
}
