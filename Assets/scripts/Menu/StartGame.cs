using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public string[] m_scenes;
    // Start is called before the first frame update
    void Start()
    {

        Debug.Log("Start");
        Debug.Log("nb scenes : " + m_scenes.Length);
        if(m_scenes == null)
        {
            Debug.Log($"NO SCENE !");
        }
        for(int i=0; i<m_scenes.Length; i++)
        {
            Debug.Log($"Scene {i} : {m_scenes[i]}");
        }
        int idx = Random.Range(0, m_scenes.Length);
        Debug.Log($"Chosen index  {idx}, loading {m_scenes[idx]}");
        SceneManager.LoadScene(m_scenes[idx]);
    }
}
