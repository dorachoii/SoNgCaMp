using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SongImg2 : MonoBehaviour
{
    //������ Ŭ������ ��, RPC�� �ش��ϴ� URL ����, ��� PC�� �� �Ϳ� ��û�ϰ� SOund Play.
    public string Artist { get; set; }
    public string Title { get; set; }
    public string Url { get; set; }
    public 
    TMP_Text _text;
    public 
    Image img;

    //������ ����.
    public void set(string artist,string title,string url) {
        Artist = artist;
        Title = title;
        Url = url;
    }


    
}
