using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    [SerializeField] UnityEvent _onClickEvent;

    public void ShowAdd()
    {
        AdsRewarded.Instance.ShowRewardedVideo(AdID, () => { _onClickEvent?.Invoke(); });
    }
}
