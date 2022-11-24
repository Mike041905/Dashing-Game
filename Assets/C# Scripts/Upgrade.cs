using Mike;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct UpgradeData
{
    public const string UPGRADE_SAVEKEY_SUFFIX = "_Upgrade";
    public const string UPGRADE_SAVEKEY_VALUE_SUFFIX = "_Value";

    public string SaveKeyLevel { get => (variableType == VariableType.Integer ? "I" : "F") + "_" + upgradeKey + UPGRADE_SAVEKEY_SUFFIX; }
    public string SaveKeyValue { get => SaveKeyLevel + UPGRADE_SAVEKEY_VALUE_SUFFIX; }
    public static string GetSaveKeyLevel(VariableType type, string upgradeKey) => (type == VariableType.Integer ? "I" : "F") + "_" + upgradeKey + UPGRADE_SAVEKEY_SUFFIX;
    public static string GetSaveKeyValue(VariableType type, string upgradeKey) => GetSaveKeyLevel(type, upgradeKey) + UPGRADE_SAVEKEY_VALUE_SUFFIX;


    [System.Serializable]
    public enum VariableType
    {
        Integer,
        Float
    }

    [Header("General")]
    [SerializeField] string upgradeKey;
    [SerializeField] string name;
    [SerializeField] VariableType variableType;
    [SerializeField] int maxLevel;

    [Header("Stat Scaler")]
    [SerializeField] float startingValue;
    [SerializeField] float upgradeAdditionValue;
    [SerializeField] float upgradeMultiplier;
    [SerializeField] float _upgradeOffset;

    [Header("Cost Scaler")]
    [SerializeField] float _startingCost;
    [SerializeField] float _costAddition;
    [SerializeField] float costMultiplier;
    [SerializeField] float costOffset;

    // LMAO why can Properties return values bigger that their soposed max value
    public string UpgradeName { get => name; }

    public int Level { get => GetLevel(); set { SetLevel(value); SaveUpgradeValue(); } }
    public bool MaxLevelReached { get => maxLevel > 0 && Level >= maxLevel; }

    /// <summary>
    /// Automaticaly converts to correct value
    /// </summary>
    public float UpgradeValue
    {
        get => LevelToUpgradeValue(Level);
    }
    public float NextUpgradeValue 
    {
        get => LevelToUpgradeValue(Level + 1);
    }

    public float Cost { get => math.ceil(Mathf.Pow(costMultiplier, Level - 1) * _startingCost + costOffset + Mathf.Pow(_costAddition, Level)); }


    //--------------------------


    float LevelToUpgradeValue(int level)
    {
        float val = Mathf.Pow(upgradeMultiplier, level - 1) * startingValue + _upgradeOffset + Mathf.Pow(upgradeAdditionValue, level);
        return variableType == VariableType.Integer ? Mathf.Ceil(val) : val;
    }

    int GetLevel()
    {
        int level = GetLevel(upgradeKey, variableType);
        SaveUpgradeValue(level);
        return level;
    }
    public static int GetLevel(string key, VariableType type)
    {
        if (!PlayerPrefs.HasKey(GetSaveKeyLevel(type, key))) 
        { 
            PlayerPrefs.SetInt(GetSaveKeyLevel(type, key), 1); 
            return 1; 
        }

        return PlayerPrefs.GetInt(GetSaveKeyLevel(type, key));
    }
    void SetLevel(int val)
    {
        PlayerPrefs.SetInt(SaveKeyLevel, val);
        SaveUpgradeValue();
    }

    void SaveUpgradeValue() => PlayerPrefs.SetFloat(SaveKeyValue, UpgradeValue);
    void SaveUpgradeValue(int level) => PlayerPrefs.SetFloat(SaveKeyValue, LevelToUpgradeValue(level));
    public static float GetUpgradeValue(string upgradeKey, VariableType type) => PlayerPrefs.GetFloat(GetSaveKeyValue(type, upgradeKey));


    public void Upgrade()
    {
        if (MaxLevelReached) { return; }

        Level++;
    }
}

public class Upgrade : MonoBehaviour
{
    [Header("Referencess")]
    public TextMeshProUGUI upgradeName;
    public TextMeshProUGUI stats;
    public TextMeshProUGUI cost;
    public Button upgradeButton;

    UpgradeData upgradeData;
    public UpgradeData UpgradeData { get => upgradeData; set { upgradeData = value; Initialize(); } }

    //----------------------------------------------------
     
    void Initialize()
    {
        UpdateUpgradeDetails();
        UpdateUpgradeButton();

        if (UpgradeData.MaxLevelReached) { enabled = false; return; }

        InputManager.Instance.OnUpgrade += UpdateUpgradeButton;
    }


    private void UpdateUpgradeButton()
    {
        upgradeButton.interactable = GameManager.Insatnce.Coins >= UpgradeData.Cost && !UpgradeData.MaxLevelReached;
    }

    public void Up()
    {
        if (GameManager.Insatnce.Coins - UpgradeData.Cost < 0 || UpgradeData.MaxLevelReached) { return; }

        GameManager.Insatnce.RemoveCoins(UpgradeData.Cost);
        UpgradeData.Upgrade();

        InputManager.Instance.Upgrade();
        UpdateUpgradeDetails();
    }

    private void UpdateUpgradeDetails()
    {
        if (UpgradeData.MaxLevelReached)
        {
            stats.text = MikeString.ConvertNumberToString(UpgradeData.UpgradeValue);
            cost.text = "MAX LEVEL";
        }
        else
        {
            upgradeName.text = UpgradeData.UpgradeName;
            stats.text = MikeString.ConvertNumberToString(UpgradeData.UpgradeValue) + " >> " + MikeString.ConvertNumberToString(UpgradeData.NextUpgradeValue);
            cost.text = MikeString.ConvertNumberToString(UpgradeData.Cost);
        }
    }

    public static float GetUpgrade(string upgradeKey, UpgradeData.VariableType type)
    {
        return UpgradeData.GetUpgradeValue(upgradeKey, type);
    }
}
