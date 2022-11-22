using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Events;

public class AdsRewarded : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsInitializationListener, IUnityAdsShowListener
{
    static AdsRewarded _instance;
    public static AdsRewarded Instance { get => _instance; }

    private void Awake()
    {
        _instance = this;
        Advertisement.Initialize(GameID, _testMode, transform, this);
        print($"Ads initialized with game id: {GameID} and is" + (_testMode ? " " : " NOT ") + "running in Test Mode ");
    }

    [SerializeField] string[] _preLoadAdsIOS = new string[0];
    [SerializeField] string[] _preLoadAdsAndroid = new string[0];

    [SerializeField] string _gameIdIOS = "Your-Apple-ID";
    [SerializeField] string _gameIdAndroid = "Your-Google-ID";

    [SerializeField] bool _testMode = true;

#if UNITY_IOS
    string GameID { get { return _gameIdIOS; } }
    string[] PreLoadAds { get { return _preLoadAdsIOS; } }
#endif
#if UNITY_ANDROID
    string GameID { get { return _gameIdAndroid; } }
    string[] PreLoadAds { get { return _preLoadAdsAndroid; } }
#endif

    Dictionary<string, UnityAction<UnityAdsShowCompletionState>> _callbacks = new(); 
    void InvokeCallback(string placementID, UnityAdsShowCompletionState result)
    {
        if(_callbacks.TryGetValue(placementID, out UnityAction<UnityAdsShowCompletionState> callback))
        {
            callback.Invoke(result); 
            _callbacks.Remove(placementID); 
        }
    }

    public void ShowRewardedVideo(string placementID, UnityAction<UnityAdsShowCompletionState> onFinished)
    {
        if (Advertisement.IsReady(placementID)) 
        { 
            Advertisement.Show(placementID); 
            _callbacks.Add(placementID, onFinished);
        }
        else 
        { 
            Debug.Log("Rewarded video is not ready!");
            onFinished.Invoke(UnityAdsShowCompletionState.UNKNOWN);
            Advertisement.Load(placementID);
        }
    }

    public void LoadAd(string placementId)
    {
        Advertisement.Load(placementId);
    }


    // ---------------------------


    public void OnUnityAdsDidStart(string placementId)
    {
        print($"Ad with placementId: {placementId} Did started");
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        print($"Ad with placementId: {placementId} Was loaded");
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        print($"AD palacementId: {placementId} error: {error} - {message}");
        Advertisement.Load(placementId);
    }
    public void OnInitializationComplete()
    {
        print($"IUnityAdsInitializationListener Complete!");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        print($"AD Initialization failure: {error} - {message}");
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        print($"Failed to show ad with placementId: {placementId} error: {error} - {message}");
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        print($"OnUnityAdsShowStart placementId: {placementId}");
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        print($"OnUnityAdsShowClick placementId: {placementId}");
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        print($"OnUnityAdsShowComplete placementId: {placementId} UnityAdsShowCompletionState: {showCompletionState}");
        InvokeCallback(placementId, showCompletionState);
        Advertisement.Load(placementId);
    }
}