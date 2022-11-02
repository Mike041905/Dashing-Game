using Mike;
using TMPro;
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

    [System.Serializable]
    public enum VariableType
    {
        Integer,
        Float,
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


    /// <summary>
    /// Automaticaly converts to correct value
    /// </summary>
    public float UpgradeValue
    {
        get
        {
            return UpgradeData.variableType == UpgradeData.VariableType.Integer
                ? PlayerPrefs.GetInt(UpgradeData.variableSaveKey, Mathf.RoundToInt(UpgradeData.startingValue))
                : PlayerPrefs.GetFloat(UpgradeData.variableSaveKey, UpgradeData.startingValue);
        }
        set
        {
            if (UpgradeData.variableType == UpgradeData.VariableType.Integer)
            {
                PlayerPrefs.SetInt(UpgradeData.variableSaveKey, Mathf.RoundToInt(value));
            }
            else
            {
                PlayerPrefs.SetFloat(UpgradeData.variableSaveKey, value);
            }
        }
    }
    public int Cost { get => Mathf.CeilToInt(UpgradeValue * UpgradeData.costMultiplier + UpgradeData.costOffset); }


    //----------------------------------------------------


    void Initialize()
    {
        UpdateUpgradeDetails();
        UpdateUpgradeButton();

        InputManager.Instance.OnUpgrade += UpdateUpgradeButton;
    }


    private void UpdateUpgradeButton()
    {
        upgradeButton.interactable = GameManager.Insatnce.Coins < Mathf.Round(PlayerPrefs.GetInt(UpgradeData.variableSaveKey) * UpgradeData.costMultiplier);
    }

    public void Up()
    {
        InputManager.Instance.Upgrade(UpgradeData);
        UpdateUpgradeDetails();
    }

    private void UpdateUpgradeDetails()
    {
        upgradeName.text = UpgradeData.variableSaveKey;
        stats.text = MikeString.ConvertNumberToString(UpgradeValue) + " >> " + MikeString.ConvertNumberToString(UpgradeValue * UpgradeData.upgradeMultiplier + UpgradeData.upgradeAdditionValue);
        cost.text = MikeString.ConvertNumberToString(Cost);
    }
}
