using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsSkippable : MonoBehaviour
{
    [SerializeField]  string _gameIdIOS = "Your-Apple-ID";
    [SerializeField]  string _mySurfacingIdIOS = "Interstitial_iOS";

    [SerializeField] string _gameIdAndroid = "Your-Google-ID";
    [SerializeField] string _mySurfacingIdAndroid = "Interstitial_Android";

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
    }

    public void ShowInterstitialAd()
    {
        if (!enabled) { return; }

        if (Advertisement.IsReady())
        {
            Advertisement.Show(SurfacingID);
        }
    }
}