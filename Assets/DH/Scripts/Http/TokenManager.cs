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
            PlayerPrefs.SetString(tokenKey, value); //set 될때 새로 저장
            }
    }
    private void Awake()
    {
        //저장된 토큰 불러오기
        Token = PlayerPrefs.GetString(tokenKey); //시작할때 토큰 불러오기 
    }
}
