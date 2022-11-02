using UnityEngine;
using TMPro;
using Mike;

[System.Serializable]
public struct UpgradeData
{
    public string variable;
    public VariableType variableType;
    public float startingValue;
    public float upgradeAdditionValue;
    public float upgradeMultiplier;
    public float costMultiplier;

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
    [SerializeField] public InputManager inputManager;
    [SerializeField] public TextMeshProUGUI stats;
    [SerializeField] public TextMeshProUGUI cost;

    [Header("Stats")]
    public UpgradeData upgradeData;

    //----------------------------------------------------


    private void Awake()
    {
        InitializeVariable();
        UpdateUpgradeDetails();
    }


    //----------------------------------------------------


    public void Up()
    {
        inputManager.Upgrade(upgradeData.variable, upgradeData.variableType, upgradeData.upgradeAdditionValue, upgradeData.upgradeMultiplier, upgradeData.costMultiplier);
        UpdateUpgradeDetails();
    }

    public void InitializeVariable()
    {
        if(PlayerPrefs.HasKey(upgradeData.variable)) { return; }
        switch (upgradeData.variableType)
        {
            case UpgradeData.VariableType.Integer:
                PlayerPrefs.SetInt(upgradeData.variable, Mathf.RoundToInt(upgradeData.startingValue));
                break;
            case UpgradeData.VariableType.Float:
                PlayerPrefs.SetFloat(upgradeData.variable, upgradeData.startingValue);
                break;
        }
    }

    void UpdateUpgradeDetails()
    {
        switch (upgradeData.variableType)
        {
            case UpgradeData.VariableType.Integer:
                stats.text = PlayerPrefs.GetInt(upgradeData.variable) + " > " + Mathf.Round(PlayerPrefs.GetInt(upgradeData.variable) * upgradeData.upgradeMultiplier + upgradeData.upgradeAdditionValue);
                cost.text = MikeString.ConvertNumberToString(Mathf.Round(PlayerPrefs.GetInt(upgradeData.variable) * upgradeData.costMultiplier));
                break;

            case UpgradeData.VariableType.Float:
                stats.text = MikeString.ConvertNumberToString(MikeMath.Round(PlayerPrefs.GetFloat(upgradeData.variable), 2)) + " > " + MikeString.ConvertNumberToString(MikeMath.Round(PlayerPrefs.GetFloat(upgradeData.variable) * upgradeData.upgradeMultiplier + upgradeData.upgradeAdditionValue, 2));
                cost.text = MikeString.ConvertNumberToString(Mathf.Round(PlayerPrefs.GetFloat(upgradeData.variable) * upgradeData.costMultiplier));
                break;
        }
    }
}
