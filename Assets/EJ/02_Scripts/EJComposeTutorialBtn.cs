using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EJComposeTutorialBtn : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject tutorialCanvas;

    public void clickTutorial()
    {
        tutorialCanvas.SetActive(true);
        gameObject.SetActive(false);
    }

    public void clickX()
    {
        tutorialCanvas.SetActive(false);
    }
}
