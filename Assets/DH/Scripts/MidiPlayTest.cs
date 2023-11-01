using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MidiPlayTest : MonoBehaviour
{
    public string midiFilePath = "example.mid"; 
    AndroidJavaObject midiPlayer;
    // Start is called before the first frame update
    void Start()
    {
        string path = Application.persistentDataPath + midiFilePath;
        LogManager.instance.Log("��� �αװ� �������°��� �𸣰ڳ� ��¥");
        midiPlayer = new AndroidJavaObject("com.unity3d.midi.MidiPlayer");



        if (File.Exists(midiFilePath))
        {
            LogManager.instance.Log("Playing");
            midiPlayer.Call("loadMidiFile", midiFilePath);
            midiPlayer.Call("play");
        }
        else {
            LogManager.instance.Log("File Is Not Exist");
                    
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
