using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogManager : MonoBehaviour
{
    public static LogManager instance;
    public Text text;
    private void Awake()
    {
        instance = this;
        //text = GetComponent<Text>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Log(string s) {
        text.text += " \n : " + s;
    } 
}
