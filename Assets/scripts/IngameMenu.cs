using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngameMenu : MonoBehaviour
{
    public GameMaster m_gameMaster;
    public Button m_new;
    public Button m_undo;
    public Button m_redo;
    public Button m_automate;
    public Button m_options;
    public Text m_score;
    public Text m_turn;

    private void OnMouseUpAsButton()
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {
        m_new.onClick.AddListener(m_gameMaster.NewGame);
        m_undo.onClick.AddListener(m_gameMaster.Undo);
        m_redo.onClick.AddListener(m_gameMaster.Redo);
        m_automate.onClick.AddListener(m_gameMaster.Automate);
        m_options.onClick.AddListener(m_gameMaster.OpenOptionsMenu);
    }

    // Update is called once per frame
    void Update()
    {
        m_automate.gameObject.SetActive(m_gameMaster.CanAutomate());
        m_undo.gameObject.SetActive(m_gameMaster.CanUndo());
        m_redo.gameObject.SetActive(m_gameMaster.CanRedo());
        m_score.text = m_gameMaster.GetScrore().ToString() + "pts";
        m_turn.text = m_gameMaster.GetTurn().ToString();
    }
}
