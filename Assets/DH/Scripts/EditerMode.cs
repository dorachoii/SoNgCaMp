using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditerMode : MonoBehaviour
{
    public static EditerMode instance;

    private void Awake()
    {
        instance = this;
    }
    //�����͸�� : Pitch������� Volume ���� ��� Beat ���� ��� 
    public enum EditerState { 
        Pitch,Volume,Beat
    }
    public EditerState currentState;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeStatePitch() {
        currentState = EditerState.Pitch; 
        Debug.Log("State �����");
    }
    public void ChangeStateVolume()
    {
        currentState = EditerState.Volume;
        Debug.Log("State �����");
    }
    public void ChangeStateBeat()
    {
        currentState = EditerState.Beat;
        Debug.Log("State �����");
    }
    public void te14st() { }


}
