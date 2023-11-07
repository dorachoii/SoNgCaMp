//using Gpm.WebView;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class WebViewTest : MonoBehaviour
//{
//    // Start is called before the first frame update
//    void Start()
//    {
        
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        if (Input.GetMouseButtonDown(0))
//        {
//            var config = new GpmWebViewRequest.Configuration()
//            {
//                style = GpmWebViewStyle.FULLSCREEN,
//                orientation = GpmOrientation.UNSPECIFIED,
//                isClearCookie = true,
//                isClearCache = true,
//                backgroundColor = "#FFFFFF",
//                isNavigationBarVisible = true,
//                navigationBarColor = "#4B96E6",
//                title = "The page title.",
//                isBackButtonVisible = true,
//                isForwardButtonVisible = true,
//                isCloseButtonVisible = true,
//                supportMultipleWindows = true,
//            };

//            GpmWebView.ShowUrl("https://github.com/", config, (callbackType, data, error) => {
//                print(callbackType);
//            }, null);
//        }
//    }
//}
