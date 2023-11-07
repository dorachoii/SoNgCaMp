using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickerTest : MonoBehaviour
{
    Material mat;

    RectTransform rt;

    Color color1;
    Color color2;

    public MeshRenderer mr;

    void Start()
    {
        mat = GetComponent<Image>().material;
        rt = GetComponent<RectTransform>();


        color1 = Color.white;
        color2 = Color.red;
        mat.SetColor("_Color1", color1);
        mat.SetColor("_Color2", color2);
    }

    // Update is called once per frame
    void Update()
    {
        
        if(Input.GetMouseButton(0))
        {
            Vector2 localPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, Input.mousePosition, null, out localPos);
            if(rt.rect.x < localPos.x && localPos.x < rt.rect.xMax &&
               rt.rect.y < localPos.y && localPos.y < rt.rect.yMax)
            {
                float x = (rt.rect.width * 0.5f - localPos.x) / rt.rect.width;
                
                print(localPos + ", " + x );

                Color picColor = Color.Lerp(color2, color1, x);
                mr.material.color = picColor;
                print(picColor);
            }

        }        
    }
}
