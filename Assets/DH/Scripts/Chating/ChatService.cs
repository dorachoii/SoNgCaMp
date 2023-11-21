using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Events;

public class ChatService : MonoBehaviour
{
    //입력창 
    [SerializeField]
    InputField field;
    [SerializeField]
    TMP_InputField tmp_field;

    //Using TMP?
    bool useTmp = false;

    private void Awake()
    {
        useTmp = tmp_field != null ? true : false;
        if (field == null && tmp_field == null)
        {
            throw new System.Exception();
            //Error!
        }
        
    }

    [SerializeField]
    //보드창 
    RectTransform content;
    [SerializeField]
    RectTransform contentParent;

    [SerializeField]
    GameObject textObj;
    //입력 함수
    public void Input() {
        string s = !useTmp ? field.text : tmp_field.text;
        //get InputField String
        CreateChat(s);
        if (useTmp)
            tmp_field.text = null;
        else
            field.text = null;
    }

    public void Input2() {
        string s = !useTmp ? field.text : tmp_field.text;
        chatevent.Invoke(s);
        if (useTmp)
        {
            tmp_field.text = null;
            tmp_field.Select();

        }
        else {
            field.text = null;
            field.Select();
        }
            
    }

    //string
    public void CreateChat(string s) {
        GameObject tobj = Instantiate(textObj, content);
        ChatText chat_txt = tobj.GetComponent<ChatText>();
        chat_txt.SetText(s);

        //추가될때 
        //content.position += new Vector3(0,textObj.transform.localScale.y);

        //스크롤 뷰의 Height와
        //나의 차의 위치가 content의 위치가 될거같은데.

        //추가가 됬고
        StartCoroutine(AdjusttView());
        
    }

    //생성시 바로 생성안됨
    public IEnumerator AdjusttView() {
        //한 프레임 후에 진행
        yield return null;
        float new_height = content.rect.height;
        if (new_height > contentParent.rect.height)
        {
            //크다는것은 초과분이 있다는것.
            content.anchoredPosition = Vector2.up * (new_height - contentParent.rect.height);
        }


    }
    public UnityEvent<string> chatevent;
}
