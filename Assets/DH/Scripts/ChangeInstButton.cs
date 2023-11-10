using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ChangeInstButton : MonoBehaviour, IPointerDownHandler,IPointerClickHandler
{
    public int ChangeChanel;
    public TextMeshProUGUI text;
    public UnityEvent<int> ev;
    
    public Image img;

    public void OnPointerClick(PointerEventData eventData)
    {
        InstrumentManager.instance.ChangeInstrument(ChangeChanel);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //InstrumentManager.instance.ChangeInstrument(ChangeChanel);
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
