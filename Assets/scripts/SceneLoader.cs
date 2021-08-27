using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{

    public string[] m_scenes;
    // Start is called before the first frame update

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

    // Start is called before the first frame update
    void Awake()
    {
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

    public void LoadScene(bool loadGameWorld, bool loadMainMenu, bool loadScoreMenu, string level = null)
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
        PlayerPrefs.SetString(LAST_SCENE_NAME, level);
        SceneManager.LoadScene(level);
    }
}
