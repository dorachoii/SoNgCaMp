using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HttpController;

public class LoadSong : MonoBehaviour
{

    private void Start()
    {
        Load();
    }
    public void Load() {
        //�ε带 �Ѵ�
        HttpRequest rq = new HttpBuilder()
            .Uri("/api/v1/findAllfiles")
            .Type(ReqType.GET)
            .Success((down) => {
                Debug.Log(down.text);
                FileListDTO dto = JsonUtility.FromJson<FileListDTO>(down.text);
                Setlist(dto);

                //Debug.Log(dtoList);

            })
            .Failure((down) => {
                //��û ���н�...
                //�⺻ ���� 
                FileListDTO dto = new FileListDTO();
                dto.files = new List<FileDTO>();
                dto.files.Add(new FileDTO() { songTitle = "DEFAULT", needSession = "DEFAULT", songArtist = "DEFAULT" }); //DEFAULT ����
                dto.files.Add(new FileDTO() { songTitle = "DEFAULT",needSession = "DEFAULT" ,songArtist = "DEFAULT" }); //DEFAULT ����
                dto.files.Add(new FileDTO() { songTitle = "DEFAULT", needSession = "DEFAULT", songArtist = "DEFAULT" }); //DEFAULT ����
                dto.files.Add(new FileDTO() { songTitle = "DEFAULT", needSession = "DEFAULT", songArtist = "DEFAULT" }); //DEFAULT ����
                dto.files.Add(new FileDTO() { songTitle = "DEFAULT", needSession = "DEFAULT", songArtist = "DEFAULT" }); //DEFAULT ����
                Setlist(dto);
            })
            .build();



        StartCoroutine(SendRequest(rq));
        
    }

    [SerializeField]
    Transform content;
    [SerializeField]
    GameObject songImgObj;

    public int selectScene;
    public void Setlist(FileListDTO dto)
    {
        int count = 0;
        dto.files.ForEach((info)=> {
            //Img �����ϱ�
            GameObject go = Instantiate(songImgObj, content);
            go.SetActive(false);
            SongImg img =  go.GetComponent<SongImg>();
            img.index = count;
            //info�� img url�� null�̸�
            if (info.imageFileUrl == null) {
                
                SetFaildList(info, img,count);
            }

            //�̹��� ��û
            HttpRequest rq = new HttpBuilder()
            .Host(info.imageFileUrl)
            .Type(ReqType.GET)
            .Success((down)=> {
                //Byte to sprite
                Sprite sprite = LoadSpriteFromBytes(down.data);
                //�ؽ�Ʈ ����
                img.Set(info.songTitle, info.songArtist, info.needSession, "?",sprite,info, selectScene);
                go.SetActive(true);
            })
            .Failure((down)=> {
                //Sprite sprite = LoadSpriteFromBytes(down.data);
                //�̹��� ��û ����. 
                //img.Set(info.songTitle, info.songArtist, info.needSession, "?", null, info, selectScene);
                //go.SetActive(true);
            })
            .build();

            
            StartCoroutine(SendRequest(rq));
            count++;
        });
    }

    //faild list
    [SerializeField]
    Sprite[] faildImgList;
    public void SetFaildList(FileDTO info, SongImg img, int count) {
        img.isDefault = true;
        //��û ���ϰ� �ٷ�
            img.Set(info.songTitle, info.songArtist, info.needSession, "?", faildImgList[count], info, selectScene);
            img.gameObject.SetActive(true);
    }


    public Sprite LoadSpriteFromBytes(byte[] imageBytes)
    {
        // Texture2D ����
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageBytes); // ����Ʈ �����͸� Texture2D�� �ε�

        // Sprite ����
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);

        return sprite;
    }

}
