using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    //�ð�
    public float time = 2;
    float currentTime;
    //���� �ð�
    float backTime;
    public AudioClip[] clips;
    int count;
    void Update()
    {
        //�ð��� �����ٰ�
        //currentTime += Time.deltaTime * time;

        //���� �ð� -���� �ð�
        //if (currentTime - backTime > time)
        //{


        //    Debug.Log("��´�."); count++;
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
