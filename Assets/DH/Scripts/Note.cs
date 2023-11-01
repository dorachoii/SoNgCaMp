using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    //시간
    public float time = 2;
    float currentTime;
    //이전 시간
    float backTime;
    public AudioClip[] clips;
    int count;
    void Update()
    {
        //시간이 지나다가
        //currentTime += Time.deltaTime * time;

        //현재 시간 -이전 시간
        //if (currentTime - backTime > time)
        //{


        //    Debug.Log("찍는다."); count++;
        //    if (count > clips.Length)
        //    {
        //        count = 0; currentTime = 0; backTime = 0;
        //    }
        //    backTime = currentTime;
        //    if (clips[count])
        //    {
        //        PlaySound.instance.oneplay(clips[count]);

        //    }

        //}


    }
}
