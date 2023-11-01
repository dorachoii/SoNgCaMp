using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageButton : MonoBehaviour
{
    public Text text;
    public int idx;
    Button button;
    private void Awake()
    {
        button = GetComponent<Button>();
    }
    //Start 즉 버튼이 생성될때 한번만 해주면 된다!!!
    private void Start()
    {
        click();
    }

    //페이지를 클릭 했을 때,
    //내 인덱스에 해당해는 요소 가져오기
    //웹 개발을 생각 해 보자.
    public void click() {
        //몇 번째 노트 데이터를 알려주세요.
        NoteBlockInfo[] notes = UIManager.instance.NoteList[idx];
        Debug.Log("Loading!!");

        //받은 데이터 렌더링 하기
        for (int i = 0; i < notes.Length; i++) {

            if (notes[i] != null) {

                //불러올때는 노트의 정보가
                Notes note = Board.instance.drags[i].Tile;
                NoteBlockInfo info = note.info = notes[i];
                note._Beat = info.beat;
                note._IPitch = info.Pitch;
                
                Debug.Log(info.pitch);
                note.gameObject.SetActive(info.enable);
            }


        }

    }
}
