using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EJ_screenshot : MonoBehaviour
{
    public void ClickScreenShot()
    {
        string timeStamp = System.DateTime.Now.ToString("yyyy-MM-dd");
        string fileName = "SongCamp-ScreenShot-" + timeStamp + ".png";


        //ScreenCapture.CaptureScreenshot("SongCamp.PNG");
        ScreenCapture.CaptureScreenshot("~/Downloads/" + fileName);

        print("스크린캡처가 실행되었습니다");
    }
}
