using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ByteTest : MonoBehaviour
{
    int num = 53051;
    int convertDelta;
    // Start is called before the first frame update
    void Start()
    {
        int i = 0;
        //처음 반복했으니까 -1 
        //while (true) {
        //    if (num >> (7 * i) <= 0) {
        //        break;
        //    }
        //    i++;
        //}
        //최상위 비트 0 0으로 하기. 0111 1111 이니까!!
        //convertDelta = num & 127;
        ////7비트 꺼냈음. 7비트 삭제해줘.
        //num = num >> 7;

        //for (; num > 0;) {
        //    convertDelta = (convertDelta << 8) | (num | 128);
        //    num = num >> 7;

        //}
        //byte[] convertByte = BitConverter.GetBytes(convertDelta);

        //Debug.Log(convertByte);

       // byte[] bytes = MidiManager.ConvertDeltaTime(5305);
        //Debug.LogError(bytes);
        //Byte Order? 몰루
    }

    // Update is called once per frame
    void Update()
    {
        //이 수의 모든 바이트 수.

        //num & 128 은 1비트 + 7비트 가져오기..
        //ConvertDelta에 8비트 밀고 or해서 쓰기.


        //convertDelta = (convertDelta << 8) | (num | 128);
        //num = num >> 7;

        //convertDelta = (convertDelta << 8) | (num | 128);
        //num = num >> 7;

    }
}
