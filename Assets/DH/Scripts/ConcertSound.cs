using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConcertSound : MonoBehaviour
{
    
    private void Awake()
    {
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.Instantiate("MultiSound",Vector3.zero,Quaternion.identity);
    }
}
