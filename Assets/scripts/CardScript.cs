using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CardScript : ObjectBase
{
    public enum CardColor
    {
        notSet=0,
        red=1,
        black=2,
        __MAX__
    }
    public enum Symbol
    {
        notSet=0,
        spade=1,
        heart=2,
        diamond=3,
        club=4,
        __MAX__
    }
    public enum Face
    {
        notSet=0,
        recto=1,
        verso=2
    }
    public enum Figure
    {
        notSet=0,
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
    public AudioClip m_cardDeal;
    public AudioClip m_cardFlip;
    private Vector3 m_targetPos;
    private bool m_isMoving;
    public float m_flipSpeed = 100.0f;
    private float m_speed;
    //[ReadOnly]
    public int m_value;
    //[ReadOnly]
    public Figure m_figure;
    //[ReadOnly]
    public CardColor m_color;
    //[ReadOnly]
    public Symbol m_symbol;
    public Face m_face;
    private bool m_moveWithMouse;
    private ObjectBase m_lastObjectUnder;
    private GameMaster.eMoves m_lastMove;
    private ObjectBase m_lastParent;

    public override void Awake()
    {
        base.Awake();
        m_targetPos = transform.position;
        m_isMoving = false;
        m_moveWithMouse = false;
        m_speed = 0f;
        OutLine(false);
        m_lastMove = GameMaster.eMoves.eNotSet;
        m_lastParent = null;
    }


    void Update()
    {
        if(!GetGameMaster())
        {
            return;
        }
        if (GetGameMaster().IsPaused())
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

    public void EnablePhysic(bool bWithGravity)
    {
        m_mesh.GetComponent<Animation>().enabled = false;
        Rigidbody rb = this.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        if(bWithGravity)
        {
            rb.useGravity = true;
        }
    }

    public bool IsInFrustrum()
    {
        return m_recto.GetComponent<Renderer>().isVisible || m_verso.GetComponent<Renderer>().isVisible;
    }

    public void DisablePhysic()
    {
        m_mesh.GetComponent<Animation>().enabled = true;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Rigidbody>().useGravity = false;
    }

    public void AddForceAndTorque(Vector3 f, Vector3 t)
    {
        Rigidbody rb = this.GetComponent<Rigidbody>();
        rb.AddForce(f);
        rb.AddTorque(t, ForceMode.Impulse);
    }

    public void Boom()
    {
        EnablePhysic(false);
        float x = Random.Range(-1f, 1f);
        float y = Random.Range(-1f, 1f);
        float z = Random.Range(-1f, 0f);
        float f = GetGameMaster().m_boomFactor;
        Vector3 t = new Vector3(x, y, z);
        Vector3 v = Camera.main.transform.position - transform.position;
        v.Normalize();
        v.x *= f;// * x;
        v.y *= f;// * y;
        v.z *= f;// * z;
        AddForceAndTorque(v, t);
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
            m_speed = GetGameMaster().m_mouseSpeed;
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
            bool bFailed = true;
            ObjectBase curP = GetParent();
            ObjectBase newP = m_lastObjectUnder;
            //GameMaster.eMoves move = GameMaster.eMoves.eNullMove;
            if (newP != null)
            {
                CardScript s = newP.GetComponentInParent<CardScript>();
                Tableau tab = newP.GetComponentInParent<Tableau>();
                FamilyPile fam = newP.GetComponentInParent<FamilyPile>();
                DeckScript dek = newP.GetComponentInParent<DeckScript>();
                if (fam != null)
                {
                    if (fam.CanAdd(this))
                    {
                        this.MoveToParent(fam, CardScript.Face.recto, GetGameMaster().m_speed, false);
                        bFailed = false;
                        newP = fam;
                    }
                }
                else if (tab != null && tab.IsEmpty() && this.m_figure == Figure.king)
                {
                    MoveToParent(tab, m_face, GetGameMaster().m_speed, false);
                    bFailed = false;
                    newP = tab;
                }
                else if (s && s.CanAdd(this) && dek == null)    //card is not in deck
                {
                    MoveToParent(s, m_face, GetGameMaster().m_speed, false);
                    bFailed = false;
                    newP = s;
                }
            }
            if (bFailed)
            {
                //move back
                ObjectBase parentObject = GetParent().GetComponent<ObjectBase>();
                MoveToParent(curP, m_face, GetGameMaster().m_moveBackSpeed, false);
                newP = curP;
            }
            else
            {
                //ComputeMoveType(curP, newP);
            }
            SetHitable(true);
        }      
    }

    private GameMaster.eMoves ComputeMoveType(ObjectBase curP, ObjectBase newP)
    {
        GameMaster.eMoves m = GameMaster.eMoves.eNotSet;
        if (curP.IsDrawn())
        {
            if (newP.IsDiscard())
            {
                m = GameMaster.eMoves.eDrawnToDiscard;
            }
            else
            {
                m = GameMaster.eMoves.eImpossibleMove;
            }
        }
        else if (curP.IsCard())
        {
            if (newP.IsCard())
            {
                m = GameMaster.eMoves.eCardToCard;
            }
            else if (newP.Istableau())
            {
                m = GameMaster.eMoves.eCardToTableau;
            }
            else if (newP.IsFamily())
            {
                m = GameMaster.eMoves.eCardToFamily;
            }
        }
        else if (curP.IsDiscard())
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
        else if (curP.IsFamily())
        {
            if (newP.IsCard())
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
        return m;
    }

    private bool CanAdd(CardScript s)
    {
        return this.m_color != s.m_color && this.m_value == (s.m_value + 1) && s.m_face == Face.recto && this.m_face == Face.recto;
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

    public void MoveToParent(ObjectBase newparent, Face f, float speed, bool bImmediateFlip)
    {
        m_lastParent = GetParent();
        if (m_lastParent == null)
        {
            Debug.LogError(m_figure + " has no parent. strange ?");
        }
        else
        {
            
            newparent.Add(this);
            m_targetPos = newparent.GetTargetPosition(this);
            this.transform.rotation = newparent.transform.rotation;
            flipTo(f, bImmediateFlip);
            m_isMoving = true;
            m_speed = speed;
            AudioSource audio = Camera.main.GetComponent<AudioSource>();
            audio.clip = m_cardDeal;
            //audio.
            //audio.Play();
            m_lastMove = ComputeMoveType(m_lastParent, newparent);
        }
    }


    private void MoveWithMouse()
    {
        Vector3 f = GetGameMaster().transform.forward;
        Plane plane = new Plane(f, GetGameMaster().transform.position);
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
        if(m_lastObjectUnder != null)
        {
            m_lastObjectUnder.OutLine(false);
        }
        if (m_face == Face.recto)
        {
            m_lastObjectUnder = GetObjectUnder();
        }
        if (m_lastObjectUnder != null)
        {
            m_lastObjectUnder.OutLine(true);
        }
    }

    private void UpdateMove()
    {
        if(m_isMoving)
        {
            SetHitable(false);
            
            Vector3 parentPos = GetParent().transform.position + GetParent().transform.TransformVector(m_targetPos);
            this.transform.position = Vector3.MoveTowards(this.transform.position, parentPos, m_speed * Time.deltaTime);
            CardScript child = this.transform.GetComponentInChildren<CardScript>();
            if( !m_moveWithMouse && Mathf.Abs(Vector3.SqrMagnitude(transform.position - parentPos)) < 0.0005)
            {
                EndMove(parentPos);
            }
        }
    }

    private void EndMove(Vector3 finalPos)
    {                              
        transform.position = finalPos;
        m_isMoving = false;
        SetHitable(true);
        if (m_lastMove != GameMaster.eMoves.eNotSet && m_lastMove != GameMaster.eMoves.eImpossibleMove)
        {
            GetGameMaster().OnMovePlayed(m_lastMove);
            m_lastMove = GameMaster.eMoves.eNotSet;
        }
        m_lastParent = GetParent();
    }

    public void Swap(CardScript cs)
    {
        Debug.Assert(this.GetNbChildCards() == 0, "bad1");
        Debug.Assert(cs.GetNbChildCards() == 0, "bad2");
        ObjectBase thisParent = this.GetParent();
        ObjectBase otherParent = cs.GetParent();
        Vector3 thisPos = this.transform.position;
        Vector3 otherPos = cs.transform.position;
        Face thisFace = this.m_face;
        Face otherFace = cs.m_face;
        int thisindex = this.transform.GetSiblingIndex();
        int otherIndex = cs.transform.GetSiblingIndex();
        thisParent.Add(cs);
        otherParent.Add(this);
        this.transform.SetSiblingIndex(otherIndex);
        cs.transform.SetSiblingIndex(thisindex);
        this.transform.position = otherPos;
        cs.transform.position = thisPos;
        this.flipTo(otherFace, true);
        cs.flipTo(thisFace, true);
    }

    public void flipTo(Face f, bool force)
    {
        if (force)
        {
            m_face = f;
            if (m_face == Face.recto)
            {
                m_mesh.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            }
            else if (m_face == Face.verso)
            {
                m_mesh.transform.localRotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
            }
            m_symbolImage.gameObject.SetActive(m_face == Face.recto);
            m_value1Image.gameObject.SetActive(m_face == Face.recto);
            m_value2Image.gameObject.SetActive(m_face == Face.recto);
            m_symbol1Image.gameObject.SetActive(m_face == Face.recto);
            m_symbol2Image.gameObject.SetActive(m_face == Face.recto);
            m_recto.gameObject.SetActive(m_face == Face.recto);
        }
        else if (/*force ||*/ f != m_face)
        {
            m_face = f;
            Animation a = m_mesh.GetComponent<Animation>();
            if (m_face == Face.recto)
            {
                a.Play("cardFlipVersoToRecto", PlayMode.StopAll);
            }
            else if (m_face == Face.verso)
            {
                a.Play("cardFlipRectoToVerso", PlayMode.StopAll);
            }   
            m_symbolImage.gameObject.SetActive(m_face == Face.recto);
            m_value1Image.gameObject.SetActive(m_face == Face.recto);
            m_value2Image.gameObject.SetActive(m_face == Face.recto);
            m_symbol1Image.gameObject.SetActive(m_face == Face.recto);
            m_symbol2Image.gameObject.SetActive(m_face == Face.recto);
            m_recto.gameObject.SetActive(m_face == Face.recto);
            AudioSource audio = Camera.main.GetComponent<AudioSource>();
            if(audio != null)
            {
                audio.clip = m_cardFlip;
                audio.Play();
            }
        }      
    }

    public void FixFlipAnimation()
    {

    }

    public void EndAnimation()
    {
    }

    public void Set(CardScript.Symbol symbol, CardScript.Figure name, TexturePack tp)
    {
        m_face = Face.recto;
        m_figure = name;
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

    private ObjectBase GetObjectUnder()
    {
        int layerMask = 1 << 6;
        Vector3 s = this.transform.GetComponent<BoxCollider>().size;
        Vector3 f = GetGameMaster().transform.forward;
        Vector3 p = this.transform.position;
        p.y += s.y / 2;
        Ray r1 = new Ray(p, f);
        p.y -= s.y;
        Ray r2 = new Ray(p, f);
        RaycastHit hit;
        if (Physics.Raycast(r1, out hit, Mathf.Infinity, layerMask))
        {
            Transform t = hit.transform;
            return t.GetComponentInParent<ObjectBase>();
        }
        if (Physics.Raycast(r2, out hit, Mathf.Infinity, layerMask))
        {
            Transform t = hit.transform;
            return t.GetComponentInParent<ObjectBase>();
        }
        return null;
    }
}
