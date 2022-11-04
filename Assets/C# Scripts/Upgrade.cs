using Mike;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct UpgradeData
{
    public string variableSaveKey;
    public VariableType variableType;
    public float startingValue;
    public float upgradeAdditionValue;
    public float upgradeMultiplier;
    public float costMultiplier;
    public float costOffset;

    // LMAO why can Properties return values bigger that their soposed max value
    /// <summary>
    /// Automaticaly converts to correct value
    /// </summary>
    public float UpgradeValue
    {
        get
        {
            return variableType == VariableType.Integer
                ? GetIntWithDefault(variableSaveKey, Mathf.RoundToInt(startingValue))
                : GetFloatWithDefault(variableSaveKey, startingValue);
        }
        set
        {
            if (variableType == VariableType.Integer)
            {
                PlayerPrefs.SetInt(variableSaveKey, Mathf.RoundToInt(value));
            }
            else
            {
                PlayerPrefs.SetFloat(variableSaveKey, value);
            }
        }
    }
    public float NextUpgradeValue 
    {
        get
        {
            if(variableType == VariableType.Integer)
            {
                return math.ceil(UpgradeValue * upgradeMultiplier + upgradeAdditionValue);
            }
            else
            {
                return UpgradeValue * upgradeMultiplier + upgradeAdditionValue;
            }
        }
    }
    public float Cost { get => math.ceil(UpgradeValue * costMultiplier + costOffset); }

    [System.Serializable]
    public enum VariableType
    {
        Integer,
        Float,
    }

    // IDK why unity wont asign these values on GetInt Automaticaly.
    int GetIntWithDefault(string key, int defaultValue)
    {
        if (!PlayerPrefs.HasKey(key)) { PlayerPrefs.SetInt(key, defaultValue); return defaultValue; }

        return PlayerPrefs.GetInt(key);
    }
    float GetFloatWithDefault(string key, float defaultValue)
    {
        if (!PlayerPrefs.HasKey(key)) { PlayerPrefs.SetFloat(key, defaultValue); return defaultValue; }

        return PlayerPrefs.GetFloat(key);
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

        InputManager.Instance.OnUpgrade += UpdateUpgradeButton;
    }


    private void UpdateUpgradeButton()
    {
        upgradeButton.interactable = GameManager.Insatnce.Coins >= UpgradeData.Cost;
    }

    public void Up()
    {
        InputManager.Instance.Upgrade(UpgradeData);
        UpdateUpgradeDetails();
    }

    private void UpdateUpgradeDetails()
    {
        upgradeName.text = UpgradeData.variableSaveKey;
        stats.text = MikeString.ConvertNumberToString(UpgradeData.UpgradeValue) + " >> " + MikeString.ConvertNumberToString(UpgradeData.NextUpgradeValue);
        cost.text = MikeString.ConvertNumberToString(UpgradeData.Cost);
    }
}
