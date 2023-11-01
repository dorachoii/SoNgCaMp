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
    //Start �� ��ư�� �����ɶ� �ѹ��� ���ָ� �ȴ�!!!
    private void Start()
    {
        click();
    }

    //�������� Ŭ�� ���� ��,
    //�� �ε����� �ش��ش� ��� ��������
    //�� ������ ���� �� ����.
    public void click() {
        //�� ��° ��Ʈ �����͸� �˷��ּ���.
        NoteBlockInfo[] notes = UIManager.instance.NoteList[idx];
        Debug.Log("Loading!!");

        //���� ������ ������ �ϱ�
        for (int i = 0; i < notes.Length; i++) {

            if (notes[i] != null) {

                //�ҷ��ö��� ��Ʈ�� ������
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
