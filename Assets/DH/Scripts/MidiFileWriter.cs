using DHMidi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
//using UnityEditor;
using DHMidi.Event;

public class Chunk {
    private byte[] ctype;
    public byte[] length;
    public byte[] data;

    public int Ctype
    {
        get;
        set;

    }


    //������ �ִ� ������ + ���ο� ������ 
    public byte[] AddData
    {
        get { return data; }
        set
        {
            byte[] newData = new byte[AddData.Length + value.Length];
            Array.Copy(AddData, newData, AddData.Length);
            Array.Copy(value,0, newData, AddData.Length, value.Length);
            data = newData;
            Length = data.Length;
        }
    }

    public int Length
    {
        get { return length.Length; }
        set
        {
            int val = IPAddress.NetworkToHostOrder(value); 
            length = BitConverter.GetBytes(val);

        }
    }

    public byte[] Buffer {
        get {
            byte[] newData = new byte[ctype.Length + length.Length + data.Length]; //buffer

            // Ctype �����͸� newData�� ����
            Array.Copy(ctype, 0, newData, 0, ctype.Length); 

            // Length �����͸� newData�� ����
            Array.Copy(length, 0, newData, ctype.Length, length.Length);

            // Data �����͸� newData�� ����
            Array.Copy(data, 0, newData, ctype.Length + length.Length, data.Length);

            return newData;
        }
        
    }
    public Chunk() { }
    public Chunk(byte[] Ctype,byte[]Length,byte[]Data) {
        this.ctype = Ctype;
        this.length = Length;
        this.data = Data;
    }

}
public class HeaderChunk : Chunk
{

    public short Format
    {
        get { return IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data,0)); }
        set {
            short val = IPAddress.NetworkToHostOrder(value);
            byte[] bytes = BitConverter.GetBytes(val);
            Array.Copy(bytes, 0, data, 0,bytes.Length);
            
        }
    }
    public short TrackCount { 
        get { return IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, 2)); }
        set {
            Debug.Log("�ٱҴϴ�!!");
            short val = IPAddress.NetworkToHostOrder(value);
            byte[] bytes = BitConverter.GetBytes(val);
            Array.Copy(bytes, 0, data, 2,bytes.Length);
        }
    }
    public short Division
    {
        get { return IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, 4)); }
        set {
            //GetBytes �ϸ� �βٲ� ��
            //NetworkToHostOrder�� ��ȯ �� 
            short val = IPAddress.NetworkToHostOrder(value);
            byte[] bytes = BitConverter.GetBytes(val);
            Array.Copy(bytes, 0, data, 4, bytes.Length);
        }
    }


    public HeaderChunk() : base() { }

    public HeaderChunk(byte[] Ctype, byte[] Length, byte[] Data) : base(Ctype, Length, Data)
    {

    }
}
public class TrackChunk : Chunk
{
    //event..-\
    List<MEvent> eventList = new List<MEvent>();
    public TrackChunk(byte[] Ctype, byte[] Length, byte[] Data) : base(Ctype, Length, Data)
    {
        //Data �� ���������� �����鼭 ���� 
        int count = 0;
        //��ŸŸ���� ����ϰ�?
        //�ϴ� 1����Ʈ�� �а� 

        //while (count < Data.Length) {
        //    //�Լ��� �ȿ��� �����Ѵ°���?
        //    int delta = Checkdelta(Data, ref count);
        //    MEvent evt = MakeEvent(delta,Data,ref count);
        //    eventList.Add(evt);

        //}

        
        
    }

    public int Checkdelta(byte[] buffer,ref int count)
    {
        int currentDelta = 0;


        int delta = 0;

        //���۰� ������
        //���ۿ��� ����Ʈ�� �ϳ� �ִ�.

        //�ϳ��� �ҷ��ͼ� 
        do
        {
            delta = buffer[count];
            currentDelta = (currentDelta << 8) | (delta);
            count++;
        } while (delta > 127);

        //���̻� deltatime�� ������ .

        return currentDelta;

    }

    public MEvent MakeEvent(int deltatime,byte[] buffer,ref int count) {
        byte eventType = buffer[count];
        if (eventType == 0xFF) {
            return new MetaEvent(deltatime,buffer,ref count);
        }
        if (eventType > 0xF0) {
            return new MidiEvent(deltatime, buffer, ref count,eventType);
        }
        return null;
                //byte msg = buffer[++count];
                //byte len = buffer[++count];
                //byte[] data = new byte[len];
                //Array.Copy(buffer,++count, data, 0,data.Length);
                //count += data.Length;
                //return new MetaEvent(eventType,msg,len,data);

            
    }
    
}


