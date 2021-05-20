using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Android;
using TMPro;

// 매니페스트 선언필요.  <uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />
//https://answers.unity.com/questions/200173/android-how-to-refresh-the-gallery-.html
public class ScreenCaptureManager : MonoBehaviour
{
    bool onCapture = false;

    [SerializeField]
    private TextMeshProUGUI sceneNumberText;  // 씬넘버 올려줄 텍스트
    int numberOfShot = 0;

  /*
    float screenShotAreaHeight = (Screen.height * 0.4f) - Screen.height;   
    float screenShotAreaWidth = (Screen.width * 0.266f) - Screen.width;
  */
    
    float screenShotAreaHeight = (Screen.height * 0.30f);
    float screenShotAreaWidth = (Screen.width * 0.825f);

    int ny = (int)(Screen.width*0.175f);

    void Start()
    {
        //  sceneNumberText = GetComponentInChildren<TextMeshProUGUI>();

        //    float screenX = Screen.width;
        //   float screenY = Screen.height;
        /*  float _width  = screenShotPanel.sizeDelta.x;
          float _height = screenShotPanel.sizeDelta.y;
          width = (int)(screenX - ((_width / screenX) * screenX));
          height = (int)(screenY - ((_height / screenY) * screenY));

          Debug.Log($"width = {width}, height = {height}");
          */
    }

    public void PressBtnCapture()
    {
        if (onCapture == false)
        {
            StartCoroutine("CRSaveScreenshot");
            NumberOfShot();
        }

    }

    public void NumberOfShot()
    {
        numberOfShot = numberOfShot + 1;
        sceneNumberText.text = $"Scene Number: 1 - <#ffffff> {numberOfShot} </color>";
        
    }

    IEnumerator CRSaveScreenshot()
    {
        onCapture = true;

        yield return new WaitForEndOfFrame();

        if (Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite) == false)
        {
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);

            yield return new WaitForSeconds(0.2f);
            yield return new WaitUntil(() => Application.isFocused == true);

            if (Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite) == false)
            {
                //다이얼로그를 위해 별도의 플러그인을 사용했었다. 이 코드는 주석 처리함.
                //AGAlertDialog.ShowMessageDialog("권한 필요", "스크린샷을 저장하기 위해 저장소 권한이 필요합니다.",
                //"Ok", () => OpenAppSetting(),
                //"No!", () => AGUIMisc.ShowToast("저장소 요청 거절됨"));

                // 별도로 확인 팝업을 띄우지 않을꺼면 OpenAppSetting()을 바로 호출함.
                OpenAppSetting();

                onCapture = false;
                yield break;
            }
        }

        string fileLocation = "mnt/sdcard/DCIM/Camera/";   // 경로설정 
        string filename = Application.productName + "_" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".png";
        string finalLOC = fileLocation + filename;

        if (!Directory.Exists(fileLocation))
        {
            Directory.CreateDirectory(fileLocation);
        }

        byte[] imageByte; //스크린샷을 Byte로 저장.Texture2D use 
      //  Texture2D tex = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, true); // 스크린샷 설정 
      //  tex.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, true);

        Texture2D tex = new Texture2D((Screen.width-ny),Screen.height, TextureFormat.RGB24, true); //  생성할 텍스트의 크기와 포멧 등을 설정하여 생성
       // tex.ReadPixels(new Rect(0, 0, screenShotAreaWidth, screenShotAreaHeight), 0, 0, true); //스크린샷 찍을 화면의 영역 지정(현제는 전체화면)
        tex.ReadPixels(new Rect(0, screenShotAreaHeight, screenShotAreaWidth, Screen.height), 0, 0, true); //스크린샷 찍을 화면의 영역 지정(현제는 전체화면)
        tex.Apply();  // 적용

        imageByte = tex.EncodeToPNG();
        DestroyImmediate(tex);

        File.WriteAllBytes(finalLOC, imageByte);


        AndroidJavaClass classPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject objActivity = classPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaClass classUri = new AndroidJavaClass("android.net.Uri");
        AndroidJavaObject objIntent = new AndroidJavaObject("android.content.Intent", new object[2] { "android.intent.action.MEDIA_SCANNER_SCAN_FILE", classUri.CallStatic<AndroidJavaObject>("parse", "file://" + finalLOC) });
        objActivity.Call("sendBroadcast", objIntent);

        //아래 한 줄 또한 별도의 안드로이드 플러그인. 별도로 만들어서 호출하는 함수를 넣어주면 된다.
        //AGUIMisc.ShowToast(finalLOC + "로 저장했습니다.");
        onCapture = false;
    }


    // https://forum.unity.com/threads/redirect-to-app-settings.461140/
    private void OpenAppSetting()
    {
        try
        {
#if UNITY_ANDROID
            using (var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (AndroidJavaObject currentActivityObject = unityClass.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                string packageName = currentActivityObject.Call<string>("getPackageName");

                using (var uriClass = new AndroidJavaClass("android.net.Uri"))
                using (AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("fromParts", "package", packageName, null))
                using (var intentObject = new AndroidJavaObject("android.content.Intent", "android.settings.APPLICATION_DETAILS_SETTINGS", uriObject))
                {
                    intentObject.Call<AndroidJavaObject>("addCategory", "android.intent.category.DEFAULT");
                    intentObject.Call<AndroidJavaObject>("setFlags", 0x10000000);
                    currentActivityObject.Call("startActivity", intentObject);
                }
            }
#endif
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }
}


/*
GameObject screenShotArea;

RectTransform screenShotArea_rt;

float screenShotAreaHeight = (Screen.height*0.4f) - Screen.height;
float screenShotAreaWidth = Screen.width*0.266f) - Screen.width;

screenShotArea_rt.sizeDelta = new Vector2(screenShotAreaWidth,screenShotAreaWidth);
*/