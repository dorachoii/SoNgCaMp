

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CSharpSynth.Effects;
using CSharpSynth.Sequencer;
using CSharpSynth.Synthesis;
using CSharpSynth.Midi;
using Melanchall.DryWetMidi.MusicTheory;
using TMPro;

[RequireComponent(typeof(AudioSource))]

public class EJSongPlayer : MonoBehaviour
{
    #region MP3 Play
    public AudioSource audio;
    public AudioClip[] songs;

    int playCount = 0;

    public EJNoteManager noteManager;
    public GameObject pauseCanvas;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Note"))
        {
            if (!audio.isPlaying)
            {
                int songindex = PlayerPrefs.GetInt("SongIndex");

                if (songindex > 3)
                {
                    songindex = 2;
                }

                audio.PlayOneShot(songs[songindex]);
                playCount++;
            }
        }
    }
    #endregion

    #region MIDI Play
    public string midiFilePath = "PianoTest1234.mid.txt";
    public bool ShouldPlayFile = true;


    //Try also: "FM Bank/fm" or "Analog Bank/analog" for some different sounds
    public string bankFilePath = "GM Bank/gm";
    public int bufferSize = 1024;
    public int midiNote = /*60*/ 100;
    public int midiNoteVolume = 100;

    [Range(0, 127)] //From Piano to Gunshot
    public int midiInstrument = 0;


    //Private 
    private float[] sampleBuffer;
    private float gain = 1f;

    public MidiSequencer midiSequencer;
    private StreamSynthesizer midiStreamSynthesizer;

    private float sliderValue = 1.0f;
    private float maxSliderValue = 127.0f;

    public float currTime;
    int currIndex;
    float delayTime = 0.5f;


    private void Awake()
    {
        midiStreamSynthesizer = new StreamSynthesizer(44100, 2, bufferSize, 40);
        sampleBuffer = new float[midiStreamSynthesizer.BufferSize];
        midiStreamSynthesizer.LoadBank(bankFilePath);
        midiSequencer = new MidiSequencer(midiStreamSynthesizer);

        //LoadSong(midiFilePath);

        //These will be fired by the midiSequencer when a song plays. Check the console for messages if you uncomment these
        //midiSequencer.NoteOnEvent += new MidiSequencer.NoteOnEventHandler(MidiNoteOnHandler);
        //midiSequencer.NoteOffEvent += new MidiSequencer.NoteOffEventHandler (MidiNoteOffHandler);
    }


    void LoadSong(string midiPath)
    {
        midiSequencer.LoadMidi(midiPath, false);
        midiSequencer.Play();
        currTime = 0;
    }

    bool isFXplayed;
    // Update is called once per frame
    void Update()
    {
        if (!audio.isPlaying)
        {
            if (playCount > 0 && !isFXplayed && !pauseCanvas.activeSelf)
            {
                isFXplayed = true;
                noteManager.startcoFinaleFX();
                EJGameUIManager.instance.successUI();
            }
        }


        #region customÄÚµå

        //currTime += Time.deltaTime;

        //if (currIndex <= midiSequencer.midiAllNoteEventsDic[0].Count && currTime > delayTime)
        //{

        //    if (currIndex > 0)
        //    {
        //        midiStreamSynthesizer.NoteOff(0, midiSequencer.midiAllNoteEventsDic[0][currIndex - 1].pitch);
        //    }

        //    if (currIndex < midiSequencer.midiAllNoteEventsDic[0].Count)
        //    {
        //        currTime -= delayTime;

        //        midiStreamSynthesizer.NoteOn(0, midiSequencer.midiAllNoteEventsDic[0][currIndex].pitch, midiNoteVolume, midiInstrument);


        //        delayTime = midiSequencer.midiAllNoteEventsDic[0][currIndex].length;
        //        currIndex++;
        //    }
        //}
        #endregion

        //if (!midiSequencer.isPlaying)
        //{
        //    //if (!GetComponent<AudioSource>().isPlaying)
        //    if (ShouldPlayFile)
        //    {
        //        //  LoadSong(midiFilePath);               
        //    }
        //}
        //else if (!ShouldPlayFile)
        //{
        //    midiSequencer.Stop(true);
        //}
    }

    #endregion

    #region default NoteHandlers
    private void OnAudioFilterRead(float[] data, int channels)
    {
        //This uses the Unity specific float method we added to get the buffer
        midiStreamSynthesizer.GetNext(sampleBuffer);

        for (int i = 0; i < data.Length; i++)
        {
            data[i] = sampleBuffer[i] * gain;
        }
    }

    public void MidiNoteOnHandler(int channel, int note, int velocity)
    {
        Debug.Log("NoteOn: " + note.ToString() + " Velocity: " + velocity.ToString());

    }

    public void MidiNoteOffHandler(int channel, int note)
    {
        Debug.Log("NoteOff: " + note.ToString());
    }
    #endregion

}