namespace DHMidi.Event {
    public class MEvent
    {
        byte[] deltatime;
        byte[] data;

        public MEvent()
        {

        }

        public MEvent(int deltatime, byte[] data)
        {

        }
    }


    public class MetaEvent : MEvent
    {
        byte etype;
        byte msg;
        byte len;
        byte[] data;
        public MetaEvent(int deltatime, byte[] buffer, ref int count)
        {

        }
    }

    public class MidiEvent : MEvent
    {
        int chanel; //��� ä�� �̺�Ʈ����
        byte fData;
        byte sData;
        byte eventType;

        public MidiEvent(int deltatime, byte[] buffer, ref int count, byte eventType)
        {
            chanel = (eventType & 0xF);
            this.eventType = (byte)(eventType >> 4);

            //�̺�Ʈ Ÿ���� �˻�
            switch (eventType)
            {
                case 0x8:
                    fData = buffer[++count];
                    sData = buffer[++count];
                    break;
                case 0x9:
                    fData = buffer[++count];
                    sData = buffer[++count];
                    break;

            }
            //�̺�Ʈ Ÿ���� ����
            // 4��Ʈ�� �̺�Ʈ Ÿ�� //4��Ʈ�� ä��

        }
    }

}


public class DummyHeaderData {
    public byte[] H_Ctype = { 0x4D, 0x54, 0x68, 0x64 };
    public byte[] H_Length = { 0x00, 0x00, 0x00, 0x06 };    
    public byte [] H_Data = { 0x00, 0x00, 0x00, 0x01, 0x00, 0x3C };  //Format... trackCount ... //Division

}
public class DummyTrackData {
    public byte[] C_Ctype = { 0x4D, 0x54, 0x72, 0x6B };
    public byte[] C_Length = { 0x00, 0x00, 0x00, 0x17 }; //20�̵ǹ���
    public byte[] C_Data = { 0x00,        
            0xFF,
            0x51, 0x03, 0x07, 0xA1, 0x20, 0x00,
            0xFF, 0x58, 0x04, 0x04, 0x02, 0x18,
            0x08
            };
}
public class MidiFileWriter : MonoBehaviour
{


