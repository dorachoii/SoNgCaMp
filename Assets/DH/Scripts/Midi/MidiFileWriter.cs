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


    //戚穿拭 赤澗 汽戚斗 + 歯稽錘 汽戚斗 
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

            // Ctype 汽戚斗研 newData稽 差紫
            Array.Copy(ctype, 0, newData, 0, ctype.Length); 

            // Length 汽戚斗研 newData稽 差紫
            Array.Copy(length, 0, newData, ctype.Length, length.Length);

            // Data 汽戚斗研 newData稽 差紫
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
            Debug.Log("郊衛艦陥!!");
            short val = IPAddress.NetworkToHostOrder(value);
            byte[] bytes = BitConverter.GetBytes(val);
            Array.Copy(bytes, 0, data, 2,bytes.Length);
        }
    }
    public short Division
    {
        get { return IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, 4)); }
        set {
            //GetBytes 馬檎 稽荷襖 級嬢姶
            //NetworkToHostOrder稽 痕発 板 
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
        //Data 研 授託旋生稽 石生檎辞 遭楳 
        int count = 0;
        //季展展績聖 域至馬壱?
        //析舘 1郊戚闘研 石壱 


        MEvent prevEvent = null;
        while (count < Data.Length)
        {
            //敗呪研 照拭辞 遭楳廃澗依戚?
            int delta = Checkdelta(Data, ref count);
            
            MEvent evt = MakeEvent(delta, Data, ref count, prevEvent);

            //戚惟 薄仙 耕巨戚坤闘檎 戚穿 戚坤闘 煽舌. 
            if(evt is MidiEvent)
                prevEvent = evt;

            eventList.Add(evt);

        }



    }

    public int Checkdelta(byte[] buffer,ref int count)
    {
        int currentDelta = 0;


        int delta = 0;

        //獄遁亜 赤聖依
        //獄遁拭澗 郊戚闘亜 馬蟹 赤陥.

        //馬蟹梢 災君人辞 
        do
        {
            delta = buffer[count];
            currentDelta = (currentDelta << 8) | (delta);
            count++;
        } while (delta > 127);

        //希戚雌 deltatime戚 蒸生檎 .

        return currentDelta;

    }

    //食奄拭 戚穿 戚坤闘税 舛左亀 琶推馬陥.
    public MEvent MakeEvent(int deltatime,byte[] buffer,ref int count,MEvent prevEvent) {

        //戚穿 戚坤闘亜 赤生檎 薄仙 雌殿 郊戚闘澗 戚穿 戚坤闘税 雌殿 郊戚闘稽 竺舛
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
        public int chanel; //嬢汗 辰確 戚坤闘昔走
        public byte fData;
        public byte sData;
        public byte etype;

        public MidiEvent(int deltatime, byte[] buffer, ref int count, byte eventType) : base(deltatime,eventType)
        {
            chanel = (eventType & 0xF); 
            etype = (byte)(eventType >> 4);
            fData = buffer[count++];
            //戚坤闘 展脊聖 伊紫
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


            //戚坤闘 展脊聖 衣舛
            // 4搾闘澗 戚坤闘 展脊 //4搾闘澗 辰確

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
    public byte[] C_Length = { 0x00, 0x00, 0x00, 0x17 }; //20戚鞠獄顕
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
        byte[] C_Length = { 0x00, 0x00, 0x00, 0x20 }; //据掘 17績!!装亜
        byte[] C_Data = { 0x00, 
            0xFF,
            0x51, 0x03, 0x07, 0xA1, 0x20, 0x00,
            0xFF, 0x58, 0x04, 0x04, 0x02, 0x18,
            0x08, 0xC0, 0x41, //食奄
            0x00, 0x90, 0x3C, 0x00 };
        TrackChunk track = new TrackChunk(C_Ctype,C_Length,C_Data);


        DummyTrackData Dummy = new DummyTrackData();
        TrackChunk track2 = new TrackChunk(Dummy.C_Ctype,Dummy.C_Length,Dummy.C_Data); 




        byte[] midiData = {
            // Offset 0x00000000 to 0x00000108
            //16遭呪 1鯵 = 1郊戚闘
            //伯希 短滴 = 4郊戚闘 2郊戚闘 6 郊戚闘
            0x4D, 0x54, 0x68, 0x64, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00, 0x01,
            //短滴 魁
            0x00, 0x3C,
            //闘窟 短滴
            0x4D, 0x54, 0x72, 0x6B, 0x00, 0x00, 0xDD, 0xDD, 0x00, //汽戚斗税 掩戚 : 据掘 1D績.
            //五展戚坤闘
            0xFF,
            0x51, 0x03, 0x07, 0xA1, 0x20, 0x00,
            0xFF, 0x58, 0x04, 0x04, 0x02, 0x18,
            //Note税 舛左. 
            0x08,
            0x00, 0x90, 0x3C, 0x00,


            //蓄亜 舛左
            //0xBB,0x15,0x90,0x3C,0x50,
            //0xBB,0x15,0x3C,0x00,
        };

        //Track拭 歯稽錘 汽戚斗 床奄 貢 Length 繕舛馬奄
        //奄糎 汽戚斗 + 級嬢神澗 汽戚斗税 掩戚 
        int dataLength = track.data.Length + data.Length;




        //歯稽錘 汽戚斗 幻級奄 (奄糎 + 歯稽錘 汽戚斗 滴奄幻鏑税 壕伸 持失)
        byte[] newData = new byte[dataLength];

        //track拭 赤澗 汽戚斗研 newData拭 薪沿
        Array.Copy(track.data,newData,track.data.Length);
        //歯稽錘 汽戚斗研 newData拭 薪沿
        Array.Copy(data, 0, newData, track.data.Length, data.Length);

        track.data = newData;
        byte[] last_data = { 0x00,
        0xFF, 0x2F, 0x00 };


        //eot 4猿走 蓄亜
        int orderLength = IPAddress.NetworkToHostOrder(dataLength  + last_data.Length);
        //Byte Order 背醤拝暗旭精汽..


        //Test Case
        //track.length = BitConverter.GetBytes(orderLength);
        track.Length = dataLength + last_data.Length;

        //header + track buffer 杯帖奄
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
        //3鯵税 汽戚斗研 佐杯拝 壕伸 持失 
        //byte[] newdata = new byte[midiData.Length + data.Length + last_data.Length];
        //差紫拝 企雌引 閤聖 壕伸, 差紫拝 姐呪
        //Array.Copy(midiData, newdata, midiData.Length);

        //差紫拝 企雌引 閤聖 壕伸, 獣拙 是帖, 差紫拝 鯵呪
        //Array.Copy(data, 0, newdata, midiData.Length,data.Length);
        //Array.Copy(last_data, 0, newdata, midiData.Length + data.Length, last_data.Length);


        //string path =  Application.persistentDataPath + "/example.mid";
        // MIDI 督析 煽舌
        //Debug.LogWarning(path + "拭 煽舌戚 鞠醸柔艦陥.");


        //File.WriteAllBytes(path, newdata);


    }
    



    // Update is called once per frame
    void Update()
    {
        
    }


    private void Awake()
    {
        //ReadMidi();
    }
    //石嬢神切壱
    public void ReadMidi() {

        MidiFile midiFile = new MidiFile();
        midiFile.Header = new HeaderChunk();

        string filePath = Application.persistentDataPath + "/example.mid.txt";


        Debug.LogError("しししし?!");
        if (File.Exists(filePath))
        {

                using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
                {
                    while (reader.BaseStream.Position < reader.BaseStream.Length) {
                        Debug.LogError("級嬢尽陥!");
                        // 食奄拭 督析拭辞 汽戚斗研 石嬢神澗 坪球研 蓄亜馬室推.
                        // 森研 級嬢, reader.ReadBytes()研 紫遂馬食 郊戚闘 壕伸聖 石嬢臣 呪 赤柔艦陥.
                        byte[] ctype = reader.ReadBytes(4);
                        byte[] length = reader.ReadBytes(4);
                        int len = BitConverter.ToInt32(length, 0);

                        len = IPAddress.NetworkToHostOrder(len);
                        byte[] buffer = reader.ReadBytes(len);

                        int ctypeI = BitConverter.ToInt32(ctype); //暗荷稽 鞠嬢赤澗汽?
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
            Debug.LogError("督析戚 糎仙馬走 省柔艦陥: " + filePath);
        }

    }

    //耕巨督析聖 石嬢辞 Midifile適掘什稽 軒渡~!
    public static MidiFile ReadMidi(string filename)
    {

        MidiFile midiFile = new MidiFile();
        midiFile.Header = new HeaderChunk();

        string filePath = Application.persistentDataPath + "/" + filename;


        Debug.LogError("しししし?!");
        if (File.Exists(filePath))
        {

            using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
            {
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    Debug.LogError("級嬢尽陥!");
                    // 食奄拭 督析拭辞 汽戚斗研 石嬢神澗 坪球研 蓄亜馬室推.
                    // 森研 級嬢, reader.ReadBytes()研 紫遂馬食 郊戚闘 壕伸聖 石嬢臣 呪 赤柔艦陥.
                    byte[] ctype = reader.ReadBytes(4);
                    byte[] length = reader.ReadBytes(4);
                    int len = BitConverter.ToInt32(length, 0);

                    len = IPAddress.NetworkToHostOrder(len);
                    byte[] buffer = reader.ReadBytes(len);

                    int ctypeI = BitConverter.ToInt32(ctype); //暗荷稽 鞠嬢赤澗汽?
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

                Debug.Log("督析 石奄 失因");
                return midiFile;

            }


        }
        else
        {
            Debug.LogError("督析戚 糎仙馬走 省柔艦陥: " + filePath);
            return null;
        }

    }
}
