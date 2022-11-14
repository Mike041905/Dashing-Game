using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BloomSettings : MonoBehaviour
{
    [SerializeField] Volume _volume;

    private void Start()
    {
        if(_volume.profile.TryGet(out Bloom bloom))
        {
            bloom.intensity.Override(StorageManager.Settings.Graphics.BloomIntensity);
            bloom.scatter.Override(StorageManager.Settings.Graphics.BloomScatter);
            bloom.highQualityFiltering.Override(StorageManager.Settings.Graphics.BloomHighQualityFilter);
            bloom.skipIterations.Override(StorageManager.Settings.Graphics.BloomSkipIterations);
        }
    }
}
