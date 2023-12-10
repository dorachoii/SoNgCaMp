using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HttpController;

public class MultiSoundManager : MonoBehaviourPun
{
    public static MultiSoundManager instance;
    public SoundManager smanager;

    private void Awake()
    {
        instance = this;
    }

    [PunRPC]
    public void Play(string url)
    {
        HttpRequest rq = new HttpBuilder()
            .Host(url)
            .Type(ReqType.Audio)
            .Success((down)=> { 
                
            })
            .build();
        LoadAudioFromURL(rq,(clip)=> {
            smanager.PlayBGM(clip);
        });
        StartCoroutine(SendRequest(rq));

        //smanager.PlayBGM(clip);
    }

    public AudioClip clip;
    public void PlayRPC(string url) {
        photonView.RPC(nameof(Play),RpcTarget.All,url);
        
    }
}
