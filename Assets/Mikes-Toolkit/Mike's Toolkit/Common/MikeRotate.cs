using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class MikeRotate : MonoBehaviour
{
    [SerializeField] Vector3 rotation;

    void Update()
    {
        transform.Rotate(rotation * Time.deltaTime);
    }
}
