using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DHMidi
{
    public class MidiFile 
    {
        public HeaderChunk Header = new HeaderChunk();
        public List<TrackChunk> TrackLsit = new List<TrackChunk>();

        public MidiFile() {
            DummyHeaderData dum = new DummyHeaderData();
            Header = new HeaderChunk(dum.H_Ctype, dum.H_Length, dum.H_Data);
        }

        public byte[] GetData() {
            Header.TrackCount = (short)TrackLsit.Count;

            List<byte> bytelist = new List<byte>();
            bytelist.AddRange(Header.Buffer);

            TrackLsit.ForEach(track => {
                bytelist.AddRange(track.Buffer);
            });


            return bytelist.ToArray();            
        }
    }
}


