using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TestButton1234 : MonoBehaviour
{
    public UnityEvent<int> ev;

    public void Testint(int aa) {
        Debug.Log("ss" + aa);
    }

    public void TestInt2(int aa) {
        Debug.LogError("Error" + aa);
    }
    void Start()
    {
        ev.Invoke(12);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
