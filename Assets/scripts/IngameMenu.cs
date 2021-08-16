using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class IngameMenu : MonoBehaviour
{
    public GameMaster m_gameMaster;
    public Button m_new;
    public Button m_undo;
    public Button m_redo;
    public Button m_automate;
    public Button m_options;
    public Button m_quit;
    public Button m_boom;
    public Button m_cheat;
    public GameObject PanelAskYesNo;
    public GameObject PanelIngame;
    public Button m_PanelAskYesNo_ButtonYes;
    public Button m_PanelAskYesNo_ButtonNo;
    public Text m_PanelAskYesNoText;
    public Text m_score;
    public Text m_turn;

    private void OnMouseUpAsButton()
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {
        PanelAskYesNo.SetActive(false);
        PanelIngame.SetActive(true);
        m_new.onClick.AddListener(OnAskRestart);
        m_undo.onClick.AddListener(m_gameMaster.Undo);
        m_redo.onClick.AddListener(m_gameMaster.Redo);
        m_automate.onClick.AddListener(m_gameMaster.Automate);
        m_options.onClick.AddListener(m_gameMaster.OpenOptionsMenu);
        //m_continue.onClick.AddListener(m_gameMaster.LoadFromSave);
        m_boom.onClick.AddListener(m_gameMaster.GetComponent<CardsCreator>().BoomCards);
        m_quit.onClick.AddListener(OnAskQuit);
        m_cheat.onClick.AddListener(OnCheat);
    }



    // Update is called once per frame
    void Update()
    {
        m_automate.gameObject.SetActive(m_gameMaster.CanAutomate());
        m_undo.gameObject.SetActive(m_gameMaster.CanUndo());
        m_redo.gameObject.SetActive(m_gameMaster.CanRedo());
        m_cheat.gameObject.SetActive(m_gameMaster.CanCheat());
        m_score.text = m_gameMaster.GetScrore().ToString() + "pts";
        m_turn.text = m_gameMaster.GetTurn().ToString();
        //m_continue.gameObject.SetActive(m_gameMaster.HasSave());
    }

    private void OnAskRestart()
    {
        m_PanelAskYesNo_ButtonYes.onClick.RemoveAllListeners();
        m_PanelAskYesNo_ButtonNo.onClick.RemoveAllListeners();
        m_PanelAskYesNoText.text = "Restart ?";
        m_PanelAskYesNo_ButtonYes.onClick.AddListener(OnNewGame);
        m_PanelAskYesNo_ButtonNo.onClick.AddListener(OnCancelPanelYesNo);
        m_gameMaster.Pause(true);
        PanelAskYesNo.SetActive(true);
        PanelIngame.SetActive(false);
    }

    private void OnCheat()
    {
        m_cheat.gameObject.SetActive(false);
        m_gameMaster.Cheat();
    }

    private void OnAskQuit()
    {
        m_PanelAskYesNo_ButtonYes.onClick.RemoveAllListeners();
        m_PanelAskYesNo_ButtonNo.onClick.RemoveAllListeners();
        m_PanelAskYesNoText.text = "Quit ?";
        m_PanelAskYesNo_ButtonYes.onClick.AddListener(OnQuit);
        m_PanelAskYesNo_ButtonNo.onClick.AddListener(OnCancelPanelYesNo);
        m_gameMaster.Pause(true);
        PanelAskYesNo.SetActive(true);
        PanelIngame.SetActive(false);
    }

    private void OnNewGame()
    {
        PanelAskYesNo.SetActive(false);
        PanelIngame.SetActive(true);
        m_gameMaster.Pause(false);
        m_gameMaster.NewGame();
    }

    private void OnCancelPanelYesNo()
    {
        PanelAskYesNo.SetActive(false);
        PanelIngame.SetActive(true);
        m_gameMaster.Pause(false);
    }

    private void OnQuit()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
