using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static HttpController;
using System;
using System.IO;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SongImg : MonoBehaviour,IPointerClickHandler
{
    public Image image;
    public TMP_Text title;
    public TMP_Text musician;
    public TMP_Text session;
    public TMP_Text genre;
    public bool isSelect;
    public bool isDefault;

    FileDTO dto;
    public void Set(string title,string musician,string session,string genre,Sprite sprite,FileDTO dto,int clickChangeScene) {
        image.sprite = sprite;
        this.title.text = title;
        this.musician.text = musician;
        this.session.text = session;
        this.genre.text = genre;
        this.image.sprite = sprite;
        this.dto = dto;
        this.clickChangeScene = clickChangeScene;
        
    }

    //��δ� ������ �� �׷��ϱ�, ������ �ް� ������ ������� �ϸ� ���� ������.
    //�׷���? � ���� ��ο� ������ �Ѵ�����
    //

    //���⼭ �׳� DTO�� ����ִ°�?
    //Ŭ��������, ��û�ϰ�? ���� �����͸� ? ����ϸ� ���� ������.

    string musicFilePath = null;
    const string saveFilePath = "music";
    public void Click() {

       /* Debug.Log("CLICK");
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
        isSelect = true;*/
    }



    public int clickChangeScene;
    //�� Ŭ��������
    public void OnPointerClick(PointerEventData eventData)
    {
        if (isSelect)
            return;

        //default�� �ٷ��̵�
        if (isDefault) {
            TextAsset tx = (TextAsset)Resources.Load("Default/default.mid");
            File.WriteAllBytes(Application.persistentDataPath + "/" + "files/compose.mid", tx.bytes);

            SceneController.StartLoadSceneAsync(this, false, clickChangeScene, null);
            return;
        }
            

        HttpRequest rq = null;
        //�� ���� �����
        //��ΰ� ���ٸ� ��û�ϱ�
        if (musicFilePath == null)
        {
            rq = new HttpBuilder()
            .Host(dto.musicFileUrl)
            .Type(ReqType.GET)
            .Success((down) => {
                musicFilePath = Application.persistentDataPath + "/" + saveFilePath + "/" + "music.mp3";
                File.WriteAllBytes(Application.persistentDataPath + "/" + saveFilePath + "/" + "music.mp3", down.data);
                //���� �ٿ�ε�
                Debug.Log("Down!!");
            })
            .Failure((down)=> {

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


        if (ClickEvent.instance.prevImg == null || ClickEvent.instance.prevImg != this)
            ClickEvent.instance.prevImg = this;
        else if (ClickEvent.instance.prevImg == this)
        {
            //���� ���� ����
            PlayerManager.Get.Add("FileDTO",dto);

            //�̵� ���� �������.
            string path = "files/" + "compose.mid";
            if (!Directory.Exists(Application.persistentDataPath + "/files"))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/files");
            }

            rq = new HttpBuilder()
                .Host(dto.midFileUrl)
                .Type(ReqType.GET)
                .Success((down) => {



                    File.WriteAllBytes(Application.persistentDataPath + "/" + path, down.data);


                    SceneManager.LoadScene(clickChangeScene);

                })
                .Failure((down)=> {
                    //�ٿ� ����!
                    //�⺻���� �ٿ�ε� 

                    TextAsset tx = (TextAsset)Resources.Load("Default/default.mid");
                    File.WriteAllBytes(Application.persistentDataPath + "/" + path, tx.bytes);
                    SceneManager.LoadScene(clickChangeScene);
                })
                .build();
                
            StartCoroutine(SendRequest(rq));


            //PlayerManager.Get.Add("MidiPath",);
            //dto �ѱ��...
            isSelect = true;
        }

    }


    public string DownLoadMidi(FileDTO dto) {
        string path = Application.persistentDataPath + "/files/compose/" + "compose.mid";
        HttpRequest rq = new HttpBuilder()
            .Host(dto.midFileUrl)
            .Type(ReqType.GET)
            .Success((down)=> {
                if (!Directory.Exists(Application.persistentDataPath + "/files"))
                {
                    Directory.CreateDirectory(Application.persistentDataPath + "/files");
                }

                File.WriteAllBytes(path,down.data);
                
                
            
            })
            .build();
        StartCoroutine(SendRequest(rq));
        return path;
    }
}
