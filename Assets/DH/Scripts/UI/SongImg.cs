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

    //모두다 받으면 좀 그러니까, 없으면 받고 있으면 재생으로 하면 되지 않을까.
    //그래서? 어떤 파일 경로에 저장을 한다음에
    //

    //여기서 그냥 DTO를 들고있는게?
    //클릭했을때, 요청하고? 받은 데이터를 ? 재생하면 되지 않을까.

    string musicFilePath = null;
    const string saveFilePath = "music";
    public void Click() {

       /* Debug.Log("CLICK");
        if (isSelect) { 
            //넘어가기
            //DTO 넘겨주기
        }

        //재생하기 

        HttpRequest rq = null;
        //내 음악 들려줘
        //경로가 없다면 요청하기
        if (musicFilePath == null) { 
            rq = new HttpBuilder()
            .Host(dto.musicFileUrl)
            .Type(ReqType.GET)
            .Success((down) => {
                musicFilePath = Application.persistentDataPath + "/" + saveFilePath + "/" + "music.mp3";
                File.WriteAllBytes(Application.persistentDataPath + "/" + saveFilePath + "/" + "music.mp3",down.data);
                //파일 다운로드
                Debug.Log("Down!!");
            })
            .build();
        }
        //있다면 그 경로로 요청
        else
        {
            rq = new HttpBuilder()
            .Host("file://")
            .Uri(musicFilePath)
            .Type(ReqType.GET)
            .build();
        }
        StartCoroutine(LoadAudioFromURL(rq, (clip) => { SoundManager.Get.PlayBGM(clip); }));

        //mp3 파일 읽어서 재생하기
        //musicFilePath = Application.persistentDataPath + "/" + saveFilePath + "/" + "music.mp3";
        isSelect = true;*/
    }



    public int clickChangeScene;
    //나 클릭했을때
    public void OnPointerClick(PointerEventData eventData)
    {
        if (isSelect)
            return;

        //default면 바로이동
        if (isDefault) {
            TextAsset tx = (TextAsset)Resources.Load("Default/default.mid");
            File.WriteAllBytes(Application.persistentDataPath + "/" + "files/compose.mid", tx.bytes);

            SceneController.StartLoadSceneAsync(this, false, clickChangeScene, null);
            return;
        }
            

        HttpRequest rq = null;
        //내 음악 들려줘
        //경로가 없다면 요청하기
        if (musicFilePath == null)
        {
            rq = new HttpBuilder()
            .Host(dto.musicFileUrl)
            .Type(ReqType.GET)
            .Success((down) => {
                musicFilePath = Application.persistentDataPath + "/" + saveFilePath + "/" + "music.mp3";
                File.WriteAllBytes(Application.persistentDataPath + "/" + saveFilePath + "/" + "music.mp3", down.data);
                //파일 다운로드
                Debug.Log("Down!!");
            })
            .Failure((down)=> {

            })
            .build();
        }
        //있다면 그 경로로 요청
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
            //파일 정보 저장
            PlayerManager.Get.Add("FileDTO",dto);

            //미디 정보 저장까지.
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
                    //다운 실패!
                    //기본으로 다운로드 

                    TextAsset tx = (TextAsset)Resources.Load("Default/default.mid");
                    File.WriteAllBytes(Application.persistentDataPath + "/" + path, tx.bytes);
                    SceneManager.LoadScene(clickChangeScene);
                })
                .build();
                
            StartCoroutine(SendRequest(rq));


            //PlayerManager.Get.Add("MidiPath",);
            //dto 넘기기...
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
