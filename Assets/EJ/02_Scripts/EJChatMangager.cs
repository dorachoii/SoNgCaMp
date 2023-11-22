//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.ComponentModel.Design;
//using UnityEngine;
//using UnityEngine.UI;

//public class EJChatMangager : MonoBehaviour
//{
//    public GameObject YellowArea, WhiteArea, DateArea;
//    public RectTransform ContentRect;
//    public Scrollbar scrollbar;
//    EJChatAreaScript lastArea;

//    public void Chat(bool isSend, string text, string user, Texture2D picture)
//    {
//        //isSend: ���� ���´���
//        //isBottom: �� ������ üũ

//        if (text.Trim() == "") return;      //��� ����� ���͸� �����ִ� ��

//        bool isBottom = scrollbar.value <= -0.00001f;

//        //print(text);

//        EJChatAreaScript Area = Instantiate(isSend? YellowArea : WhiteArea).GetComponent<EJChatAreaScript>();
//        Area.transform.SetParent(ContentRect.transform, false);
//        Area.BoxRect.sizeDelta = new Vector2(600, Area.BoxRect.sizeDelta.y);
//        Area.TextRect.GetComponent<Text>().text = text;

//        if (picture != null) Area.UserImage.sprite = Sprite.Create(picture, new Rect(0, 0, picture.width, picture.height), new Vector2(0.5f, 0.5f));

//        Fit(Area.BoxRect);

//        //�� �� �̻��̸� ũ�⸦ �ٿ����鼭 �� ���� �Ʒ��� �������� �ٷ� �� ũ�⸦ �����Ѵ�.
//        float X = Area.TextRect.sizeDelta.x + 42;
//        float Y = Area.TextRect.sizeDelta.y;

//        if (Y > 49)
//        {
//            for (int i = 0; i < 200; i++)
//            {
//                Area.BoxRect.sizeDelta = new Vector2(X - i * 2, Area.BoxRect.sizeDelta.y);
//                Fit(Area.BoxRect);

//                if (Y != Area.TextRect.sizeDelta.y) { Area.BoxRect.sizeDelta = new Vector2(X - (i * 2) + 2, Y); break; }
//            }
//        }
//        else Area.BoxRect.sizeDelta = new Vector2(X, Y);

//        //���� �Ϳ� �б��� ������ ��¥�� �����̸� ����
//        DateTime t = DateTime.Now;
//        Area.Time = t.ToString("yyyy-MM-dd-HH-mm");
//        Area.User = user;

//        //���� ���� �׻� ���ο� �ð� ����
//        int hour = t.Hour;
//        if (t.Hour == 0) hour = 12;
//        else if (t.Hour > 12) hour -= 12;
//        Area.TimeText.text = (t.Hour > 12 ? "����" : "����") + hour + ":" + t.Minute.ToString("D2");


//        //���� �Ͱ� ������ ���� �ð�
//        //���ο� �ð��̸� �̹����� ���
//        bool isSame = lastArea != null && lastArea.Time == Area.Time && lastArea.User == Area.User;
//        if (isSame) lastArea.TimeText.text = "";

//        if (!isSend)
//        {
//            Area.UserImage.gameObject.SetActive(!isSame);
//            Area.UserText.gameObject.SetActive(!isSame);
//            Area.UserText.text = Area.User;
//        }

//        Fit(Area.BoxRect);
//        Fit(Area.AreaRect);
//        Fit(ContentRect);
//        lastArea = Area;

//        if (!isSend && !isBottom) return;
//        Invoke("ScrollDelay", 0.03f);

//        //���� �Ͱ� ��¥�� �ٸ��� ��¥ ���� ���̱�
//        if (lastArea != null && lastArea.Time.Substring(0, 10) != Area.Time.Substring(0, 10))
//        {
//            Transform CurDateArea = Instantiate(DateArea).transform;
//            CurDateArea.SetParent(ContentRect.transform, false);
//            CurDateArea.SetSiblingIndex(CurDateArea.GetSiblingIndex() - 1);

//            string week = "";

//            switch (t.DayOfWeek)
//            {
//                case DayOfWeek.Sunday: week = "��"; break;
//                case DayOfWeek.Monday: week = "��"; break;
//                case DayOfWeek.Tuesday: week = "��"; break;
//                case DayOfWeek.Wednesday: week = "��"; break;
//                case DayOfWeek.Thursday: week = "��"; break;
//                case DayOfWeek.Friday: week = "��"; break;
//                case DayOfWeek.Saturday: week = "��"; break;
//            }

//            CurDateArea.GetComponent<EJChatAreaScript>().DateText.text = t.Year + "��" + t.Month + "��" + t.Day + "��" + week + "����";
//        }

//    }

//    void Fit(RectTransform Rect) => LayoutRebuilder.ForceRebuildLayoutImmediate(Rect);
//}
