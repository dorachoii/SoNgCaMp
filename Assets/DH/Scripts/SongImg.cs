using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static HttpController;
using System;
using System.IO;
using UnityEngine.EventSystems;

public class SongImg : MonoBehaviour,IPointerClickHandler
{
    public Image image;
    public TMP_Text title;
    public TMP_Text musician;
    public TMP_Text session;
    public TMP_Text genre;
    public bool isSelect;
    FileDTO dto;
    public void Set(string title,string musician,string session,string genre,Sprite sprite,FileDTO dto) {
        image.sprite = sprite;
        this.title.text = title;
        this.musician.text = musician;
        this.session.text = session;
        this.genre.text = genre;
        this.image.sprite = sprite;
        this.dto = dto;
        
    }

    //��δ� ������ �� �׷��ϱ�, ������ �ް� ������ ������� �ϸ� ���� ������.
    //�׷���? � ���� ��ο� ������ �Ѵ�����
    //

    //���⼭ �׳� DTO�� ����ִ°�?
    //Ŭ��������, ��û�ϰ�? ���� �����͸� ? ����ϸ� ���� ������.

    string musicFilePath = null;
    const string saveFilePath = "music";
    public void Click() {

        Debug.Log("CLICK");
        if (isSelect) { 
            //�Ѿ��
            //DTO �Ѱ��ֱ�
        }

        //����ϱ� 

        HttpRequest rq = null;
        //�� ���� �����
        //��ΰ� ���ٸ� ��û�ϱ�
        if (musicFilePath == null) { 
            rq = new HttpBuilder()
            .Host(dto.musicFileUrl)
            .Type(ReqType.GET)
            .Success((down) => {
                musicFilePath = Application.persistentDataPath + "/" + saveFilePath + "/" + "music.mp3";
                File.WriteAllBytes(Application.persistentDataPath + "/" + saveFilePath + "/" + "music.mp3",down.data);
                //���� �ٿ�ε�
                Debug.Log("Down!!");
            })
            .build();
        }
        //�ִٸ� �� ��η� ��û
        else
        {
            rq = new HttpBuilder()
            .Host("file://")
            .Uri(musicFilePath)
            .Type(ReqType.GET)
            .build();
        }
        StartCoroutine(LoadAudioFromURL(rq, (clip) => { SoundManager.Get.PlayBGM(clip); }));

        //mp3 ���� �о ����ϱ�
        //musicFilePath = Application.persistentDataPath + "/" + saveFilePath + "/" + "music.mp3";
        isSelect = true;
    }



    //�� Ŭ��������
    public void OnPointerClick(PointerEventData eventData)
    {
        if (isSelect)
            return;

        if (ClickEvent.instance.prevImg == null || ClickEvent.instance.prevImg != this)
            ClickEvent.instance.prevImg = this;
        else if (ClickEvent.instance.prevImg == this)
        {
            Debug.Log("�ι� Ŭ��");
            //dto �ѱ��...
            isSelect = true;
        }

    }
}
