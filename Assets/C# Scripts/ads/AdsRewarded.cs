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

    Dictionary<string, UnityAction> _callbacks = new(); 

    void InvokeCallback(string placementID)
    {
        if(_callbacks.TryGetValue(placementID, out UnityAction callback))
        {
            callback.Invoke(); 
            _callbacks.Remove(placementID); 
        }
    }

    void Start()
    {
        Advertisement.AddListener(this);
        Advertisement.Initialize(GameID, _testMode);
    }

    public void ShowRewardedVideo(string placementID, UnityAction onFinished)
    {
        if (Advertisement.IsReady(placementID)) 
        { 
            Advertisement.Show(placementID); 
            _callbacks.Add(placementID, onFinished);
        }
        else { Debug.Log("Rewarded video is not ready!"); }
    }

    public void OnUnityAdsDidFinish(string surfacingId, ShowResult showResult) 
    {
        if (showResult == ShowResult.Finished)
        {
            InvokeCallback(surfacingId);
        }
        else if (showResult == ShowResult.Skipped)
        {
            _callbacks.Remove(surfacingId);
            print("Ad Skipped");
        }
        else if (showResult == ShowResult.Failed)
        {
            _callbacks.Remove(surfacingId);
            print("Ad Failed");
        }
    }

    public void OnUnityAdsReady(string surfacingId)
    {

    }

    public void OnUnityAdsDidError(string message)
    {
        Debug.Log(message);
    }

    public void OnDestroy() 
    {
        Advertisement.RemoveListener(this);
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        
    }
}