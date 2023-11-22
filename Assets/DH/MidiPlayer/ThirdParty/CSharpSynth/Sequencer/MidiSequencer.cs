using System;
using System.Collections.Generic;
using CSharpSynth.Midi;
using CSharpSynth.Synthesis;
using CSharpSynth.Banks;
using UnityEngine;
using Unity.VisualScripting.FullSerializer;
using UnityEngine.Assertions.Must;

namespace CSharpSynth.Sequencer
{
    //역할: 연주 데이터를 입력해줄 함수들만 존재
    //01. LoadMidi
    //02. 그 외 다양한 연주 함수 (Play, Stop, Mute 등)

    public class MidiSequencer
    {
        //--Custom Dictionary
        //LoadMidi 함수 안에서 midiNoteInfo를 담아 줄 딕셔너리 선언
        //key -- int: 악기 정보
        //value -- midiNoteInfo : event
        public Dictionary<int, List<MidiEventInfo>> midiAllNoteEventsDic = new Dictionary<int, List<MidiEventInfo>>();

        //--Variables
        #region default variables
        private MidiFile _MidiFile;
        private StreamSynthesizer synth;
        private int[] currentPrograms;
        private List<byte> blockList;
        private double PitchWheelSemitoneRange = 2.0;
        private bool playing = false;
        private bool looping = false;
        private MidiSequencerEvent seqEvt;
        private int sampleTime;
        private int eventIndex;
        #endregion

        //--Events
        #region default events
        public delegate void NoteOnEventHandler(int channel, int note, int velocity);
        public event NoteOnEventHandler NoteOnEvent;
        public delegate void NoteOffEventHandler(int channel, int note);
        public event NoteOffEventHandler NoteOffEvent;
        #endregion

        //--Public Properties
        #region default properties
        public bool isPlaying
        {
            get { return playing; }
        }
        public int SampleTime
        {
            get { return sampleTime; }
        }
        public int EndSampleTime
        {
            get { return (int)_MidiFile.Tracks[0].TotalTime; }
        }
        public TimeSpan EndTime
        {
            get { return new TimeSpan(0, 0, (int)SynthHelper.getTimeFromSample(synth.SampleRate, (int)_MidiFile.Tracks[0].TotalTime)); }
        }
        public TimeSpan Time
        {
            get { return new TimeSpan(0, 0, (int)SynthHelper.getTimeFromSample(synth.SampleRate, sampleTime)); }
            set { SetTime(value); }
        }
        public double PitchWheelRange
        {
            get { return PitchWheelSemitoneRange; }
            set { PitchWheelSemitoneRange = value; }
        }
        #endregion

        //--Public Methods
        #region default Methods
        public MidiSequencer(StreamSynthesizer synth)
        {
            currentPrograms = new int[16]; //16 channels
            this.synth = synth;
            this.synth.setSequencer(this);
            blockList = new List<byte>();
            seqEvt = new MidiSequencerEvent();
        }

        public void setProgram(int channel, int program)
        {
            currentPrograms[channel] = program;
        }
        public bool Looping
        {
            get { return looping; }
            set { looping = value; }
        }

        #endregion

        #region EJUsing: program Methods - 악기정보 함수
        public string getProgramName(int channel)
        {
            if (channel == 9)
                return synth.SoundBank.getInstrument(currentPrograms[channel], true).Name;
            else
                return synth.SoundBank.getInstrument(currentPrograms[channel], false).Name;
        }

        public int getProgramIndex(int channel)
        {
            return currentPrograms[channel];
        }

        #endregion


        float time;

