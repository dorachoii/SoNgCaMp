using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class EJSceneManager : MonoBehaviour
{
    static EJSceneManager instance;

    public int currSceneIdx;

    public static EJSceneManager GetInstance()
    {
        if(instance == null)
        {
            GameObject go = new GameObject("EJSceneManager");
            go.AddComponent<EJSceneManager>();
        }
        return instance;
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        currSceneIdx = SceneManager.GetActiveScene().buildIndex;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            currSceneIdx++;
            SceneManager.LoadScene(currSceneIdx);
        }
    }

    public void LoadScene(int sceneIdx)
    {
        currSceneIdx = sceneIdx;
        SceneManager.LoadScene(sceneIdx);
    }
}
