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


    bool? _supportAds = null;
    bool SupportAds { get => _supportAds ??= StorageManager.Settings.Misc.SupportAds; }

     bool Banner { get => _banner && SupportAds; set { _banner = value; UpdateAdStatus(); } }
     bool Interstitial { get => _interstitial && SupportAds; set { _interstitial = value; UpdateAdStatus(); } }

    private void Awake()
    {
        UpdateAdStatus();
    }

    void UpdateAdStatus()
    {
        _ad.enabled = Banner || Interstitial;
    }
}