    //public static HeaderChunk chunk = new HeaderChunk();
    void Start()
    {
        
    }
    

    
    //22 + a
    static public void write(byte[] data) {
        
        //Temp Data;
        byte[] H_Ctype = { 0x4D, 0x54, 0x68, 0x64 };
        byte[] H_Length = { 0x00, 0x00, 0x00, 0x06 };
        byte[] H_Data = { 0x00, 0x00, 0x00, 0x01, 0x00, 0x3C };
        HeaderChunk header = new HeaderChunk(H_Ctype,H_Length,H_Data);



        byte[] C_Ctype = { 0x4D, 0x54, 0x72, 0x6B };
        byte[] C_Length = { 0x00, 0x00, 0x00, 0x20 }; //���� 17��!!����
        byte[] C_Data = { 0x00, 
            0xFF,
            0x51, 0x03, 0x07, 0xA1, 0x20, 0x00,
            0xFF, 0x58, 0x04, 0x04, 0x02, 0x18,
            0x08, 0xC0, 0x41, //����
            0x00, 0x90, 0x3C, 0x00 };
        TrackChunk track = new TrackChunk(C_Ctype,C_Length,C_Data);


        DummyTrackData Dummy = new DummyTrackData();
        TrackChunk track2 = new TrackChunk(Dummy.C_Ctype,Dummy.C_Length,Dummy.C_Data); 




        byte[] midiData = {
            // Offset 0x00000000 to 0x00000108
            //16���� 1�� = 1����Ʈ
            //��� ûũ = 4����Ʈ 2����Ʈ 6 ����Ʈ
            0x4D, 0x54, 0x68, 0x64, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00, 0x01,
            //ûũ ��
            0x00, 0x3C,
            //Ʈ�� ûũ
            0x4D, 0x54, 0x72, 0x6B, 0x00, 0x00, 0xDD, 0xDD, 0x00, //�������� ���� : ���� 1D��.
            //��Ÿ�̺�Ʈ
            0xFF,
            0x51, 0x03, 0x07, 0xA1, 0x20, 0x00,
            0xFF, 0x58, 0x04, 0x04, 0x02, 0x18,
            //Note�� ����. 
            0x08,
            0x00, 0x90, 0x3C, 0x00,


            //�߰� ����
            //0xBB,0x15,0x90,0x3C,0x50,
            //0xBB,0x15,0x3C,0x00,
        };

        //Track�� ���ο� ������ ���� �� Length �����ϱ�
        //���� ������ + ������ �������� ���� 
        int dataLength = track.data.Length + data.Length;




        //���ο� ������ ����� (���� + ���ο� ������ ũ�⸸ŭ�� �迭 ����)
        byte[] newData = new byte[dataLength];

        //track�� �ִ� �����͸� newData�� �ű�
        Array.Copy(track.data,newData,track.data.Length);
        //���ο� �����͸� newData�� �ű�
        Array.Copy(data, 0, newData, track.data.Length, data.Length);

        track.data = newData;
        byte[] last_data = { 0x00,
        0xFF, 0x2F, 0x00 };


        //eot 4���� �߰�
        int orderLength = IPAddress.NetworkToHostOrder(dataLength  + last_data.Length);
        //Byte Order �ؾ��ҰŰ�����..


        //Test Case
        //track.length = BitConverter.GetBytes(orderLength);
        track.Length = dataLength + last_data.Length;

        //header + track buffer ��ġ��
        byte[] realnewData = new byte[header.Buffer.Length + track.Buffer.Length + last_data.Length];

        Array.Copy(header.Buffer,realnewData,header.Buffer.Length);
        Array.Copy(track.Buffer, 0, realnewData, header.Buffer.Length,track.Buffer.Length);
        Array.Copy(last_data, 0, realnewData, header.Buffer.Length + track.Buffer.Length, last_data.Length);
        //        Array.Copy();

        string path = Application.persistentDataPath + "/" + "example.mid.txt";
        File.WriteAllBytes(path, data);

        Debug.LogError(path);
        //LogManager.instance.Log(path);
        //AssetDatabase.Refresh();
        //3���� �����͸� ������ �迭 ���� 
        //byte[] newdata = new byte[midiData.Length + data.Length + last_data.Length];
        //������ ���� ���� �迭, ������ ����
        //Array.Copy(midiData, newdata, midiData.Length);

        //������ ���� ���� �迭, ���� ��ġ, ������ ����
        //Array.Copy(data, 0, newdata, midiData.Length,data.Length);
        //Array.Copy(last_data, 0, newdata, midiData.Length + data.Length, last_data.Length);


        //string path =  Application.persistentDataPath + "/example.mid";
        // MIDI ���� ����
        //Debug.LogWarning(path + "�� ������ �Ǿ����ϴ�.");


        //File.WriteAllBytes(path, newdata);


    }
    



    // Update is called once per frame
    void Update()
    {
        
    }


    private void Awake()
    {
        ReadMidi();
    }
    //�о���ڰ�
    public void ReadMidi() {

        MidiFile midiFile = new MidiFile();
        midiFile.Header = new HeaderChunk();

        string filePath = Application.persistentDataPath + "/example.mid.txt";


        Debug.LogError("��������?!");
        if (File.Exists(filePath))
        {
            try
            {
                using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
                {
                    while (reader.BaseStream.Position < reader.BaseStream.Length) {
                        Debug.LogError("���Դ�!");
                        // ���⿡ ���Ͽ��� �����͸� �о���� �ڵ带 �߰��ϼ���.
                        // ���� ���, reader.ReadBytes()�� ����Ͽ� ����Ʈ �迭�� �о�� �� �ֽ��ϴ�.
                        byte[] ctype = reader.ReadBytes(4);
                        byte[] length = reader.ReadBytes(4);
                        int len = BitConverter.ToInt32(length, 0);

                        len = IPAddress.NetworkToHostOrder(len);
                        byte[] buffer = reader.ReadBytes(len);

                        int ctypeI = BitConverter.ToInt32(ctype); //�Ųٷ� �Ǿ��ִµ�?
                        ctypeI = IPAddress.NetworkToHostOrder(ctypeI);

                        switch (ctypeI)
                        {
                            case 0x4d546864:
                                midiFile.Header = new HeaderChunk(ctype,length,buffer);
                                break;
                            case 0x4d54726b:
                                midiFile.TrackLsit.Add(new TrackChunk(ctype, length, buffer));
                                break;
                        }
                    }

                    Debug.Log(midiFile);
                   
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("���� �б� ����: " + e.Message);
            }
        }
        else
        {
            Debug.LogError("������ �������� �ʽ��ϴ�: " + filePath);
        }

    }
}