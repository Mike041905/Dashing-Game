using UnityEngine;
using UnityEngine.Events;

public class Option : MonoBehaviour
{
    bool _initialized = false;
    public void Initialize(string optionKey, UnityAction apply)
    {
        Initialize(optionKey, false, apply);
    }
    public void Initialize(string optionKey, bool requireApply, UnityAction apply)
    {
        if (_initialized) { return; }
        _initialized = true;

        apply += Apply;
        OptionKey = optionKey;
        RequireApply = requireApply;
    }
    
    public string OptionKey { get; private set; }
    public bool RequireApply { get; private set; }

    string _value = "";


    public void SetValue(float value)
    {
        if (RequireApply) { this._value = value.ToString(); return; }

        StorageManager.Settings.SaveOption(OptionKey, value);
    }

    public void SetValue(int value)
    {
        if (RequireApply) { this._value = value.ToString(); return; }

        StorageManager.Settings.SaveOption(OptionKey, value);
    }

    public void SetValue(bool value)
    {
        if (RequireApply) { this._value = value.ToString(); return; }

        StorageManager.Settings.SaveOption(OptionKey, value);
    }

    public void SetValue(string value)
    {
        if (RequireApply) { this._value = value.ToString(); return; }

        StorageManager.Settings.SaveOption(OptionKey, value);
    }


    public void Apply()
    {
        if (!RequireApply) { return; }
        ForceApply();
    }

    public void ForceApply()
    {
        StorageManager.Settings.SaveOption(OptionKey, _value);
    }
}
