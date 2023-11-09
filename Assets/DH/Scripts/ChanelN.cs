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
        }
    }
    public int max;


    public void PlusButton() {
        Count++;
        text.text = "CH" + Count;
        btn.mytrack.number = Count;
    }
    public void MinusButton() {
        Count--;
        text.text = "CH" + Count;
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
