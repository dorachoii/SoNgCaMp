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
        /// �ʸ� �޾Ƽ� Deltatime���� ��ȯ�ϴ� �Լ�.(����� ���ڷ� �׽�Ʈ��)
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

        public static byte[] CustomGetBytes(int num) {
            //�ϴ�? ó������ �о����
            //if(num >> 4 0)
            //8����Ʈ��� ��.
            //8����Ʈ --> 4�а� 4�������ϱ� 4�а�.
            int number = num;
            while (number >> 4 > 0) {
                //4 ��Ʈ�� �н��ϴ�!!!
                byte b = (byte)(number >> 4 & 0x0F);

                //�о����� 4��Ʈ �����
                number = number >> 4;


            }


            return null;
        }

        public static byte[] RemoveZero(byte[] array) {
            //����Ʈ �迭�� �ް�, 
            //NETWORK ORDER ���. 
            if (BitConverter.IsLittleEndian)
                Array.Reverse(array);
                
            List<byte> list = array.ToList();
            //ù��° 0�� ������.
            for(int i = 0; i < list.Count; i++)
            {
                //0~3�����ϱ�. Count -1�϶��� 0�̿��� ���ø� �ؾ��Ѵ�.
                if (list[i] == 0 && i != list.Count - 1) {
                    list.RemoveAt(i);
                    i = -1;
                }
            }
            return list.ToArray();
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

            if(!BitConverter.IsLittleEndian)
                Array.Reverse(convertByte);
            //byte�ϼ��� �ְ� int�ϼ��� �ִµ�?
            //������ : int �����ε� ���� Byte��� �̴޵� ����Ʈ�� 0�� �ǹ��� ���� üũ�� �ʿ���. 

            return convertByte;
        }


        /// <summary>
        /// ���� ����Ʈ deltatime�� ������ deltatime���� �����ϴ� �۾�
        /// </summary>
        public static int ReadDeltaTime(byte[] buffer)
        {
            //��ŸŸ���� ����Ҷ��� Bigendian ������� �Ǿ��־�� ��.
            if (BitConverter.IsLittleEndian)
                Array.Reverse(buffer);

            int time = 0;
            byte b;
            int offset = 0;
            do
            {
                b = buffer[offset];
                offset++;
                //�̰� ���ϴ°�?
                //0x7F = 0111 1111
                //& �����ؼ� ������ 1 ������ 0 

                //������ �ִ� ���� 7��Ʈ �̵���Ű�� ���� deltatime�� 7��Ʈ �߰��ϴ� ������ �ϴ� ��.
                //��Ÿ Ÿ���� ������ �ȴ�~
                time = (time << 7) | (b & 0x7F);

                //why? 0111 1111�� and ��Ű�ϱ� �ֻ��� ��Ʈ�� ������ 0�� ��. �� ���ư��ٴ� �Ҹ��̴�.
                //time�� left shift �Ѵٴ� ������ �����ʿ� ��Ʈ�� 7�� ä����
                //�װ��� or �ϸ� ���� Ÿ���� ������ ��.
            }
            //127 ���� ���� �ֻ��� ��Ʈ�� 0�̴�. ���� 127���� Ŭ���� time�� ����ϴ� ������ �ݺ��Ѵ�.
            while (b > 127);
            return time;
        }


        //128���� �Ǳ⸦ ��� �����ؾ� �ϴ°�??
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
        //�� ä���� �Ǳ⸦ �̰����� �ٲ���.
        public static byte[] ChangeInstument(int channel, Instruments instruments)
        {
            //Cc (�Ǳ�)
            //�ٵ� ������? 16�����ϱ�.
            //C0�� 192�̴ϱ�,

            byte _event = (byte)(CHANGE_EVENT + channel); //�̺�Ʈ�� ä�� ��ġ�� ( 1 ����Ʈ)
            //�ڿ��� �Ǳ� ����Ʈ ����.
            byte instrument = (byte)instruments;


            return new byte[] { _event, instrument };
        }

        /// <summary>
        /// BPM TO MICRO... MIDI FIle�� SET TEMPO EVENT�� ��Ʈ�� ����ũ���ʷ� �����.
        /// </summary>
        public static int ConvertBpmToMicro(int bpm)
        {
            //�ʴ� ��Ʈ
            double bps = bpm / 60;

            // 1 / bps = 1��Ʈ�� ��
            
            //��Ʈ �� ����ũ�� �� 
            return (int) ((1 / bps) * 1000000);
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
