using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleAnimation : MonoBehaviour
{
    [SerializeField] string _key;
    [SerializeField] Animator _animator;

    public void SetAnimationState(bool state)
    {
        _animator.SetBool(_key, state);
    }
}
