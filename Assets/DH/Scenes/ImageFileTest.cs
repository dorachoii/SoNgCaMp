using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ImageFileTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnclickImageLoad() {
        NativeGallery.GetImageFromGallery((file) =>
        {
            FileInfo selected = new FileInfo(file);

            StartCoroutine(LoadImage(file));
        });
    }
    public RawImage image;
    IEnumerator LoadImage(string path) {
        yield return null;
        byte[] fileData = File.ReadAllBytes(path);
        Texture2D tex = new Texture2D(0,0);
        tex.LoadImage(fileData);
        image.texture = tex;
    }
}
