using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SolitaireText : MonoBehaviour
{
    public Text m_text;
    public void SetText(string s)
    {
        m_text.text = s;
    }
}
