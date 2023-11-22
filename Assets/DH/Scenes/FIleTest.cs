using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using UnityEngine;
using UnityEngine.Networking;
using static HttpController;

public class FIleTest : MonoBehaviour
{
    AudioSource source;
    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }
    // Start is called before the first frame update
    void Start()
    {

        HttpRequest rq = new HttpBuilder()
            .Host("http://192.168.0.20/")
            .Uri("test1234/download")
            .Success((down)=> {

            })
            .build();
        StartCoroutine(LoadAudioFromURL(rq, (i) => { SoundManager.Get.PlayBGM(i); }));

    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
