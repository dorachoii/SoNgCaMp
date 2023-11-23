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
public struct LoginDTO2 { 
    public LoginResponseDTO authority;
}

[Serializable]
public struct LoginResponseDTO {
    public int userNo;
    public string userNickname;
    public int musician;
    public int sessionType;
    public int genre;
    public int mood;
    public int characterType;

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
public struct SongDTO 
{
    public string needSession;
    public List<SongUserDTO> participantList;
    public string songArtist;
    public string songTitle;
    public SongDTO(string needSession,List<SongUserDTO> participantList,string songArtist,string songTitle) {
        this.needSession = needSession;
        this.participantList = participantList;
        this.songArtist = songArtist;
        this.songTitle = songTitle;
    }
}
[Serializable]
public struct SongUserDTO 
{
    public int sessionType;
    public int userNo;

}

[Serializable]
public struct FileDTO
{
    public int fileNo;
    public int[] createdDate;
    public int[] modifiedDate;
    public string songTitle;
    public string songArtist;
    public string needSession;
    public string imageFileUrl;
    public string musicFileUrl;
    public string midFileUrl;
}

[Serializable]
public struct FileListDTO {
    public List<FileDTO> files;
}