using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DH {
    public class D_MidiManager : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        //Deltatime to Seconds
        public static int ConvertDeltaTimeToSeconds(int deltatime)
        {
            return 0;
        }

        //Seconds to Delta
        /// <summary>
        /// �ʸ� �޾Ƽ� Deltatime���� ��ȯ�ϴ� �Լ�.(����� ���ڷ� �׽�Ʈ��)
        /// </summary>
        public static int ConvertSecondsToDeltatime(float seconds)
        {
            int Division = 60;
            return (int)(Division * seconds);
        }

        /// <summary>
        /// Ÿ�Կ� ���缭 �ڿ� 0 ����� �Լ�
        /// </summary>
        public static byte[] GetBytesCheckType(int num)
        {
            byte[] bytes = null;
            if (num >> 8 <= 0)
            {
                Debug.Log(num >> 8);
                Debug.Log("Byte �Դϴ�~����");

                bytes = new byte[] { (byte)num };
            }
            else if (num >> 16 <= 0)
            {
                Debug.Log(num >> 16);
                Debug.Log("Short �Դϴ�~����");

                bytes = BitConverter.GetBytes((short)num);
            }
            //�ֻ��� ��Ʈ = ��ȣ��Ʈ?
            else if (num >> 31 <= 0)
            {
                Debug.Log(num >> 31);
                Debug.Log("int �Դϴ�~����");

                bytes = BitConverter.GetBytes((int)num);
            }
            if (bytes == null)
            {
                throw new NullReferenceException();
            }

            return bytes;
        }



        /// <summary>
        /// deltatime�� midi ���� �Ծ࿡ �°� ��ȯ�ϴ� �Լ�
        /// </summary>
        /// <returns>Convert Deltatime</returns>
        public static byte[] ConvertDeltaTime(int deltaTime)
        {
            int convertDelta = 0;
            //ù ��° ����Ʈ�� ù ��° ��Ʈ = 0
            convertDelta = deltaTime & 127;
            //7��Ʈ�� �о����� 7 ��Ʈ ������.
            deltaTime = deltaTime >> 7;
            //���������� ù ��° ��Ʈ�� 1�� �ƴѰ�
            //7��Ʈ�� ���� �� ������ �ݺ�
            for (; deltaTime > 0;)
            {
                convertDelta = (convertDelta << 8) | (deltaTime | 128);
                deltaTime = deltaTime >> 7;
            }
            byte[] convertByte = GetBytesCheckType(convertDelta);
            //byte�ϼ��� �ְ� int�ϼ��� �ִµ�?
            //������ : int �����ε� ���� Byte��� �̴޵� ����Ʈ�� 0�� �ǹ��� ���� üũ�� �ʿ���. 

            return convertByte;
        }
    }
    class MidiFiles
    {
        //���
        byte[] midiData = {
            // Offset 0x00000000 to 0x00000108
            //16���� 1�� = 1����Ʈ
            //��� ûũ = 4����Ʈ 2����Ʈ 6 ����Ʈ
            0x4D, 0x54, 0x68, 0x64, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00, 0x01,
            //ûũ ��
            0x3C, 0x00,
            //Ʈ�� ûũ
            0x4D, 0x54, 0x72, 0x6B, 0x00, 0x00, 0xDD, 0xDD, 0x00, //�������� ���� : ���� 1D��.
            //��Ÿ�̺�Ʈ
            0xFF,
            0x51, 0x03, 0x07, 0xA1, 0x20, 0x00,
            0xFF, 0x58, 0x04, 0x04, 0x02, 0x18,
            //Note�� ����. 
            0x08,
            0x00, 0x90, 0x53, 0x64,
            0x82, 0xA4, 0x47, 
            //0x80,
            0x53, 0x00


            //�߰� ����
            //0xBB,0x15,0x90,0x3C,0x50,
            //0xBB,0x15,0x3C,0x00,
        };
        byte[] last_data = { 0x00,
        0xFF, 0x2F, 0x00 };

        //Ʈ��

        //��¼��

        //�̵� ���� ���ø��� �ϳ� �����Ѵ�.

    }
    class MusicNode
    {
        int deltatime;
        int pitch;
        int velocity;
        //���� //���� // ����
        public enum State { On, End };
        State state;
        public MusicNode() { }

        //�̰Ŵ� �ƴ�..��Ȯ���� ��� ������ ������ ����(�̺�Ʈ�� ��ӹ��� �׸��� �̺�Ʈ��¼���� ��ŸŸ�Ӱ� �̺�Ʈ�� �������� 0,�� �̺�Ʈ�� ����������
        /// <summary>
        /// MusicNode �����ϱ�
        /// </summary>
        public MusicNode(State state, int deltatime, int pitch, int velocity)
        {
            this.state = state;
            this.deltatime = deltatime;
            this.pitch = pitch;
            this.velocity = velocity;
        }

    }
}
