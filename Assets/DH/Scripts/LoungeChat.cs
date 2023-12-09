using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
public class LoungeChat : MonoBehaviour
{
    public static LoungeChat instance;
    private void Awake()
    {
        instance = this;
    }
    public Action<string> onSendMessage;

    [SerializeField] TMP_InputField input_field;

    public void OnSend() {
        onSendMessage?.Invoke(input_field.text);
    }
}
