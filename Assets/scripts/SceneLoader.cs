using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{

    public string[] m_scenes;
    // Start is called before the first frame update

    public string[] m_packs;

    public GameObject m_gameWorld;
    public GameObject m_mainMenu;
    public GameObject m_scoreMenu;

    public bool m_isStartGameScene = false;

    public static readonly string SCENE_TYPE_KEY = "nextSceneType";
    public static readonly string MAIN_MENU = "mainMenu";
    public static readonly string GAME_WORLD = "gameWorld";
    public static readonly string SCORES_MENU = "scoresMenu";
    public static readonly string LAST_SCENE_NUMBER = "lastSceneNumber";
    public static readonly string LAST_SCENE_NAME = "lastSceneName";
    public static readonly string LAST_TEXTURE_PACK_NAME = "lastPackName";
    public static readonly string LAST_TEXTURE_PACK_NUMBER = "lastPackNumber";

    private TexturePack m_texturePack;

    // Start is called before the first frame update
    void Awake()
    {
        m_texturePack = null;
        bool bLoadWorld = false;
        bool bLoadMainMenu = false;
        bool bLoadScores = false;
        if(m_isStartGameScene)
        {
            PlayerPrefs.DeleteKey(SCENE_TYPE_KEY);
            LoadScene(false, true, false);
        }
        else if(PlayerPrefs.HasKey(SCENE_TYPE_KEY))
        {
            bLoadMainMenu = PlayerPrefs.GetString(SCENE_TYPE_KEY) == MAIN_MENU;
            bLoadWorld = PlayerPrefs.GetString(SCENE_TYPE_KEY) == GAME_WORLD;
            bLoadScores = PlayerPrefs.GetString(SCENE_TYPE_KEY) == SCORES_MENU;
        }
        else
        {
            bLoadMainMenu = true;
        }
        if (bLoadMainMenu)
        {
            Instantiate(m_mainMenu, Vector3.zero, Quaternion.identity);
        }
        if (bLoadWorld)
        {
            Instantiate(m_gameWorld, Vector3.zero, Quaternion.identity);
        }
        if(bLoadScores)
        {
            Instantiate(m_scoreMenu, Vector3.zero, Quaternion.identity);
        }
    }

    public TexturePack GetPack()
    {
        if(m_texturePack == null)
        {
            string s = PlayerPrefs.GetString(LAST_TEXTURE_PACK_NAME);
            if(string.IsNullOrEmpty(s))
            {
                s = m_packs[0];
                PlayerPrefs.SetInt(LAST_TEXTURE_PACK_NUMBER, 0);
                PlayerPrefs.SetString(LAST_TEXTURE_PACK_NAME, s);
            }
            m_texturePack = Resources.Load<TexturePack>($"Packs/{s}/{s}");
        }
        return m_texturePack;
    }

    public void LoadScene(bool loadGameWorld, bool loadMainMenu, bool loadScoreMenu, string level = null, string pack = null)
    {
        if(loadScoreMenu)
        {
            PlayerPrefs.SetString(SCENE_TYPE_KEY, SCORES_MENU);
        }
        if(loadGameWorld)
        {
            PlayerPrefs.SetString(SCENE_TYPE_KEY, GAME_WORLD);
        }
        if(loadMainMenu)
        {
            PlayerPrefs.SetString(SCENE_TYPE_KEY, MAIN_MENU);
        }
        if(string.IsNullOrEmpty(level))
        {
            int idx = 0;
            if (PlayerPrefs.HasKey(LAST_SCENE_NUMBER))
            {
                idx = (PlayerPrefs.GetInt(LAST_SCENE_NUMBER) + 1) % m_scenes.Length;
            }
            PlayerPrefs.SetInt(LAST_SCENE_NUMBER, idx);
            level = m_scenes[idx];
        }
        if (string.IsNullOrEmpty(pack))
        {
            int idx = 0;
            if (PlayerPrefs.HasKey(LAST_TEXTURE_PACK_NUMBER))
            {
                idx = (PlayerPrefs.GetInt(LAST_TEXTURE_PACK_NUMBER) + 1) % m_packs.Length;
            }
            PlayerPrefs.SetInt(LAST_TEXTURE_PACK_NUMBER, idx);
            pack = m_packs[idx];
        }
        PlayerPrefs.SetString(LAST_TEXTURE_PACK_NAME, pack);
        PlayerPrefs.SetString(LAST_SCENE_NAME, level);
        SceneManager.LoadScene(level);
    }
}
