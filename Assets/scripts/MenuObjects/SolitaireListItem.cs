using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SolitaireListItem : MonoBehaviour
{
    public Text m_text;
    public string m_value;

    public void Set(string text, string value)
    {
        m_text.text = text;
        m_value = value;
    }
    public string GetText()
    {
        return m_text.text;
    }

    public string GetValue()
    {
        if(string.IsNullOrEmpty(m_value))
        {
            Debug.LogWarning($"[SolitaireListItem] : value for {this.name} is null. Using text {m_text.text}");
            return m_text.text;
        }
        return m_value;
    }
}

