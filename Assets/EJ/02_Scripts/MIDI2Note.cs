using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Composing;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Multimedia;
using Melanchall.DryWetMidi.Standards;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class MIDI2Note : MonoBehaviour
{
    public string sss;

    // Start is called before the first frame update
    void Start()
    {
        var midiFile = MidiFile.Read("C:/Users/user/Downloads/" + sss + ".mid");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //MidiFile


    MidiFile midi;
    private MidiFile midi2Notes()
    {
        //if (�ð� > long) �ճ�Ʈ�� �����;
        //if(�ð� > mid) �巡�� ��Ʈ�� �����;
        //else��� short�� �����

        return midi;
    }
}
