using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ChatText : MonoBehaviour
{
    [SerializeField]
    Text text;
    [SerializeField]
    TextMeshProUGUI tmp_text;
    //is legercy or tmp
    bool useTmp;

    private void Awake()
    {
        useTmp = tmp_text != null ? true : false;
        if (text == null && tmp_text == null) {
            throw new System.Exception();
            //Error!
        }
    }

    public void SetText(string s)
    {
        if (useTmp) { tmp_text.text = s; }
        else { text.text = s; }
    }
}