        #region EJUsing Loading Midi Method : midiAllNoteEvents 채워주는 부분 추가
        public bool LoadMidi(MidiFile midi, bool UnloadUnusedInstruments)
        {
            //time += base.Time.deltaTime;            
            if (playing == true) return false;

            _MidiFile = midi;

            //#region custom) midiEvent담아주는 부분
            //// j = track count
            //// i = events count



            //for (int j = 0; j < _MidiFile.Tracks.Length; j++)
            //{
            //    MidiEvent[] midiEvents_tracks = _MidiFile.Tracks[j].MidiEvents;

            //    List<MidiEventInfo> midiNoteEvents = new List<MidiEventInfo>();
            //    MidiEventInfo midiEventInfo_each = null;
            //    float playedTime = 0;

            //    for (int i = 0; i < midiEvents_tracks.Length; i++)
            //    {
            //        if (midiEvents_tracks[i].midiChannelEvent == MidiHelper.MidiChannelEvent.Note_On)
            //        {
            //            midiEventInfo_each = new MidiEventInfo();

            //            //on일 때의 deltaTime을 더해 줘야 쉼표까지 포함되어서 playedTime 누적
            //            playedTime += (midiEvents_tracks[i].deltaTime /480) * (60/120);    

            //            midiEventInfo_each.startTime = playedTime;
            //            midiEventInfo_each.pitch = midiEvents_tracks[i].parameter1;

            //            midiNoteEvents.Add(midiEventInfo_each);
            //        }

            //        //Note_Off check
            //        if (midiEvents_tracks[i].midiChannelEvent == MidiHelper.MidiChannelEvent.Note_Off)
            //        {
            //            //off될 때의 deltaTime이 곧 length. on과 off 사이의 길이
            //            midiEventInfo_each.length = (midiEvents_tracks[i].deltaTime / 480.0f) * (60.0f / 120.0f);

            //            // 그 길이만큼 또 누적
            //            playedTime += midiEventInfo_each.length; 

            //            //endTime 체크 필요!!!
            //            midiEventInfo_each.endTime = playedTime;
            //        }
                        
            //    }

            //    if (midiNoteEvents.Count > 0)
            //    {
            //        //midiAllNoteEvents[0] = midiNoteEvents;
            //        midiAllNoteEventsDic[getProgramIndex(j)] = midiNoteEvents;

            //        Debug.Log("000 LoadMidi 함수가 실행되었고, midiAllNoteEventsDic[getProgramIndex(j)]의 midiEvent의 개수는" + midiNoteEvents.Count + "가 담겼다");
            //    }
            //}

            //#endregion

            #region default - 연주 속도 설정 deltaTime >>> seconds?
            if (_MidiFile.SequencerReady == false)
            {
                try
                {
                    //Combine all tracks into 1 track that is organized from lowest to highest abs time
                    _MidiFile.CombineTracks();
                    //Convert delta time to sample time
                    eventIndex = 0;
                    uint lastSample = 0;
                    for (int x = 0; x < _MidiFile.Tracks[0].MidiEvents.Length; x++)
                    {
                        _MidiFile.Tracks[0].MidiEvents[x].deltaTime = lastSample + (uint)DeltaTimetoSamples(_MidiFile.Tracks[0].MidiEvents[x].deltaTime);
                        lastSample = _MidiFile.Tracks[0].MidiEvents[x].deltaTime;
                        //Update tempo
                        if (_MidiFile.Tracks[0].MidiEvents[x].midiMetaEvent == MidiHelper.MidiMetaEvent.Tempo)
                        {
                            _MidiFile.BeatsPerMinute = MidiHelper.MicroSecondsPerMinute / System.Convert.ToUInt32(_MidiFile.Tracks[0].MidiEvents[x].Parameters[0]);
                        }
                    }
                    //Set total time to proper value
                    _MidiFile.Tracks[0].TotalTime = _MidiFile.Tracks[0].MidiEvents[_MidiFile.Tracks[0].MidiEvents.Length-1].deltaTime;
                    //reset tempo
                    _MidiFile.BeatsPerMinute = 120;
                    //mark midi as ready for sequencing
                    _MidiFile.SequencerReady = true;
                }
                catch (Exception ex)
                {
                    //UnitySynth
                    Debug.Log("Error Loading Midi:\n" + ex.Message);
                    return false;
                }
            }
            #endregion
            blockList.Clear();
            #region default - 악기 설정
            if (UnloadUnusedInstruments == true)
            {
                if (synth.SoundBank == null)
                {//If there is no bank warn the developer =)
                    Debug.Log("No Soundbank loaded !");
                }
                else
                {
                    string bankStr = synth.SoundBank.BankPath;
                    //Remove old bank being used by synth
                    synth.UnloadBank();
                    //Add the bank and switch to it with the synth
                    BankManager.addBank(new InstrumentBank(synth.SampleRate, bankStr, _MidiFile.Tracks[0].Programs, _MidiFile.Tracks[0].DrumPrograms));
                    synth.SwitchBank(BankManager.Count - 1);
                }
            }
            return true;
            #endregion
        }

