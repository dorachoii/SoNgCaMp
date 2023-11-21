using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Events;

public class ChatService : MonoBehaviour
{
    //�Է�â 
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
    //����â 
    RectTransform content;
    [SerializeField]
    RectTransform contentParent;

    [SerializeField]
    GameObject textObj;
    //�Է� �Լ�
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

        //�߰��ɶ� 
        //content.position += new Vector3(0,textObj.transform.localScale.y);

        //��ũ�� ���� Height��
        //���� ���� ��ġ�� content�� ��ġ�� �ɰŰ�����.

        //�߰��� ���
        StartCoroutine(AdjusttView());
        
    }

    //������ �ٷ� �����ȵ�
    public IEnumerator AdjusttView() {
        //�� ������ �Ŀ� ����
        yield return null;
        float new_height = content.rect.height;
        if (new_height > contentParent.rect.height)
        {
            //ũ�ٴ°��� �ʰ����� �ִٴ°�.
            content.anchoredPosition = Vector2.up * (new_height - contentParent.rect.height);
        }


    }
    public UnityEvent<string> chatevent;
}
