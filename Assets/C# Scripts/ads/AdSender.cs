using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Events;

public class AdSender : MonoBehaviour
{
    [SerializeField] protected string _adIdAndroid = "Rewarded_Android";
    [SerializeField] protected string _adIdIOS = "Rewarded_iOS";

#if UNITY_ANDROID
    string AdID { get => _adIdAndroid; }
#endif
#if UNITY_IOS
    string AdID { get => _adIdIOS; }
#endif

    public UnityEvent OnAdFinished;
    public UnityEvent OnAdSkipped;
    public UnityEvent OnAdFailed;

    private void Start()
    {
        AdsRewarded.Instance.LoadAd(AdID);
    }

    public virtual void ShowAdd()
    {
        AdsRewarded.Instance.ShowRewardedVideo(AdID, AdCallback);
    }

    void AdCallback(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Failed:
                OnAdFailed?.Invoke();
                break;

            case ShowResult.Skipped:
                OnAdSkipped?.Invoke();
                break;

            case ShowResult.Finished:
                OnAdFinished?.Invoke();
                break;
        }
    }
}
