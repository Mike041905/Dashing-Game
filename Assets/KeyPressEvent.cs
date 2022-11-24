using UnityEngine;
using UnityEngine.Events;

public class KeyPressEvent : MonoBehaviour
{
    [SerializeField] string _keyName;

    [SerializeField] UnityEvent _onKeyDown;
    [SerializeField] UnityEvent _onKeyContinous;
    [SerializeField] UnityEvent _onKeyUp;

    private void Update()
    {
        if (Input.GetKeyDown(_keyName)) { _onKeyDown?.Invoke(); }
        if (Input.GetKey(_keyName)) { _onKeyContinous?.Invoke(); }
        if (Input.GetKeyUp(_keyName)) { _onKeyUp?.Invoke(); }
    }
}
