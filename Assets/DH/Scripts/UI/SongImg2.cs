using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SongImg2 : MonoBehaviour
{
    //������ Ŭ������ ��, RPC�� �ش��ϴ� URL ����, ��� PC�� �� �Ϳ� ��û�ϰ� Sound Play.
    public string Artist { get; set; }
    public string Title { get; set; }
    public string Url { get; set; }
    public 
    TMP_Text _text;
    public 
    Image img;

    FileDTO dto;

    //������ ����.
    public void set(string text,Sprite img,FileDTO dto) {
        _text.text = text;
        this.img.sprite = img;
        this.dto = dto;
        //������ ����
    }

    public void Play() {
        //�� �ڽ��� MP3 ��ũ�� RPC�� ���� ��,
        MultiSoundManager.instance.PlayRPC(dto.musicFileUrl);
    }
    
}
