using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class EJ_screenshot : MonoBehaviour
{
    public GameObject screenShot;
    public RawImage screenshotIMG;
    public RawImage defaultIMG;

    private string lastSavedFilePath;
    public void ClickScreenShot()
    {
        string timeStamp = System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
        string fileName = "SongCamp-ScreenShot-" + timeStamp + ".png";

        // 바탕화면에 저장
        string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory);
        string filePath = Path.Combine(desktopPath, fileName);

        lastSavedFilePath = filePath;
        ScreenCapture.CaptureScreenshot(filePath);
        Debug.Log("스크린샷이 저장된 경로: " + filePath);


        // 방금 저장한 이미지 표시
        // 코루틴을 사용하여 스크린샷 저장 후 처리
        StartCoroutine(WaitForScreenshot());

    }
    IEnumerator WaitForScreenshot()
    {
       
        yield return new WaitUntil(() => File.Exists(lastSavedFilePath));

        screenShot.SetActive(true);
        // 스크린샷을 Texture2D로 변환하여 RawImage에 표시
        byte[] fileData = File.ReadAllBytes(lastSavedFilePath);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(fileData);
        screenshotIMG.texture = texture;

        // 이미지 끄기
        StartCoroutine(ScreenshotOFF());
    }


    IEnumerator ScreenshotOFF()
    {
        yield return new WaitForSeconds(4f);
        screenShot.SetActive(false);
    }
}
