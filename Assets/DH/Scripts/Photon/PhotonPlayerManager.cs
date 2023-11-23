using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonPlayerManager : MonoBehaviour
{
    [SerializeField]
    string playerObjectPath;

    [SerializeField]
    Vector3 spawnPostion;
    private void Awake()
    {
        PhotonNetwork.Instantiate(playerObjectPath,spawnPostion, Quaternion.identity);
    }
    // Start is called before the first frame update

}