        public bool LoadMidi(string file, bool UnloadUnusedInstruments)
        {
            if (playing == true)  return false;

            MidiFile mf = null;

            try
            {
                mf = new MidiFile(file);
                
            }
            catch (Exception ex)
            {
                //UnitySynth
                Debug.Log("Error Loading Midi:\n" + ex.Message);
                return false;
            }
            return LoadMidi(mf, UnloadUnusedInstruments);
        }

        #endregion

        #region default Methods - 연주 관련
        public void Play()
        {
            if (playing == true)
                return;
            //Clear the current programs for the channels.
            Array.Clear(currentPrograms, 0, currentPrograms.Length);
            //Clear vol, pan, and tune
            ResetControllers();
            //set bpm
            _MidiFile.BeatsPerMinute = 120;
            //Let the synth know that the sequencer is ready.
            eventIndex = 0;
            playing = true;
        }
        public void Stop(bool immediate)
        {
            playing = false;
            sampleTime = 0;
            if (immediate)
                synth.NoteOffAll(true);
            else
                synth.NoteOffAll(false);
        }
        public bool isChannelMuted(int channel)
        {
            if (blockList.Contains((byte)channel))
                return true;
            return false;
        }
        public void MuteChannel(int channel)
        {
            if (channel > -1 && channel < 16)
                if (blockList.Contains((byte)channel) == false)
                    blockList.Add((byte)channel);
        }
        public void UnMuteChannel(int channel)
        {
            if (channel > -1 && channel < 16)
                blockList.Remove((byte)channel);
        }
        public void MuteAllChannels()
        {
            for (int x = 0; x < 16; x++)
                blockList.Add((byte)x);
        }
        public void UnMuteAllChannels()
        {
            blockList.Clear();
        }
        public void ResetControllers()
        {
            //Reset Pan Positions back to 0.0f
            Array.Clear(synth.PanPositions, 0, synth.PanPositions.Length);
            //Set Tuning Positions back to 0.0f
            Array.Clear(synth.TunePositions, 0, synth.TunePositions.Length);
            //Reset Vol Positions back to 1.00f
            for (int x = 0; x < synth.VolPositions.Length; x++)
                synth.VolPositions[x] = 1.00f;
        }
        public MidiSequencerEvent Process(int frame)
        {
            seqEvt.Events.Clear();
            //stop or loop
            if (sampleTime >= (int)_MidiFile.Tracks[0].TotalTime)
            {
                sampleTime = 0;
                if (looping == true)
                {
                    //Clear the current programs for the channels.
                    Array.Clear(currentPrograms, 0, currentPrograms.Length);
                    //Clear vol, pan, and tune
                    ResetControllers();
                    //set bpm
                    _MidiFile.BeatsPerMinute = 120;
                    //Let the synth know that the sequencer is ready.
                    eventIndex = 0;
                }
                else
                {
                    playing = false;
                    synth.NoteOffAll(true);
                    return null;
                }
            }
            while (eventIndex < _MidiFile.Tracks[0].EventCount && _MidiFile.Tracks[0].MidiEvents[eventIndex].deltaTime < (sampleTime + frame))
            {
                seqEvt.Events.Add(_MidiFile.Tracks[0].MidiEvents[eventIndex]);
                eventIndex++;
            }
            return seqEvt;
        }
        public void IncrementSampleCounter(int amount)
        {
            sampleTime = sampleTime + amount;
        }
        public void ProcessMidiEvent(MidiEvent midiEvent)
        {
            if (midiEvent.midiChannelEvent != MidiHelper.MidiChannelEvent.None)
            {
                switch (midiEvent.midiChannelEvent)
                {
                    case MidiHelper.MidiChannelEvent.Program_Change:
                        if (midiEvent.channel != 9)
                        {
                            if (midiEvent.parameter1 < synth.SoundBank.InstrumentCount)
                                currentPrograms[midiEvent.channel] = midiEvent.parameter1;
                        }
                        else //its the drum channel
                        {
                            if (midiEvent.parameter1 < synth.SoundBank.DrumCount)
                                currentPrograms[midiEvent.channel] = midiEvent.parameter1;
                        }
                        break;
                    case MidiHelper.MidiChannelEvent.Note_On:

                        if (blockList.Contains(midiEvent.channel))
                            return;
                        if (this.NoteOnEvent != null) {
                            Debug.Log("ON~!");
                            Debug.LogError(midiEvent.deltaTime);
                            this.NoteOnEvent(midiEvent.channel, midiEvent.parameter1, midiEvent.parameter2);
                        
                        }
                        synth.NoteOn(midiEvent.channel, midiEvent.parameter1, midiEvent.parameter2, currentPrograms[midiEvent.channel]);
                        break;
                    case MidiHelper.MidiChannelEvent.Note_Off:
                        if (this.NoteOffEvent != null)
                            this.NoteOffEvent(midiEvent.channel, midiEvent.parameter1);
                        synth.NoteOff(midiEvent.channel, midiEvent.parameter1);
                        break;
                    case MidiHelper.MidiChannelEvent.Pitch_Bend:
                        //Store PitchBend as the # of semitones higher or lower
                        synth.TunePositions[midiEvent.channel] = (double)midiEvent.Parameters[1] * PitchWheelSemitoneRange;
                        break;
                    case MidiHelper.MidiChannelEvent.Controller:
                        switch (midiEvent.GetControllerType())
                        {
                            case MidiHelper.ControllerType.AllNotesOff:
                                synth.NoteOffAll(true);
                                break;
                            case MidiHelper.ControllerType.MainVolume:
                                synth.VolPositions[midiEvent.channel] = midiEvent.parameter2 / 127.0f;
                                break;
                            case MidiHelper.ControllerType.Pan:
                                synth.PanPositions[midiEvent.channel] = (midiEvent.parameter2 - 64) == 63 ? 1.00f : (midiEvent.parameter2 - 64) / 64.0f;
                                break;
                            case MidiHelper.ControllerType.ResetControllers:
                                ResetControllers();
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (midiEvent.midiMetaEvent)
                {
                    case MidiHelper.MidiMetaEvent.Tempo:
                        _MidiFile.BeatsPerMinute = MidiHelper.MicroSecondsPerMinute / System.Convert.ToUInt32(midiEvent.Parameters[0]);
                        break;
                    default:
                        break;
                }
            }
        }
        public void Dispose()
        {
            Stop(true);
            //Set anything that may become a circular reference to null...
            synth = null;
            _MidiFile = null;
            seqEvt = null;
        }
        //--Private Methods
        private int DeltaTimetoSamples(uint DeltaTime)
        {
            return SynthHelper.getSampleFromTime(synth.SampleRate, (DeltaTime * (60.0f / (((int)_MidiFile.BeatsPerMinute) * _MidiFile.MidiHeader.DeltaTiming))));
        }
        private void SetTime(TimeSpan time)
        {
            int _stime = SynthHelper.getSampleFromTime(synth.SampleRate, (float)time.TotalSeconds);
            if (_stime > sampleTime)
            {
                SilentProcess(_stime - sampleTime);
            }
            else if (_stime < sampleTime)
            {//we have to restart the midi to make sure we get the right temp, instrument, etc
                synth.Stop();
                sampleTime = 0;
                Array.Clear(currentPrograms, 0, currentPrograms.Length);
                ResetControllers();
                _MidiFile.BeatsPerMinute = 120;
                eventIndex = 0;
                SilentProcess(_stime);
            }
        }
        private void SilentProcess(int amount)
        {
            while (eventIndex < _MidiFile.Tracks[0].EventCount && _MidiFile.Tracks[0].MidiEvents[eventIndex].deltaTime < (sampleTime + amount))
            {
                if (_MidiFile.Tracks[0].MidiEvents[eventIndex].midiChannelEvent != MidiHelper.MidiChannelEvent.Note_On)
                    ProcessMidiEvent(_MidiFile.Tracks[0].MidiEvents[eventIndex]);               
                eventIndex++;
            }
            sampleTime = sampleTime + amount;
        }
        #endregion
    }
}

