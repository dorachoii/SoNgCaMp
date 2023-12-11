using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetHostMenu : MonoBehaviour
{
    private void Awake()
    {
        string host = PlayerPrefs.GetString("HOST");
        HttpController.default_host = host;
        HttpInfo.defaultHost = host + "/";
    }
    void Start()
    {
        
    }


    void Update()
    {
        
    }
}
