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
        //ó�� �ݺ������ϱ� -1 
        //while (true) {
        //    if (num >> (7 * i) <= 0) {
        //        break;
        //    }
        //    i++;
        //}
        //�ֻ��� ��Ʈ 0 0���� �ϱ�. 0111 1111 �̴ϱ�!!
        //convertDelta = num & 127;
        ////7��Ʈ ������. 7��Ʈ ��������.
        //num = num >> 7;

        //for (; num > 0;) {
        //    convertDelta = (convertDelta << 8) | (num | 128);
        //    num = num >> 7;

        //}
        //byte[] convertByte = BitConverter.GetBytes(convertDelta);

        //Debug.Log(convertByte);

       // byte[] bytes = MidiManager.ConvertDeltaTime(5305);
        //Debug.LogError(bytes);
        //Byte Order? ����
    }

    // Update is called once per frame
    void Update()
    {
        //�� ���� ��� ����Ʈ ��.

        //num & 128 �� 1��Ʈ + 7��Ʈ ��������..
        //ConvertDelta�� 8��Ʈ �а� or�ؼ� ����.


        //convertDelta = (convertDelta << 8) | (num | 128);
        //num = num >> 7;

        //convertDelta = (convertDelta << 8) | (num | 128);
        //num = num >> 7;

    }
}
