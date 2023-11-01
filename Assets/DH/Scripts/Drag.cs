using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Drag : MonoBehaviour, IPointerDownHandler
{
    public Notes Tile;
    //����, ������ �����ؾ���
    public GameObject TileFactory;
    public void OnPointerDown(PointerEventData eventData)
    {
        //Instantiate(Tile,transform);
        //�������� ���� ���� �ִ� ��ü��
        
        Tile.gameObject.SetActive(true);

        //Paste ----------- Test Code
        Tile.info.Copy(NoteManager.instance.SaveData);
        Tile._IPitch = NoteManager.instance.SaveData.Pitch;
        Tile._Beat = NoteManager.instance.SaveData.beat;
        Tile.info.enable = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
