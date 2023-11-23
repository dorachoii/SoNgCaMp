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
//        //isSend: 내가 보냈는지
//        //isBottom: 맨 끝인지 체크

//        if (text.Trim() == "") return;      //띄어 쓰기와 엔터를 없애주는 것

//        bool isBottom = scrollbar.value <= -0.00001f;

//        //print(text);

//        EJChatAreaScript Area = Instantiate(isSend? YellowArea : WhiteArea).GetComponent<EJChatAreaScript>();
//        Area.transform.SetParent(ContentRect.transform, false);
//        Area.BoxRect.sizeDelta = new Vector2(600, Area.BoxRect.sizeDelta.y);
//        Area.TextRect.GetComponent<Text>().text = text;

//        if (picture != null) Area.UserImage.sprite = Sprite.Create(picture, new Rect(0, 0, picture.width, picture.height), new Vector2(0.5f, 0.5f));

//        Fit(Area.BoxRect);

//        //두 줄 이상이면 크기를 줄여가면서 한 줄이 아래로 내려가면 바로 전 크기를 대입한다.
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

//        //현재 것에 분까지 나오는 날짜와 유저이름 대입
//        DateTime t = DateTime.Now;
//        Area.Time = t.ToString("yyyy-MM-dd-HH-mm");
//        Area.User = user;

//        //현재 것은 항상 새로운 시간 대입
//        int hour = t.Hour;
//        if (t.Hour == 0) hour = 12;
//        else if (t.Hour > 12) hour -= 12;
//        Area.TimeText.text = (t.Hour > 12 ? "오후" : "오전") + hour + ":" + t.Minute.ToString("D2");


//        //이전 것과 같으면 이전 시간
//        //새로운 시간이면 이미지가 뜬다
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

//        //이전 것과 날짜가 다르면 날짜 영역 보이기
//        if (lastArea != null && lastArea.Time.Substring(0, 10) != Area.Time.Substring(0, 10))
//        {
//            Transform CurDateArea = Instantiate(DateArea).transform;
//            CurDateArea.SetParent(ContentRect.transform, false);
//            CurDateArea.SetSiblingIndex(CurDateArea.GetSiblingIndex() - 1);

//            string week = "";

//            switch (t.DayOfWeek)
//            {
//                case DayOfWeek.Sunday: week = "일"; break;
//                case DayOfWeek.Monday: week = "일"; break;
//                case DayOfWeek.Tuesday: week = "일"; break;
//                case DayOfWeek.Wednesday: week = "일"; break;
//                case DayOfWeek.Thursday: week = "일"; break;
//                case DayOfWeek.Friday: week = "일"; break;
//                case DayOfWeek.Saturday: week = "일"; break;
//            }

//            CurDateArea.GetComponent<EJChatAreaScript>().DateText.text = t.Year + "년" + t.Month + "월" + t.Day + "일" + week + "요일";
//        }

//    }

//    void Fit(RectTransform Rect) => LayoutRebuilder.ForceRebuildLayoutImmediate(Rect);
//}
