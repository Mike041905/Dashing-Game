using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsRewarded : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsInitializationListener, IUnityAdsShowListener
{
    static AdsRewarded _instance;
    public static AdsRewarded Instance { get => _instance; }

    private void Awake()
    {
        _instance = this;
        Advertisement.Initialize(GameID, _testMode, this);
        print($"Ads initialized with game id: {GameID} and is" + (_testMode ? " " : " NOT ") + "running in Test Mode ");
    }

    [SerializeField] string[] _preLoadAdsIOS = new string[0];
    [SerializeField] string[] _preLoadAdsAndroid = new string[0];

    [SerializeField] string _gameIdIOS = "Your-Apple-ID";
    [SerializeField] string _gameIdAndroid = "Your-Google-ID";

    [SerializeField] bool _testMode = true;

    Dictionary<string, Action<ShowResult>> _actions = new();

    #region Platform Sepecific Properties
#if UNITY_IOS
    string GameID { get { return _gameIdIOS; } }
    string[] PreLoadAds { get { return _preLoadAdsIOS; } }
#endif
#if UNITY_ANDROID
    string GameID { get { return _gameIdAndroid; } }
    string[] PreLoadAds { get { return _preLoadAdsAndroid; } }
#endif
    #endregion


    //------------------------------


    public void ShowRewardedVideo(string placementID, Action<ShowResult> onFinished = null)
    {
        if (Advertisement.isInitialized) 
        { 
            Advertisement.Show(placementID, this);
            _actions.Add(placementID, onFinished);
        }
        else 
        { 
            Debug.Log("Rewarded video is not ready!");
            onFinished.Invoke(ShowResult.Failed);
            LoadAd(placementID);
        }
    }

    public void LoadAd(string placementId)
    {
        Advertisement.Load(placementId, this);
    }


    // ---------------------------


    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        print($"AD Initialization failure: {error} - {message}");
    }

    public void OnInitializationComplete()
    {
        print($"IUnityAdsInitializationListener Complete!");

        foreach (string adId in PreLoadAds)
        {
            LoadAd(adId);
        }
    }


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


    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        print($"Failed to show ad with placementId: {placementId} error: {error} - {message}");
        if(_actions.TryGetValue(placementId, out var action))
        {
            action?.Invoke(ShowResult.Failed);
        }
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
        if(_actions.TryGetValue(placementId, out var action))
        {
            action?.Invoke((ShowResult)showCompletionState);
            _actions.Remove(placementId);
        }
        LoadAd(placementId);
    }
}