using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneComposer : MonoBehaviour
{
    public float m_deltaW = 0.1f;
    public float m_deltaH = 0.1f;
    public Transform m_targetMesh;
    public Transform m_target;
    public bool Recalc = false;

    // Start is called before the first frame update
    void Start()
    {
        Compute();
    }

    // Update is called once per frame
    void Update()
    {
        if(Recalc)
        {
            Recalc = false;
            Compute();
        }
    }

    void Compute()
    {
        Vector3 b = m_targetMesh.GetComponent<MeshFilter>().mesh.bounds.size;
        b.x *= m_targetMesh.transform.lossyScale.x;
        b.y *= m_targetMesh.transform.lossyScale.y;
        b.z *= m_targetMesh.transform.lossyScale.z;
        float width = b.x / 1;
        float height = b.z / 1;
        Vector3 targetPosition = m_target.transform.position;
        Camera c = this.GetComponent<Camera>();
        float d = (targetPosition - c.transform.position).magnitude;
        float frustumHeight = 2.0f * d * Mathf.Tan(c.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float frustumWidth = frustumHeight * c.aspect;


        float desiredFrustrumHeight = (width + m_deltaW) / c.aspect;
        d = desiredFrustrumHeight / (2.0f * Mathf.Tan(c.fieldOfView * 0.5f * Mathf.Deg2Rad));
        Vector3 newBoardPos = c.transform.position;
        newBoardPos.y += (desiredFrustrumHeight - height) / 2.0f;
        newBoardPos += d * m_target.forward;
        //newCamPos.y = targetPosition.y;
        m_target.transform.position = newBoardPos;
    }
}
