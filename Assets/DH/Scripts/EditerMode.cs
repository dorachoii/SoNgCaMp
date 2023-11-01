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
    //에디터모드 : Pitch수정모드 Volume 수정 모드 Beat 수정 모드 
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
        Debug.Log("State 변경됨");
    }
    public void ChangeStateVolume()
    {
        currentState = EditerState.Volume;
        Debug.Log("State 변경됨");
    }
    public void ChangeStateBeat()
    {
        currentState = EditerState.Beat;
        Debug.Log("State 변경됨");
    }
    public void te14st() { }


}
