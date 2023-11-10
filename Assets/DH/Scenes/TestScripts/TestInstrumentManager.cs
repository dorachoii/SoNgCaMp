using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInstrumentManager : MonoBehaviour
{
    public static TestInstrumentManager instance;

    
    public Action<int> SelectAction
    {
        get;
        set;
    }



}
