using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SongImg2 : MonoBehaviour
{
    //방장이 클릭했을 때, RPC로 해당하는 URL 전송, 모든 PC는 그 것에 요청하고 Sound Play.
    public string Artist { get; set; }
    public string Title { get; set; }
    public string Url { get; set; }
    public 
    TMP_Text _text;
    public 
    Image img;

    FileDTO dto;

    //사진은 아직.
    public void set(string text,Sprite img,FileDTO dto) {
        _text.text = text;
        this.img.sprite = img;
        this.dto = dto;
        //데이터 세팅
    }

    public void Play() {
        //내 자신의 MP3 링크를 RPC로 보낸 후,
        MultiSoundManager.instance.PlayRPC(dto.musicFileUrl);
    }
    
}
