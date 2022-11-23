using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Option : MonoBehaviour
{
    bool _initialized = false;
    public void Initialize(string optionName, string optionKey, UnityAction apply = null)
    {
        Initialize(optionName, optionKey, false, apply);
    }
    public void Initialize(string optionName, string optionKey, bool requireApply, UnityAction apply = null)
    {
        if (_initialized) { return; }
        _initialized = true;

        apply += Apply;
        OptionKey = optionKey;
        RequireApply = requireApply;
        _label.text = optionName;
    }

    [SerializeField] TextMeshProUGUI _label;

    public string OptionKey { get; private set; }
    public bool RequireApply { get; private set; }

    string _value = "";
    Type _valueType;

    enum Type
    {
        Float,
        Int,
        Bool,
        String
    }


    public void SetValue(float value)
    {
        if (RequireApply) { _valueType = Type.Float; this._value = value.ToString(); return; }

        StorageManager.Settings.SaveOption(OptionKey, value);
    }

    public void SetValue(int value)
    {
        if (RequireApply) { _valueType = Type.Int; this._value = value.ToString(); return; }

        StorageManager.Settings.SaveOption(OptionKey, value);
    }

    public void SetValue(bool value)
    {
        if (RequireApply) { _valueType = Type.Bool; this._value = value.ToString(); return; }

        StorageManager.Settings.SaveOption(OptionKey, value);
    }

    public void SetValue(string value)
    {
        if (RequireApply) { _valueType = Type.String; this._value = value.ToString(); return; }

        StorageManager.Settings.SaveOption(OptionKey, value);
    }


    public void Apply()
    {
        if (!RequireApply) { return; }
        ForceApply();
    }

    public void ForceApply()
    {
        switch (_valueType)
        {
            case Type.Float:
                StorageManager.Settings.SaveOption(OptionKey, float.Parse(_value));
                break;
            case Type.Int:
                StorageManager.Settings.SaveOption(OptionKey, int.Parse(_value));
                break;
            case Type.Bool:
                StorageManager.Settings.SaveOption(OptionKey, bool.Parse(_value));
                break;
            case Type.String:
                StorageManager.Settings.SaveOption(OptionKey, _value);
                break;
        }
    }
}
