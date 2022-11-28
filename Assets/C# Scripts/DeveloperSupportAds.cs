using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class DeveloperSupportAds : MonoBehaviour
{
    [SerializeField] MonoBehaviour _ad;

    [Header("Support Type")]
    [SerializeField] bool _banner = false;
    [SerializeField] bool _interstitial = false;

     bool Banner { get => _banner && StorageManager.Settings.Misc.SupportAds; set { _banner = value; UpdateAdStatus(); } }
     bool Interstitial { get => _interstitial && StorageManager.Settings.Misc.SupportAds; set { _interstitial = value; UpdateAdStatus(); } }

    private void Awake()
    {
        UpdateAdStatus();
    }

    void UpdateAdStatus()
    {
        _ad.enabled = Banner || Interstitial;
    }
}
