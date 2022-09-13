using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class MikeRotate : MonoBehaviour
{
    public enum RotationMode
    {
        Update,
        FixedUpdate,
        RigidBody
    }

    public RotationMode mode;
    public Vector3 rotationOverTime;

    private Rigidbody rb;
    private Rigidbody2D rb2d;

    private void Start()
    {
        if(mode != RotationMode.RigidBody) { return; }

        TryGetComponent(out rb);
        TryGetComponent(out rb2d);
    }

    void Update()
    {
        if(mode != RotationMode.Update) { return; }

        transform.Rotate(Time.deltaTime * rotationOverTime);
    }

    private void FixedUpdate()
    {
        if (mode == RotationMode.FixedUpdate)
        {
            transform.Rotate(transform.eulerAngles + Time.fixedDeltaTime * rotationOverTime);
        }
        else if(mode == RotationMode.RigidBody)
        {
            if(rb != null)
            {
                rb.MoveRotation(Quaternion.Euler(rotationOverTime * Time.fixedDeltaTime));
            }
            else if (rb2d != null)
            {
                rb2d.MoveRotation(Quaternion.Euler(rotationOverTime * Time.fixedDeltaTime));
            }
            else
            {
                Debug.LogWarning("There is no");
            }
        }
    }
}
