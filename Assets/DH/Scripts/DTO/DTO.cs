using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DTO { 

}
[Serializable]
public struct ResponceDTO<T>
{
    public int httpStatus;
    public string message;
    public T results;

}
[Serializable]
public struct LoginResponseDTO : IToString{
    public int userNo;
    public string userNickname;
    public int musician;
    public int sessionType;
    public int genre;
    public int mood;
    public int chracterType;
    public Color hexStringCloth;
    public Color hexStringFace;
    public Color hexStringSkin;
    public Color hexStringRibbon;
    public bool isCrownOn;
    public bool isGlassOn;
    public bool isBagOn;
    public bool isCapOn;
    public AuthorityDTO authority;

    public string toString()
    {
        return null;
    }
}
[Serializable]
public struct AuthorityDTO : IToString{
    public string accessToken;
    public string refreshToken;

    public string toString()
    {
        return accessToken + "refreshToken : " + refreshToken;
    }
}

public interface IToString {
    string toString();
}

[Serializable]
public struct Info2 : IToString
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
public struct Info3 : IToString
{
    public int sessionType;
    public int userNo;

    public string toString()
    {
        return "" + sessionType + userNo;
    }
}