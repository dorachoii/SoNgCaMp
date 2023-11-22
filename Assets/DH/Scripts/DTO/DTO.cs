using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DTO { 

}
[Serializable]
public struct ResponseDTO<T>
{
    public int httpStatus;
    public string message;
    public T results;

}

[Serializable]
public struct LoginDTO
{
    public LoginResponseDTO loginResponse;
    
}


[Serializable]
public struct LoginResponseDTO {
    public int userNo;
    public string userNickname;
    public int musician;
    public int sessionType;
    public int genre;
    public int mood;
    public int chracterType;

    public string hexStringCloth;
    public string hexStringFace;
    public string hexStringSkin;
    public string hexStringRibbon;

    public bool isCrownOn;
    public bool isGlassOn;
    public bool isBagOn;
    public bool isCapOn;
    public List<AuthorityDTO> authority;

}
[Serializable]
public struct AuthorityDTO {
    public string accessToken;
    public string refreshToken;

}


[Serializable]
public struct Info2 
{
    public string needSession;
    public List<Info3> particlpantList;
    public string songArtist;
    public string songTitle;

    public string toString()
    {
        return needSession + songArtist + songTitle;
    }
}
[Serializable]
public struct Info3 
{
    public int sessionType;
    public int userNo;

    public string toString()
    {
        return "" + sessionType + userNo;
    }
}