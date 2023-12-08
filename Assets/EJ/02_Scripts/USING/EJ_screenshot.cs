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

        // ����ȭ�鿡 ����
        string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory);
        string filePath = Path.Combine(desktopPath, fileName);

        lastSavedFilePath = filePath;
        ScreenCapture.CaptureScreenshot(filePath);
        Debug.Log("��ũ������ ����� ���: " + filePath);


        // ��� ������ �̹��� ǥ��
        // �ڷ�ƾ�� ����Ͽ� ��ũ���� ���� �� ó��
        StartCoroutine(WaitForScreenshot());

    }
    IEnumerator WaitForScreenshot()
    {
       
        yield return new WaitUntil(() => File.Exists(lastSavedFilePath));

        screenShot.SetActive(true);
        // ��ũ������ Texture2D�� ��ȯ�Ͽ� RawImage�� ǥ��
        byte[] fileData = File.ReadAllBytes(lastSavedFilePath);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(fileData);
        screenshotIMG.texture = texture;

        // �̹��� ����
        StartCoroutine(ScreenshotOFF());
    }


    IEnumerator ScreenshotOFF()
    {
        yield return new WaitForSeconds(4f);
        screenShot.SetActive(false);
    }
}
