using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public int number = 1;
    // Start is called before the first frame update
    void Start()
    {
        int aa = number & 127;
        Debug.Log(aa); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Onclick() {
        Debug.LogWarning(number >> 8);
        Debug.LogWarning(number >> 16);
        Debug.LogWarning(number >> 32);
        //계산하기 
        if (number >> 8 <= 0)
        {
            Debug.Log(number >> 8);
            Debug.Log("Byte 입니다~");
        }
        else if (number >> 16 <= 0)
        {
            Debug.Log(number >> 16);
            Debug.Log("Short 입니다~");
        }
        //최상위 비트 = 부호비트?
        else if (number >> 31 <= 0)
        {
            Debug.Log(number >> 32);
            Debug.Log("int 입니다~!");
        }
    }
}
