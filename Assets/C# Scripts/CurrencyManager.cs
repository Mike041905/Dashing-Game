using System;
using Unity.Mathematics;
using UnityEngine;

public class Currency
{
    const string KeySuffix = "_Currency";

    public string Name;
    public uint ID;

    public string SaveKey;

    [SerializeField] double _value = 0;

    public Currency(string name, uint iD, string savePath, double value, bool wholeNumbers)
    {
        Name = name;
        ID = iD;
        SaveKey = savePath;
        _value = value;
        WholeNumbers = wholeNumbers;
    }

    public double Value { get => WholeNumbers ? math.round(_value) : _value; private set => _value = value; }

    [SerializeField] bool _wholeNumbers = false;
    public bool WholeNumbers { get => _wholeNumbers; private set => _wholeNumbers = value; }

    public virtual double Get() { return _value; }
    public virtual void Set(double value) { _value = value; Save(); }
    public virtual void Add(double value) { _value += value; Save(); }
    public virtual void Remove(double value) { _value -= value; Save(); }

    public Currency Save()
    {
        PlayerPrefs.SetString(SaveKey + KeySuffix, JsonUtility.ToJson(this));

        return this;
    }

    public static Currency Load(string key)
    {
        string jsonStr = PlayerPrefs.GetString(key + KeySuffix, "");
        return jsonStr == "" ? null : JsonUtility.FromJson<Currency>(jsonStr);
    }
}

[Serializable]
public struct CurrencyManifestData
{
    public string Name;
    public uint ID;
    public string SavePath;
    public double StartingValue;
    public bool WholeValues;

    public Currency LoadData()
    {
        Currency currency = Currency.Load(SavePath);
        
        if(currency == null)
        {
            return new Currency(Name, ID, SavePath, StartingValue, WholeValues).Save();
        }

        return currency;
    }
}

[Serializable]
public struct CurrencyManifest
{
    public CurrencyManifestData[] Manifest;
}

public class CurrencyManager : MonoBehaviour
{
    [SerializeField] CurrencyManifest _manifest;
    public Currency[] Currencies { get; private set; } = new Currency[0];


    static CurrencyManager _instance;
    public static CurrencyManager Instance { get => _instance; }

    private void Awake()
    {
        InitializeSingleton();

        if(_manifest.Manifest == null) { throw new("Currency manifest cannot be empty!"); }
        LoadCurrencies();

        for (int i = 0; i < Currencies.Length; i++)
        {
            print
            (
                $"ID: {Currencies[i].ID}\n" +
                $"Name: {Currencies[i].Name}\n" +
                $"Value: {Currencies[i].Value}\n" +
                $"WholeValues {Currencies[i].WholeNumbers}"
            );
        }
    }
    
    void InitializeSingleton()
    {
        if (_instance != null) { Destroy(this); ; return; }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void LoadCurrencies()
    {
        Currencies = new Currency[_manifest.Manifest.Length];

        for (int i = 0; i < _manifest.Manifest.Length; i++)
        {
            Currencies[i] = _manifest.Manifest[i].LoadData();
        }
    }

    public Currency GetCurrency(uint id)
    {
        foreach (Currency currency in Currencies)
        {
            if(currency.ID == id) { return currency; }
        }

        return null;
    }
    
    public Currency GetCurrency(string name)
    {
        foreach (Currency currency in Currencies)
        {
            if(currency.Name == name) { return currency; }
        }

        return null;
    }
}
