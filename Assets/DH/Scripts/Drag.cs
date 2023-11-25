using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Drag : MonoBehaviour, IPointerDownHandler
{
    public Notes Tile;
    //만약, 없으면 생성해야함
    public GameObject TileFactory;
    public void OnPointerDown(PointerEventData eventData)
    {
        //Instantiate(Tile,transform);
        //생성하지 말고 원래 있던 개체로
        

        Debug.Log(UIManager.instance.currentTrack);

        //Paste ----------- Test Code
        Tile.info.Copy(NoteManager.instance.SaveData);
        Tile._IPitch = NoteManager.instance.SaveData.Pitch;
        Tile._Beat = NoteManager.instance.SaveData.beat;
        Tile._IBeat = NoteManager.instance.SaveData.Beat;
        Tile.gameObject.SetActive(true);
        Tile.info.enable = true;
        //현재 트랙이 드럼이라면 
        if (UIManager.instance.currentTrack.number == 9) { 
            Tile._IPitch = (int)UIManager.instance.currentTrack.instrument;
            return;
        }


        Debug.Log(UIManager.instance.currentTrack);

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
