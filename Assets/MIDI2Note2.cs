using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CSharpSynth.Effects;
using CSharpSynth.Sequencer;
using CSharpSynth.Synthesis;
using CSharpSynth.Midi;

[RequireComponent(typeof(AudioSource))]
public class MIDI2Note2 : MonoBehaviour
{
    //Public
    //Check the Midi's file folder for different songs

    //인스펙터 창에서 바꿔줌
    //파일명(이름): 전체 적어주기
    public string midiFilePath_name = "Groove.mid.txt";
    public bool ShouldPlayFile = true;

    //Try also: "FM Bank/fm" or "Analog Bank/analog" for some different sounds
    public string bankFilePath = "GM Bank/gm";

    public int bufferSize = 1024;   //샘플 데이터를 처리하는 양. 작을 수록 latency가 짧다
    public int midiNote = 60;
    public int midiNoteVolume = 100;

    [Range(0, 127)] //From Piano to Gunshot
    public int midiInstrument = 0;

    //Private 
    private float[] sampleBuffer;
    private float gain = 1f;
    private MidiSequencer midiSequencer;
    private StreamSynthesizer midiStreamSynthesizer;

    //synthesizer vs sequencer
    //1) synthesizer: 프로그램
    //2) sequencer: 연주 데이터를 재생하여 자동 연주하는 것을 목적으로 한 소프트웨어 (스피커?)

    //StreamSynthesizer 매개변수
    //Streaming: 파일 전송하고 재생하는 방식
    //(software) Synthesizer: 디지털 오디오를 만들어내는 컴퓨터 프로그램

    // 1) sampleRate: 전송 속도 느낌.
    // sampleRate가 낮을 수록 데이터 크기가 작아지고, 전송이 빠르다.

    // 2) audioChannel: 소리가 몇 개의 채널을 표시하는지
    // 2개: 좌우, 5개: 좌우, 중앙, 후방 등

    // 3) bufferSize: 아날로그 신호를 디지털 신호로 변환할 때 샘플 데이터를 처리하는 양.
    // bufferSize가 낮을 수록 latency가 줄어들고 컴퓨터가 힘들어짐.

    // 4) maxpoly: polyphony 다성 음악, 단선 음악 등의 개념
    // 화음을 얼마나 쌓을 수 있는지의 개념인 것 같음

    private float sliderValue = 1.0f;
    private float maxSliderValue = 127.0f;

    // Awake is called when the script instance
    // is being loaded.
    void Awake()
    {
        midiStreamSynthesizer = new StreamSynthesizer(44100, 2, bufferSize, 40);
        sampleBuffer = new float[midiStreamSynthesizer.BufferSize];

        //악기 담아둔 것 musicBank
        midiStreamSynthesizer.LoadBank(bankFilePath);

        //연주해줄 스피커를 배당해주는 느낌
        midiSequencer = new MidiSequencer(midiStreamSynthesizer);

        //These will be fired by the midiSequencer when a song plays. Check the console for messages if you uncomment these

        midiSequencer.NoteOnEvent += new MidiSequencer.NoteOnEventHandler (MidiNoteOnHandler);
        //midiSequencer.NoteOffEvent += new MidiSequencer.NoteOffEventHandler (MidiNoteOffHandler);			
    }

    //midi읽어와서 play까지
    void LoadSong(string midiPath)
    {
        midiSequencer.LoadMidi(midiPath, false);
        midiSequencer.Play();
    }

    // Start is called just before any of the
    // Update methods is called the first time.
    void Start()
    {
        //1) 곡을 load하고 대기열(allNoteInfo)에 담는다.>>> 이 과정을 빠르게 하는 방법을 고안.
        //1-1) 배속 해서 넣기
        //1-2) buffersize 줄여보기? 
        midiSequencer.LoadMidi(midiFilePath_name, true);



        //파일을 받아와서 개별 노트에 해당하는 정보 필요.



        //2) 레일 배분하기 (0~5까지) >> pitch에 해당하는 레일 번호 배당


        //3) 노트 구분하기 (short, long, drag) >> note length와 이전 음과 올라가는지 구분
        //4) 레일별 대기열(noteInfo_Rails)에 담는다.
    }

    // Update is called every frame, if the
    // MonoBehaviour is enabled.
    void Update()
    {
        if (!midiSequencer.isPlaying)
        {
            //if (!GetComponent<AudioSource>().isPlaying)
            if (ShouldPlayFile)
            {
                LoadSong(midiFilePath_name);

            }
        }
        else if (!ShouldPlayFile) { LoadSong(midiFilePath_name); }
        {
            midiSequencer.Stop(true);   //stop, pause 차이 찾아보기
        }


        if (Input.GetButtonDown("Fire1"))
        {
            //midiNote: 음
            //midiNoteVolume: velocity
            //midiInstrument: 악기
            midiStreamSynthesizer.NoteOn(0, midiNote, midiNoteVolume, midiInstrument);
        }

        if (Input.GetButtonUp("Fire1"))
        {
            midiStreamSynthesizer.NoteOff(0, midiNote);
        }


    }
	
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
}
