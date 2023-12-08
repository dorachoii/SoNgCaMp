using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
    public Coroutine cor;
    void Start()
    {
        cor = StartCoroutine(StartBoard());
    }

    IEnumerator StartBoard() {
        while (true) {
            transform.forward = Camera.main.transform.forward;
            yield return null;
        }
    }
    void Update()
    {
       
    }
}
