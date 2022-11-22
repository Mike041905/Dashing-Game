using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Events;

public class AdsRewarded : MonoBehaviour, IUnityAdsListener
{
    static AdsRewarded _instance;
    public static AdsRewarded Instance { get => _instance; }

    private void Awake()
    {
        _instance = this;
    }


    [SerializeField] string _gameIdIOS = "Your-Apple-ID";

    [SerializeField] string _gameIdAndroid = "Your-Google-ID";

    [SerializeField] bool _testMode = true;

#if UNITY_IOS
    string GameID { get { return _gameIdIOS; } }
#endif
#if UNITY_ANDROID
    string GameID { get { return _gameIdAndroid; } }
#endif

    Dictionary<string, UnityAction<ShowResult>> _callbacks = new(); 

    void InvokeCallback(string placementID, ShowResult showResult)
    {
        if(_callbacks.TryGetValue(placementID, out UnityAction<ShowResult> callback))
        {
            callback.Invoke(showResult); 
            _callbacks.Remove(placementID); 
        }
    }

    void Start()
    {
        Advertisement.AddListener(this);
        Advertisement.Initialize(GameID, _testMode);
    }

    public void ShowRewardedVideo(string placementID, UnityAction<ShowResult> onFinished)
    {
        if (Advertisement.IsReady(placementID)) 
        { 
            Advertisement.Show(placementID); 
            _callbacks.Add(placementID, onFinished);
        }
        else 
        { 
            Debug.Log("Rewarded video is not ready!");
            onFinished.Invoke(ShowResult.Failed);
        }
    }

    public void OnUnityAdsDidFinish(string surfacingId, ShowResult showResult) 
    {
        print($"Ad result: {showResult}");
        InvokeCallback(surfacingId, showResult);
        LoadAd(surfacingId);
    }

    public void OnUnityAdsReady(string surfacingId)
    {
        print($"Ad with surfacingId: {surfacingId} is ready");
    }

    public void OnUnityAdsDidError(string message)
    {
        Debug.Log($"Ad Failed! - {message}");
    }

    public void OnDestroy() 
    {
        Advertisement.RemoveListener(this);
        print($"{gameObject} Ad listener has been destroyed");
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        print($"Ad with placementId: {placementId} Did started");
    }

    public void LoadAd(string adUnitId)
    {
        Advertisement.Load(adUnitId);
        Debug.Log("Loading Ad: " + adUnitId);
    }
}