using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
//using UnityEditor;

public class Chunk {
    private byte[] ctype;
    public byte[] length;
    public byte[] data;


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
    
    public TrackChunk(byte[] Ctype, byte[] Length, byte[] Data) : base(Ctype, Length, Data)
    {
        
    }
}

public class DummyHeaderData {
    public byte[] H_Ctype = { 0x4D, 0x54, 0x68, 0x64 };
    public byte[] H_Length = { 0x00, 0x00, 0x00, 0x06 };
    public byte [] H_Data = { 0x00, 0x00, 0x00, 0x01, 0x00, 0x3C };

}
public class DummyTrackData {
    public byte[] C_Ctype = { 0x4D, 0x54, 0x72, 0x6B };
    public byte[] C_Length = { 0x00, 0x00, 0x00, 0x17 };
    public byte[] C_Data = { 0x00,
            0xFF,
            0x51, 0x03, 0x07, 0xA1, 0x20, 0x00,
            0xFF, 0x58, 0x04, 0x04, 0x02, 0x18,
            0x08,
            0x00, 0x90, 0x3C, 0x00 };
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
        byte[] C_Length = { 0x00, 0x00, 0x00, 0x17 };
        byte[] C_Data = { 0x00, 
            0xFF,
            0x51, 0x03, 0x07, 0xA1, 0x20, 0x00,
            0xFF, 0x58, 0x04, 0x04, 0x02, 0x18,
            0x08,
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

        string path = Application.persistentDataPath + "/" + "example.mid.txt";
        File.WriteAllBytes(path, data);

        LogManager.instance.Log(path);
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
}
