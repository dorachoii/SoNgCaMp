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
        StartCoroutine(LoadAudioFromURL(rq, (clip) => {
            Debug.Log("재생이 왜 안되느냐.");
            smanager.PlayBGM(clip);
        }));



        //smanager.PlayBGM(clip);
    }

    public AudioClip clip;
    public void PlayRPC(string url) {
        Play(url);
        
        //photonView.RPC(nameof(Play),RpcTarget.All,url);
        
    }
}
