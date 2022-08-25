using EZCameraShake;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeCamera : MonoBehaviour
{
    public void Shake()
    {
        CameraShaker.Instance.ShakeOnce(2, 15, .25f, .35f);
    }
}
