using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Events;

public class AdSender : MonoBehaviour
{
    [SerializeField] string _adIdAndroid = "Rewarded_Android";
    [SerializeField] string _adIdIOS = "Rewarded_iOS";

#if UNITY_ANDROID
    string AdID { get => _adIdAndroid; }
#endif
#if UNITY_IOS
    string AdID { get => _adIdIOS; }
#endif

    [SerializeField] UnityEvent _onAdFinished;
    [SerializeField] UnityEvent _onAdSkipped;
    [SerializeField] UnityEvent _onAdFailed;

    private void Start()
    {
        AdsRewarded.Instance.LoadAd(AdID);
    }

    public void ShowAdd()
    {
        AdsRewarded.Instance.ShowRewardedVideo(AdID, AdCallback);
    }

    void AdCallback(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Failed:
                _onAdFailed?.Invoke();
                break;

            case ShowResult.Skipped:
                _onAdSkipped?.Invoke();
                break;

            case ShowResult.Finished:
                _onAdFinished?.Invoke();
                break;
        }
    }
}
