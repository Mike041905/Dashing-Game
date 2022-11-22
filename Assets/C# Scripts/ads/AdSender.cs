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

    void AdCallback(UnityAdsShowCompletionState result)
    {
        switch (result)
        {
            case UnityAdsShowCompletionState.UNKNOWN:
                _onAdFailed?.Invoke();
                break;

            case UnityAdsShowCompletionState.SKIPPED:
                _onAdSkipped?.Invoke();
                break;

            case UnityAdsShowCompletionState.COMPLETED:
                _onAdFinished?.Invoke();
                break;
        }
    }
}
