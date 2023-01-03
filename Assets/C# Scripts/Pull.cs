using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pull : MonoBehaviour
{
    float _size;
    [SerializeField] float force;
    [SerializeField] float _maxShakeMagnitude = 1;
    [SerializeField] float _shakeMagnitudeExponent = 1;
    [SerializeField] float _maxShakeRoughness = 1;
    [SerializeField] float _shakeRoughnessExponent = 1;
    [SerializeField, Tooltip("In miliseconds (is rounded later)")] float _maxVibration = 600;

    private void Start()
    {
        _size = GetComponent<CircleCollider2D>().radius;
    }

    private void OnTriggerStay2D(Collider2D c)
    {
        if (!isActiveAndEnabled) { return; }

        if(!c.CompareTag("Player")) { return; }
        if(Player.Instance.PlayerDash.CurrentDash != null) { return; }

        float distancePerc = (1 - Vector2.Distance(transform.position, c.ClosestPoint(transform.position)) / _size);
        
        EZCameraShake.CameraShaker.Instance.ShakeOnce(Mathf.Pow(distancePerc, _shakeMagnitudeExponent) * _maxShakeMagnitude, Mathf.Pow(distancePerc, _shakeRoughnessExponent) * _maxShakeRoughness, 0, Time.fixedDeltaTime);
        c.transform.position = Vector2.MoveTowards(c.transform.position, transform.position, distancePerc * force * Time.deltaTime);
        HapticFeedback.Vibrate(Mathf.RoundToInt(_maxVibration * Time.fixedDeltaTime));
    }
}
