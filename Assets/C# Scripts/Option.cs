using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Option : MonoBehaviour
{
    [Serializable]
    enum OptionType
    {
        Float,
        Int,
        Bool,
        String
    }

    bool _initialized = false;
    public void Initialize(string optionName, string optionKey, OptionsMenu menu, UnityAction apply = null)
    {
        Initialize(optionName, optionKey, false, menu, apply);
    }
    public void Initialize(string optionName, string optionKey, bool requireApply, OptionsMenu menu, UnityAction apply = null)
    {
        Initialize(optionName, optionKey, _defaultValue, requireApply, menu, apply);
    }

    public void Initialize(string optionName, string optionKey, string defaultValue, bool requireApply, OptionsMenu menu, UnityAction apply = null)
    {
        if (_initialized) { return; }
        _initialized = true;

        apply += Apply;
        OptionKey = optionKey;
        RequireApply = requireApply;
        _label.text = optionName;
        _optionsMenu = menu;
        _defaultValue = defaultValue;
        InitalizeValue();
    }

    [SerializeField] TextMeshProUGUI _label;

    [Header("Interactable")] // Couldn't find a shared class for components that have a value field so imma just do this manualy :(
    [SerializeField] Slider _slider;
    [SerializeField] Toggle _toggle;
    [SerializeField] TMP_InputField _inputField;
    [SerializeField] TMP_Dropdown _dropdown;

    public string OptionKey { get; private set; }
    public bool RequireApply { get; private set; }

    [SerializeField] OptionType _valueType;
    [SerializeField] string _defaultValue;

    string _value = "";
    OptionsMenu _optionsMenu;

    private void OnEnable()
    {
        // Workaround for animator
        if(!_initialized) { return; }

        if (_slider != null) { _slider.onValueChanged.Invoke(_slider.value); }
        if (_toggle != null) { _toggle.onValueChanged.Invoke(_toggle.isOn); }
        if (_inputField != null) { _inputField.onValueChanged.Invoke(_inputField.text); }
        if (_dropdown != null) { _dropdown.onValueChanged.Invoke(_dropdown.value); }
    }

    public void SetValue(float value)
    {
        if (ReqireApply(value)) { return; }

        StorageManager.Settings.SaveOption(OptionKey, value);
    }

    public void SetValue(int value)
    {
        if (ReqireApply(value)) { return; }

        StorageManager.Settings.SaveOption(OptionKey, value);
    }

    public void SetValue(bool value)
    {
        if (ReqireApply(value)) { return; }

        StorageManager.Settings.SaveOption(OptionKey, value);
    }

    public void SetValue(string value)
    {
        if (ReqireApply(value)) { return; }

        StorageManager.Settings.SaveOption(OptionKey, value);
    }


    public bool ReqireApply<T>(T value)
    {
        if (!RequireApply) { return false; } 
        _valueType = ParseType<T>();
        _value = value.ToString();
        _optionsMenu.RequireApply();

        return true;
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
            case OptionType.Float:
                StorageManager.Settings.SaveOption(OptionKey, float.Parse(_value));
                break;

            case OptionType.Int:
                StorageManager.Settings.SaveOption(OptionKey, int.Parse(_value));
                break;

            case OptionType.Bool:
                StorageManager.Settings.SaveOption(OptionKey, bool.Parse(_value));
                break;

            case OptionType.String:
                StorageManager.Settings.SaveOption(OptionKey, _value);
                break;

            default: throw new("Invalid type");
        }
        
    }

    void InitalizeValue()
    {
        // I have no idea what I'm doing
        OptionType type = ParseType(AutoParseString(_defaultValue, out object _val));

        switch (type)
        {
            case OptionType.Float:
                float f = StorageManager.Settings.GetOption(OptionKey, (_val as float?).Value);

                if (_slider != null) { _slider.value = f; }
                if (_inputField != null) { _inputField.text = f.ToString(); }
                break;

            case OptionType.Int:
                int i = StorageManager.Settings.GetOption(OptionKey, (_val as int?).Value);

                if (_slider != null) { _slider.value = i; }
                if (_inputField != null) { _inputField.text = i.ToString(); }
                if (_dropdown != null) { _dropdown.value = i; }
                break;

            case OptionType.Bool:
                bool b = StorageManager.Settings.GetOption(OptionKey, (_val as bool?).Value);

                if(_toggle != null) { _toggle.isOn = b; }
                break;

            case OptionType.String:
                string s = StorageManager.Settings.GetOption(OptionKey, _val as string);

                if (_inputField != null) { _inputField.text = s; }
                break;
        }
    }

    OptionType ParseType<T>()
    {
        return ParseType(typeof(T));
    }
    OptionType ParseType(Type type)
    {
        if (type == typeof(float))
        {
            return OptionType.Float;
        }
        else if (type == typeof(int))
        {
            return OptionType.Int;
        }
        else if (type == typeof(bool))
        {
            return OptionType.Bool;
        }
        else if (type == typeof(string))
        {
            return OptionType.String;
        }
        else
        {
            throw new("Invalid Type");
        }
    }
    Type ToType(OptionType optionType)
    {
        switch (optionType)
        {
            case OptionType.Float: return typeof(float);
            case OptionType.Int: return typeof(float);
            case OptionType.Bool: return typeof(float);
            case OptionType.String: return typeof(float);
        }

        return null;
    }
    Type AutoParseString(string str, out object result)
    {
        if (float.TryParse(str, out float f))
        {
            result = f;
            return typeof(float);
        }
        else if (int.TryParse(str, out int i))
        {
            result = i;
            return typeof(int);
        }
        else if (bool.TryParse(str, out bool b))
        {
            result = b;
            return typeof(bool);
        }
        else
        {
            result = str;
            return typeof(string);
        }
    }
}
