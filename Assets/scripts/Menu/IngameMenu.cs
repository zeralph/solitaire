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
    public SolitaireText m_PanelAskYesNoText;
    public SolitaireText m_score;
    public SolitaireText m_turn;
    public GameObject PanelWin;
    public Button m_winQuit;
    public SolitaireText m_winScore;
    public OptionsMenu m_optionsMenu;

    // Start is called before the first frame update
    void Start()
    {
        PanelAskYesNo.SetActive(false);
        PanelWin.SetActive(false);
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
        m_winQuit.onClick.AddListener(OnQuit);
        m_optionsMenu.BindCloseButton(m_gameMaster.CloseOptionsMenu);
    }



    // Update is called once per frame
    void Update()
    {
        m_automate.gameObject.SetActive(m_gameMaster.CanAutomate());
        m_undo.gameObject.SetActive(m_gameMaster.CanUndo());
        m_redo.gameObject.SetActive(m_gameMaster.CanRedo());
        m_cheat.gameObject.SetActive(m_gameMaster.CanCheat());
        m_score.SetText($"  {m_gameMaster.GetScrore().ToString()} pts  ");
        m_turn.SetText(m_gameMaster.GetTurn().ToString());
        //m_continue.gameObject.SetActive(m_gameMaster.HasSave());
    }

    public void OpenWin(int score)
    {
        PanelIngame.SetActive(false);
        PanelWin.SetActive(true);
        m_winScore.SetText($"{score} pts");
    }


    private void OnAskRestart()
    {
        m_PanelAskYesNo_ButtonYes.onClick.RemoveAllListeners();
        m_PanelAskYesNo_ButtonNo.onClick.RemoveAllListeners();
        m_PanelAskYesNoText.SetText("Restart ?");
        m_PanelAskYesNo_ButtonYes.onClick.AddListener(OnNewGame);
        m_PanelAskYesNo_ButtonNo.onClick.AddListener(OnCancelPanelYesNo);
        m_gameMaster.Pause(true);
        PanelAskYesNo.SetActive(true);
        PanelIngame.SetActive(false);
    }

    private void OnCheat()
    {
        m_cheat.gameObject.SetActive(false);
        //m_gameMaster.Cheat();
    }

    private void OnAskQuit()
    {
        m_PanelAskYesNo_ButtonYes.onClick.RemoveAllListeners();
        m_PanelAskYesNo_ButtonNo.onClick.RemoveAllListeners();
        m_PanelAskYesNoText.SetText("Quit ?");
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
        //sl.LoadScene("StartScene");
        SceneLoader sl = FindObjectsOfType<SceneLoader>()[0];
        sl.LoadScene(false, true, false);
    }
}
