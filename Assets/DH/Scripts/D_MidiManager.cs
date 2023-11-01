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
        /// 초를 받아서 Deltatime으로 변환하는 함수.(현재는 박자로 테스트중)
        /// </summary>
        public static int ConvertSecondsToDeltatime(float seconds)
        {
            int Division = 60;
            return (int)(Division * seconds);
        }

        /// <summary>
        /// 타입에 맞춰서 뒤에 0 지우는 함수
        /// </summary>
        public static byte[] GetBytesCheckType(int num)
        {
            byte[] bytes = null;
            if (num >> 8 <= 0)
            {
                Debug.Log(num >> 8);
                Debug.Log("Byte 입니다~람쥐");

                bytes = new byte[] { (byte)num };
            }
            else if (num >> 16 <= 0)
            {
                Debug.Log(num >> 16);
                Debug.Log("Short 입니다~람쥐");

                bytes = BitConverter.GetBytes((short)num);
            }
            //최상위 비트 = 부호비트?
            else if (num >> 31 <= 0)
            {
                Debug.Log(num >> 31);
                Debug.Log("int 입니다~람쥐");

                bytes = BitConverter.GetBytes((int)num);
            }
            if (bytes == null)
            {
                throw new NullReferenceException();
            }

            return bytes;
        }



        /// <summary>
        /// deltatime을 midi 파일 규약에 맞게 변환하는 함수
        /// </summary>
        /// <returns>Convert Deltatime</returns>
        public static byte[] ConvertDeltaTime(int deltaTime)
        {
            int convertDelta = 0;
            //첫 번째 바이트의 첫 번째 비트 = 0
            convertDelta = deltaTime & 127;
            //7비트를 읽었으니 7 비트 버린다.
            deltaTime = deltaTime >> 7;
            //동일하지만 첫 번째 비트가 1이 아닌것
            //7비트를 읽을 수 있으면 반복
            for (; deltaTime > 0;)
            {
                convertDelta = (convertDelta << 8) | (deltaTime | 128);
                deltaTime = deltaTime >> 7;
            }
            byte[] convertByte = GetBytesCheckType(convertDelta);
            //byte일수도 있고 int일수도 있는데?
            //문제점 : int 형식인데 수가 Byte라면 미달된 바이트가 0이 되버림 따라서 체크가 필요함. 

            return convertByte;
        }
    }
    class MidiFiles
    {
        //헤더
        byte[] midiData = {
            // Offset 0x00000000 to 0x00000108
            //16진수 1개 = 1바이트
            //헤더 청크 = 4바이트 2바이트 6 바이트
            0x4D, 0x54, 0x68, 0x64, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00, 0x01,
            //청크 끝
            0x3C, 0x00,
            //트랙 청크
            0x4D, 0x54, 0x72, 0x6B, 0x00, 0x00, 0xDD, 0xDD, 0x00, //데이터의 길이 : 원래 1D임.
            //메타이벤트
            0xFF,
            0x51, 0x03, 0x07, 0xA1, 0x20, 0x00,
            0xFF, 0x58, 0x04, 0x04, 0x02, 0x18,
            //Note의 정보. 
            0x08,
            0x00, 0x90, 0x53, 0x64,
            0x82, 0xA4, 0x47, 
            //0x80,
            0x53, 0x00


            //추가 정보
            //0xBB,0x15,0x90,0x3C,0x50,
            //0xBB,0x15,0x3C,0x00,
        };
        byte[] last_data = { 0x00,
        0xFF, 0x2F, 0x00 };

        //트랙

        //어쩌구

        //미디 파일 템플릿을 하나 생성한다.

    }
    class MusicNode
    {
        int deltatime;
        int pitch;
        int velocity;
        //몇초 //음계 // 세기
        public enum State { On, End };
        State state;
        public MusicNode() { }

        //이거는 아님..정확히는 상속 구조를 가지고 있음(이벤트를 상속받음 그리고 이벤트어쩌구는 델타타임과 이벤트로 나뉨따라서 0,과 이벤트로 나뉠수있음
        /// <summary>
        /// MusicNode 생성하기
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
