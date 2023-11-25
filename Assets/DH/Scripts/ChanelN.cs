using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChanelN : DragChange
{

    public TrackButton btn;
    public TextMeshProUGUI text;
    int count;
    public int Count {
        get { return count; }
        set { 
            
            count = Mathf.Clamp(value, 0, max);
            text.text = "CH" + Count;
            if (count == 9)
            {
                btn.image.sprite = InstrumentManager.instance.iconlist[(int)InstrumentManager.Instype.Percussive];
                
            }
            else {

                int i = (int)btn.mytrack.instrument;
                btn.image.sprite = InstrumentManager.instance.iconlist[(int)InstrumentManager.retIns(i)] ;
            }
        }
    }
    public int max;


    public void PlusButton() {
        Count++;
        btn.mytrack.number = Count;
        
    }
    public void MinusButton() {
        Count--;
        btn.mytrack.number = Count;
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
