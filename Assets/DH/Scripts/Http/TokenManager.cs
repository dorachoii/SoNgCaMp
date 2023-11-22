using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenManager : MonoBehaviour
{
    const string tokenKey = "UserToken";

    private static string token; 
    public static string Token
    {
        get { return PlayerPrefs.GetString(tokenKey);  }
        set {
            token = value;
            PlayerPrefs.SetString(tokenKey, value); //set �ɶ� ���� ����
            }
    }
    private void Awake()
    {
        //����� ��ū �ҷ�����
        Token = PlayerPrefs.GetString(tokenKey); //�����Ҷ� ��ū �ҷ����� 
    }
}
