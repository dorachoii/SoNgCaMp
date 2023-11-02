using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;

//Note Storage

//01. NoteType
//02. NoteInfo

//All Notes
public enum NoteType
{
    SHORT,
    LONG,
    DRAG_RIGHT,
    DRAG_LEFT
}

[System.Serializable]
public struct NoteInfo
{
    public int railIdx;
    public int type;
    public float time;

    //longNote�϶� start�̸� true, end�̸� false
    public bool isLongNoteStart;

    //dragNote�� ������ �ϴ� index
    public int DRAG_release_idx;

    //longNote�� dragNote�� �����ٰ� ���Ƽ� enableüũ�� �ؾ��� ��
    public bool isNoteEnabled;
}

public class EJNoteStorage : MonoBehaviour
{

}

