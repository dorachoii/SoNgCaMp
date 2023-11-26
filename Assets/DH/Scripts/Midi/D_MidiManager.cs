using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using System.Linq;
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
        public static int ConvertSecondsToDeltatime( float seconds, int division = 60)
        {
            
            return (int)(division * seconds);
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

        public static byte[] CustomGetBytes(int num) {
            //일단? 처음부터 읽어야지
            //if(num >> 4 0)
            //8바이트라고 해.
            //8바이트 --> 4읽고 4남았으니까 4읽고.
            int number = num;
            while (number >> 4 > 0) {
                //4 비트를 읽습니다!!!
                byte b = (byte)(number >> 4 & 0x0F);

                //읽었으니 4비트 지우기
                number = number >> 4;


            }


            return null;
        }

        public static byte[] RemoveZero(byte[] array) {
            //바이트 배열을 받고, 
            //NETWORK ORDER 방식. 
            if (BitConverter.IsLittleEndian)
                Array.Reverse(array);
                
            List<byte> list = array.ToList();
            //첫번째 0을 지우자.
            for(int i = 0; i < list.Count; i++)
            {
                //0~3까지니까. Count -1일때는 0이여도 무시를 해야한다.
                if (list[i] == 0 && i != list.Count - 1) {
                    list.RemoveAt(i);
                    i = -1;
                }
            }
            return list.ToArray();
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

            if(!BitConverter.IsLittleEndian)
                Array.Reverse(convertByte);
            //byte일수도 있고 int일수도 있는데?
            //문제점 : int 형식인데 수가 Byte라면 미달된 바이트가 0이 되버림 따라서 체크가 필요함. 

            return convertByte;
        }


        /// <summary>
        /// 가변 바이트 deltatime을 원상태 deltatime으로 복구하는 작업
        /// </summary>
        public static int ReadDeltaTime(byte[] buffer)
        {
            //델타타임을 계산할때는 Bigendian 방식으로 되어있어야 함.
            if (BitConverter.IsLittleEndian)
                Array.Reverse(buffer);

            int time = 0;
            byte b;
            int offset = 0;
            do
            {
                b = buffer[offset];
                offset++;
                //이거 왜하는가?
                //0x7F = 0111 1111
                //& 연산해서 같은거 1 나머지 0 

                //기존에 있던 값을 7비트 이동시키고 현재 deltatime을 7비트 추가하는 행위를 하는 것.
                //델타 타임이 누적이 된다~
                time = (time << 7) | (b & 0x7F);

                //why? 0111 1111을 and 시키니까 최상위 비트는 무조건 0이 됨. 즉 날아간다는 소리이다.
                //time을 left shift 한다는 행위는 오른쪽에 비트가 7개 채워짐
                //그것을 or 하면 현재 타임의 정보가 들어감.
            }
            //127 이하 수는 최상위 비트가 0이다. 따라서 127보다 클때만 time을 계산하는 수행을 반복한다.
            while (b > 127);
            return time;
        }


        //128개의 악기를 모두 열거해야 하는가??
        public enum Instruments {
            Acoustic_Grand_Piano,
            Bright_Acoustic_Piano,
            Electric_Grand_Piano,
            Honky_tonk_Piano,
            Rhodes_Piano,
            Chorused_Piano,
            Harpsichord,
            Clavinet,
            Celesta,
            Glockenspiel,
            Music_Box,
            Vibraphone,
            Marimba,
            Xylophone,
            Tubular_Bells,
            Dulcimer,
            Hammond_Organ,
            Percussive_Organ,
            Rock_Organ,
            Church_Organ,
            Reed_Organ,
            Accordion,
            Harmonica,
            Tango_Accordion,
            Acoustic_Guitar_Nylon,
            Acoustic_Guitar_Steel,
            Electric_Guitar_Jazz,
            Electric_Guitar_Clean,
            Electric_Guitar_Muted,
            Overdriven_Guitar,
            Distortion_Guitar,
            Guitar_Harmonics,
            Acoustic_Bass,
            Electric_Bass_Finger,
            Electric_Bass_Pick,
            Fretless_Bass,
            Slap_Bass_1,
            Slap_Bass_2,
            Synth_Bass_1,
            Synth_Bass_2,
            Violin,
            Viola,
            Cello,
            Contrabass,
            Tremolo_Strings,
            Pizzicato_Strings,
            Orchestral_Harp,
            Timpani,
            String_Ensemble_1,
            String_Ensemble_2,
            Synth_Strings_1,
            Synth_Strings_2,
            Choir_Aahs,
            Voice_Oohs,
            Synth_Voice,
            Orchestra_Hit,
            Trumpet,
            Trombone,
            Tuba,
            Muted_Trumpet,
            French_Horn,
            Brass_Section,
            Synth_Brass_1,
            Synth_Brass_2,
            Soprano_Sax,
            Alto_Sax,
            Tenor_Sax,
            Baritone_Sax,
            Oboe,
            English_Horn,
            Bassoon,
            Clarinet,
            Piccolo,
            Flute,
            Recorder,
            Pan_Flute,
            Bottle_Blow,
            Shakuhachi,
            Whistle,
            Ocarina,
            Lead_1_Square,
            Lead_2_Sawtooth,
            Lead_3_Calliope,
            Lead_4_Chiffer,
            Lead_5_Charang,
            Lead_6_Voice,
            Lead_7_Fifths,
            Lead_8_Brass_Lead,
            Pad_1_New_Age,
            Pad_2_Warm,
            Pad_3_Polysynth,
            Pad_4_Choir,
            Pad_5_Bowed,
            Pad_6_Metallic,
            Pad_7_Halo,
            Pad_8_Sweep,
            FX_1_Rain,
            FX_2_Soundtrack,
            FX_3_Crystal,
            FX_4_Atmosphere,
            FX_5_Brightness,
            FX_6_Goblins,
            FX_7_Echoes,
            FX_8_Sci_Fi,
            Sitar,
            Banjo,
            Shamisen,
            Koto,
            Kalimba,
            Bagpipe,
            Fiddle,
            Shana,
            Tinkle_Bell,
            Agogo,
            Steel_Drums,
            Woodblock,
            Taiko_Drum,
            Melodic_Tom,
            Synth_Drum,
            Reverse_Cymbal,
            Guitar_Fret_Noise,
            Breath_Noise,
            Seashore,
            Bird_Tweet,
            Telephone_Ring,
            Helicopter,
            Applause,
            Gunshot
        }

        public enum DrumSound
        {
            AcousticBassDrum = 35,
            BassDrum1 = 36,
            SideStick = 37,
            AcousticSnare = 38,
            HandClap = 39,
            ElectricSnare = 40,
            LowFloorTom = 41,
            ClosedHiHat = 42,
            HighFloorTom = 43,
            PedalHiHat = 44,
            LowTom = 45,
            OpenHiHat = 46,
            LowMidTom = 47,
            HiMidTom = 48,
            CrashCymbal1 = 49,
            HighTom = 50,
            RideCymbal1 = 51,
            ChineseCymbal = 52,
            RideBell = 53,
            Tambourine = 54,
            SplashCymbal = 55,
            Cowbell = 56,
            CrashSymbol2 = 57,
            Vibraslap = 58,
            RideCymbal2 = 59,
            HiBongo = 60,
            LowBongo = 61,
            MuteHiConga = 62,
            OpenHiConga = 63,
            LowConga = 64,
            HighTimbale = 65,
            LowTimbale = 66,
            HighAgogo = 67,
            LowAgogo = 68,
            Cabasa = 69,
            Maracas = 70,
            ShortWhistle = 71,
            LongWhistle = 72,
            ShortGuiro = 73,
            LongGuiro = 74,
            Claves = 75,
            HiWoodBlock = 76,
            LowWoodBlock = 77,
            MuteCuica = 78,
            OpenCuica = 79,
            MuteTriangle = 80,
            OpenTriangle = 81,
            Shaker = 82
        }

        const int CHANGE_EVENT = 192;
        //이 채널의 악기를 이것으로 바꾸자.
        public static byte[] ChangeInstument(int channel, Instruments instruments)
        {
            //Cc (악기)
            //근데 함정이? 16진수니까.
            //C0는 192이니까,

            byte _event = (byte)(CHANGE_EVENT + channel); //이벤트와 채널 합치기 ( 1 바이트)
            //뒤에는 악기 바이트 들어옴.
            byte instrument = (byte)instruments;


            return new byte[] { _event, instrument };
        }

        /// <summary>
        /// BPM TO MICRO... MIDI FIle은 SET TEMPO EVENT를 비트당 마이크로초로 계산함.
        /// </summary>
        public static int ConvertBpmToMicro(int bpm)
        {
            //초당 비트
            double bps = bpm / 60;

            // 1 / bps = 1비트당 초
            
            //비트 당 마이크로 초 
            return (int) ((1 / bps) * 1000000);
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
