using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsBanner : MonoBehaviour
{
    [System.Serializable]
    enum SerializedBannerPosition
    {
        TOP_LEFT,
        TOP_CENTER,
        TOP_RIGHT,
        BOTTOM_LEFT,
        BOTTOM_CENTER,
        BOTTOM_RIGHT,
        CENTER
    }

    [SerializeField] SerializedBannerPosition _bannerPosition;

    [SerializeField] string _gameIdIOS = "Your-Apple-ID";
    [SerializeField] string _mySurfacingIdIOS = "Banner_iOS";

    [SerializeField] string _gameIdAndroid = "Your-Google-ID";
    [SerializeField] string _mySurfacingIdAndroid = "Banner_Android";

    [SerializeField] bool _testMode = true;

#if UNITY_IOS
    string GameID { get { return _gameIdIOS; } }
    string SurfacingID { get { return _mySurfacingIdIOS;} }
#endif
#if UNITY_ANDROID
    string GameID { get { return _gameIdAndroid; } }
    string SurfacingID { get { return _mySurfacingIdAndroid; } }
#endif

    void Start()
    {
        Advertisement.Initialize(GameID, _testMode);
        StartCoroutine(ShowBannerWhenInitialized());
        Advertisement.Banner.SetPosition((BannerPosition)_bannerPosition);

    }

    IEnumerator ShowBannerWhenInitialized()
    {
        while (!Advertisement.isInitialized)
        {
            yield return new WaitForSeconds(0.5f);
        }
        Advertisement.Banner.Show(SurfacingID);
    }
}