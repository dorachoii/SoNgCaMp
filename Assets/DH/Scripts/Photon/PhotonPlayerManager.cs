using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonPlayerManager : MonoBehaviour
{
    [SerializeField]
    string playerObjectPath;

    [SerializeField]
    Vector3 spawnPostion;

    public enum CharType { 
        Ch_01,
        Ch_02,
        Ch_03
    }
    private void Awake()
    {

        LoginResponseDTO dto = (LoginResponseDTO)PlayerManager.Get.GetValue("LoginInfo");
        CharType myType = (CharType)dto.characterType;
        Debug.Log(myType);
        //Ŭ���̾�Ʈ�� Infod������ �ٸ��� ����
        //Ÿ�Կ� ���� �ٸ��� ����
        PhotonNetwork.Instantiate(myType.ToString(), spawnPostion, Quaternion.identity) ;
    }
    // Start is called before the first frame update

}
