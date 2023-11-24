using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitButton : MonoBehaviour
{

    public GameObject Target;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Onclick() {
        Target?.SetActive(false);
    }

    public void Offclick() {

        Target?.SetActive(Target.activeSelf ? false : true);
    }
}
