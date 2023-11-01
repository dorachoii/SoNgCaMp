using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DummyHeaderData dum = new DummyHeaderData();
        HeaderChunk header = new HeaderChunk(dum.H_Ctype,dum.H_Length,dum.H_Data);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
