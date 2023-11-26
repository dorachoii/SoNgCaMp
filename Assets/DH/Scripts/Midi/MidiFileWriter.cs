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


    //이전에 있는 데이터 + 새로운 데이터 
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

            // Ctype 데이터를 newData로 복사
            Array.Copy(ctype, 0, newData, 0, ctype.Length); 

            // Length 데이터를 newData로 복사
            Array.Copy(length, 0, newData, ctype.Length, length.Length);

            // Data 데이터를 newData로 복사
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
            Debug.Log("바귑니다!!");
            short val = IPAddress.NetworkToHostOrder(value);
            byte[] bytes = BitConverter.GetBytes(val);
            Array.Copy(bytes, 0, data, 2,bytes.Length);
        }
    }
    public short Division
    {
        get { return IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, 4)); }
        set {
            //GetBytes 하면 로꾸꺼 들어감
            //NetworkToHostOrder로 변환 후 
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
    public List<MEvent> eventList = new List<MEvent>();
    public TrackChunk(byte[] Ctype, byte[] Length, byte[] Data) : base(Ctype, Length, Data)
    {
        //Data 를 순차적으로 읽으면서 진행 
        int count = 0;
        //델타타임을 계산하고?
        //일단 1바이트를 읽고 


        MEvent prevEvent = null;
        while (count < Data.Length)
        {
            //함수를 안에서 진행한는것이?
            int delta = Checkdelta(Data, ref count);
            
            MEvent evt = MakeEvent(delta, Data, ref count, prevEvent);

            //이게 현재 미디이벤트면 이전 이벤트 저장. 
            if(evt is MidiEvent)
                prevEvent = evt;

            eventList.Add(evt);

        }



    }

    public int Checkdelta(byte[] buffer,ref int count)
    {
        int currentDelta = 0;


        int delta = 0;

        //버퍼가 있을것
        //버퍼에는 바이트가 하나 있다.

        //하나씩 불러와서 
        do
        {
            delta = buffer[count];
            currentDelta = (currentDelta << 8) | (delta);
            count++;
        } while (delta > 127);

        //더이상 deltatime이 없으면 .

        return currentDelta;

    }

    //여기에 이전 이벤트의 정보도 필요하다.
    public MEvent MakeEvent(int deltatime,byte[] buffer,ref int count,MEvent prevEvent) {

        //이전 이벤트가 있으면 현재 상태 바이트는 이전 이벤트의 상태 바이트로 설정
        byte eventType = buffer[count];
        if (eventType == 0xFF) {
            count++;
            return new MetaEvent(deltatime,buffer,ref count,eventType);
        }
        if (eventType > 0x7F && eventType < 0xF0)
        {
            count++;
            return new MidiEvent(deltatime, buffer, ref count, eventType); ;
        }
        if(prevEvent != null)
            return new MidiEvent(deltatime, buffer, ref count, prevEvent.Etpye);
        
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
        
        public int deltatime;
        byte[] data;
        public byte Etpye { get; }
        public MEvent()
        {

        }

        public MEvent(int deltatime,byte etype)
        {
            this.deltatime = deltatime;
            Etpye = etype;
        }
    }


    public class MetaEvent : MEvent
    {
        byte msg;
        byte len;
        byte[] data;
        public MetaEvent(int deltatime, byte[] buffer, ref int count,byte etype) : base(deltatime, etype)
        {
            msg = buffer[count++];
            len = buffer[count++];
            data = new byte[len];
            Array.Copy(buffer, count, data,0, len);
            count += len;
        }
    }

    public class MidiEvent : MEvent
    {
        public int chanel; //어느 채널 이벤트인지
        public byte fData;
        public byte sData;
        public byte etype;

        public MidiEvent(int deltatime, byte[] buffer, ref int count, byte eventType) : base(deltatime,eventType)
        {
            chanel = (eventType & 0xF); 
            etype = (byte)(eventType >> 4);
            fData = buffer[count++];
            //이벤트 타입을 검사
            switch (etype)
            {
                case 0x8:
                    sData = buffer[count++];
                    break;
                case 0x9:
                    sData = buffer[count++];
                    etype = (byte) ((sData == 0) ? 0x8 : etype);
                    break;
                case 0xC:
                    break;
                case 0xB:
                    sData = buffer[count++];
                    break;

            }


            //이벤트 타입을 결정
            // 4비트는 이벤트 타입 //4비트는 채널

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
    public byte[] C_Length = { 0x00, 0x00, 0x00, 0x17 }; //20이되버림
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
        byte[] C_Length = { 0x00, 0x00, 0x00, 0x20 }; //원래 17임!!증가
        byte[] C_Data = { 0x00, 
            0xFF,
            0x51, 0x03, 0x07, 0xA1, 0x20, 0x00,
            0xFF, 0x58, 0x04, 0x04, 0x02, 0x18,
            0x08, 0xC0, 0x41, //여기
            0x00, 0x90, 0x3C, 0x00 };
        TrackChunk track = new TrackChunk(C_Ctype,C_Length,C_Data);


        DummyTrackData Dummy = new DummyTrackData();
        TrackChunk track2 = new TrackChunk(Dummy.C_Ctype,Dummy.C_Length,Dummy.C_Data); 




        byte[] midiData = {
            // Offset 0x00000000 to 0x00000108
            //16진수 1개 = 1바이트
            //헤더 청크 = 4바이트 2바이트 6 바이트
            0x4D, 0x54, 0x68, 0x64, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00, 0x01,
            //청크 끝
            0x00, 0x3C,
            //트랙 청크
            0x4D, 0x54, 0x72, 0x6B, 0x00, 0x00, 0xDD, 0xDD, 0x00, //데이터의 길이 : 원래 1D임.
            //메타이벤트
            0xFF,
            0x51, 0x03, 0x07, 0xA1, 0x20, 0x00,
            0xFF, 0x58, 0x04, 0x04, 0x02, 0x18,
            //Note의 정보. 
            0x08,
            0x00, 0x90, 0x3C, 0x00,


            //추가 정보
            //0xBB,0x15,0x90,0x3C,0x50,
            //0xBB,0x15,0x3C,0x00,
        };

        //Track에 새로운 데이터 쓰기 및 Length 조정하기
        //기존 데이터 + 들어오는 데이터의 길이 
        int dataLength = track.data.Length + data.Length;




        //새로운 데이터 만들기 (기존 + 새로운 데이터 크기만큼의 배열 생성)
        byte[] newData = new byte[dataLength];

        //track에 있는 데이터를 newData에 옮김
        Array.Copy(track.data,newData,track.data.Length);
        //새로운 데이터를 newData에 옮김
        Array.Copy(data, 0, newData, track.data.Length, data.Length);

        track.data = newData;
        byte[] last_data = { 0x00,
        0xFF, 0x2F, 0x00 };


        //eot 4까지 추가
        int orderLength = IPAddress.NetworkToHostOrder(dataLength  + last_data.Length);
        //Byte Order 해야할거같은데..


        //Test Case
        //track.length = BitConverter.GetBytes(orderLength);
        track.Length = dataLength + last_data.Length;

        //header + track buffer 합치기
        byte[] realnewData = new byte[header.Buffer.Length + track.Buffer.Length + last_data.Length];

        Array.Copy(header.Buffer,realnewData,header.Buffer.Length);
        Array.Copy(track.Buffer, 0, realnewData, header.Buffer.Length,track.Buffer.Length);
        Array.Copy(last_data, 0, realnewData, header.Buffer.Length + track.Buffer.Length, last_data.Length);
        //        Array.Copy();

        string path = Application.persistentDataPath + "/files/" + "compose.mid";
        File.WriteAllBytes(path, data);

        Debug.LogError(path);
        //LogManager.instance.Log(path);
        //AssetDatabase.Refresh();
        //3개의 데이터를 병합할 배열 생성 
        //byte[] newdata = new byte[midiData.Length + data.Length + last_data.Length];
        //복사할 대상과 받을 배열, 복사할 갯수
        //Array.Copy(midiData, newdata, midiData.Length);

        //복사할 대상과 받을 배열, 시작 위치, 복사할 개수
        //Array.Copy(data, 0, newdata, midiData.Length,data.Length);
        //Array.Copy(last_data, 0, newdata, midiData.Length + data.Length, last_data.Length);


        //string path =  Application.persistentDataPath + "/example.mid";
        // MIDI 파일 저장
        //Debug.LogWarning(path + "에 저장이 되었습니다.");


        //File.WriteAllBytes(path, newdata);


    }
    



    // Update is called once per frame
    void Update()
    {
        
    }


    private void Awake()
    {
        //ReadMode?
        //WriteMode???
        //ReadMidi();
    }
    //읽어오자고
    public void ReadMidi() {

        MidiFile midiFile = new MidiFile();
        midiFile.Header = new HeaderChunk();

        string filePath = (string)PlayerManager.Get.GetValue("midiPath");
        if (filePath == null)
            return;
        PlayerManager.Get.RemoveValue("midiPath");

        Debug.LogError("ㅇㅇㅇㅇ?!");
        if (File.Exists(filePath))
        {

                using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
                {
                    while (reader.BaseStream.Position < reader.BaseStream.Length) {
                        Debug.LogError("들어왔다!");
                        // 여기에 파일에서 데이터를 읽어오는 코드를 추가하세요.
                        // 예를 들어, reader.ReadBytes()를 사용하여 바이트 배열을 읽어올 수 있습니다.
                        byte[] ctype = reader.ReadBytes(4);
                        byte[] length = reader.ReadBytes(4);
                        int len = BitConverter.ToInt32(length, 0);

                        len = IPAddress.NetworkToHostOrder(len);
                        byte[] buffer = reader.ReadBytes(len);

                        int ctypeI = BitConverter.ToInt32(ctype); //거꾸로 되어있는데?
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
        else
        {
            Debug.LogError("파일이 존재하지 않습니다: " + filePath);
        }

    }

    //미디파일을 읽어서 Midifile클래스로 리턴~!
    public static MidiFile ReadMidi(string filename)
    {

        MidiFile midiFile = new MidiFile();
        midiFile.Header = new HeaderChunk();

        string filePath = Application.persistentDataPath + "/" +  filename;


        Debug.LogError("ㅇㅇㅇㅇ?!");
        if (File.Exists(filePath))
        {

            using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
            {
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    Debug.LogError("들어왔다!");
                    // 여기에 파일에서 데이터를 읽어오는 코드를 추가하세요.
                    // 예를 들어, reader.ReadBytes()를 사용하여 바이트 배열을 읽어올 수 있습니다.
                    byte[] ctype = reader.ReadBytes(4);
                    byte[] length = reader.ReadBytes(4);
                    int len = BitConverter.ToInt32(length, 0);

                    len = IPAddress.NetworkToHostOrder(len);
                    byte[] buffer = reader.ReadBytes(len);

                    int ctypeI = BitConverter.ToInt32(ctype); //거꾸로 되어있는데?
                    ctypeI = IPAddress.NetworkToHostOrder(ctypeI);

                    switch (ctypeI)
                    {
                        case 0x4d546864:
                            midiFile.Header = new HeaderChunk(ctype, length, buffer);
                            break;
                        case 0x4d54726b:
                            midiFile.TrackLsit.Add(new TrackChunk(ctype, length, buffer));
                            break;
                    }
                }

                Debug.Log("파일 읽기 성공");
                return midiFile;

            }


        }
        else
        {
            Debug.LogError("파일이 존재하지 않습니다: " + filePath);
            return null;
        }

    }
}
