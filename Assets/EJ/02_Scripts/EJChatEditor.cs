using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EJChatMangager))]

public class EJChatEditor : Editor
{
    EJChatMangager chatManager;
    string text;

    private void OnEnable()
    {
        chatManager = target as EJChatMangager;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.BeginHorizontal();
        text = EditorGUILayout.TextArea(text);

        if (GUILayout.Button("������", GUILayout.Width(60)) && text.Trim() != "")
        {
            chatManager.Chat(true, text, "��", null);
            text = "";
            GUI.FocusControl(null);
        }

        if (GUILayout.Button("�ޱ�", GUILayout.Width(60)) && text.Trim() != "")
        {
            //chatManager.Chat(true, text, "����", Resources.Load<Texture>("ETC/gorani"));
            text = "";
            GUI.FocusControl(null);
        }

        EditorGUILayout.EndHorizontal();
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
