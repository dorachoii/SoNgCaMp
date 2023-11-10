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

    //�ν����� â���� �ٲ���
    //���ϸ�(�̸�): ��ü �����ֱ�
    public string midiFilePath_name = "Groove.mid.txt";
    public bool ShouldPlayFile = true;

    //Try also: "FM Bank/fm" or "Analog Bank/analog" for some different sounds
    public string bankFilePath = "GM Bank/gm";

    public int bufferSize = 1024;   //���� �����͸� ó���ϴ� ��. ���� ���� latency�� ª��
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
    //1) synthesizer: ���α׷�
    //2) sequencer: ���� �����͸� ����Ͽ� �ڵ� �����ϴ� ���� �������� �� ����Ʈ���� (����Ŀ?)

    //StreamSynthesizer �Ű�����
    //Streaming: ���� �����ϰ� ����ϴ� ���
    //(software) Synthesizer: ������ ������� ������ ��ǻ�� ���α׷�

    // 1) sampleRate: ���� �ӵ� ����.
    // sampleRate�� ���� ���� ������ ũ�Ⱑ �۾�����, ������ ������.

    // 2) audioChannel: �Ҹ��� �� ���� ä���� ǥ���ϴ���
    // 2��: �¿�, 5��: �¿�, �߾�, �Ĺ� ��

    // 3) bufferSize: �Ƴ��α� ��ȣ�� ������ ��ȣ�� ��ȯ�� �� ���� �����͸� ó���ϴ� ��.
    // bufferSize�� ���� ���� latency�� �پ��� ��ǻ�Ͱ� �������.

    // 4) maxpoly: polyphony �ټ� ����, �ܼ� ���� ���� ����
    // ȭ���� �󸶳� ���� �� �ִ����� ������ �� ����

    private float sliderValue = 1.0f;
    private float maxSliderValue = 127.0f;

    // Awake is called when the script instance
    // is being loaded.
    void Awake()
    {
        midiStreamSynthesizer = new StreamSynthesizer(44100, 2, bufferSize, 40);
        sampleBuffer = new float[midiStreamSynthesizer.BufferSize];

        //�Ǳ� ��Ƶ� �� musicBank
        midiStreamSynthesizer.LoadBank(bankFilePath);

        //�������� ����Ŀ�� ������ִ� ����
        midiSequencer = new MidiSequencer(midiStreamSynthesizer);

        //These will be fired by the midiSequencer when a song plays. Check the console for messages if you uncomment these

        midiSequencer.NoteOnEvent += new MidiSequencer.NoteOnEventHandler (MidiNoteOnHandler);
        //midiSequencer.NoteOffEvent += new MidiSequencer.NoteOffEventHandler (MidiNoteOffHandler);			
    }

    //midi�о�ͼ� play����
    void LoadSong(string midiPath)
    {
        midiSequencer.LoadMidi(midiPath, false);
        midiSequencer.Play();
    }

    // Start is called just before any of the
    // Update methods is called the first time.
    void Start()
    {
        //1) ���� load�ϰ� ��⿭(allNoteInfo)�� ��´�.>>> �� ������ ������ �ϴ� ����� ���.
        //1-1) ��� �ؼ� �ֱ�
        //1-2) buffersize �ٿ�����? 
        midiSequencer.LoadMidi(midiFilePath_name, true);



        //������ �޾ƿͼ� ���� ��Ʈ�� �ش��ϴ� ���� �ʿ�.



        //2) ���� ����ϱ� (0~5����) >> pitch�� �ش��ϴ� ���� ��ȣ ���


        //3) ��Ʈ �����ϱ� (short, long, drag) >> note length�� ���� ���� �ö󰡴��� ����
        //4) ���Ϻ� ��⿭(noteInfo_Rails)�� ��´�.
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
            midiSequencer.Stop(true);   //stop, pause ���� ã�ƺ���
        }


        if (Input.GetButtonDown("Fire1"))
        {
            //midiNote: ��
            //midiNoteVolume: velocity
            //midiInstrument: �Ǳ�
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
