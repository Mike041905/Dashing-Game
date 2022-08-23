using UnityEngine;
using TMPro;
using Mike;

public class Upgrade : MonoBehaviour
{
    [Header("Referencess")]
    [SerializeField] private InputManager inputManager;
    [SerializeField] private TextMeshProUGUI stats;
    [SerializeField] private TextMeshProUGUI cost;

    [Header("Stats")]
    [SerializeField] private string variable;
    [SerializeField] private VariableType variableType;
    [SerializeField] private float defaultValue;
    [SerializeField] private float upgradeAdditionValue = 0;
    [SerializeField] private float upgradeMultiplier = 2;
    [SerializeField] private float costMultiplier = 2;

    [System.Serializable]
    public enum VariableType
    {
        Integer,
        Float,
    }


    //----------------------------------------------------


    private void Awake()
    {
        InitializeVariable();
        UpdateUpgradeDetails();
    }


    //----------------------------------------------------


    public void Up()
    {
        inputManager.Upgrade(variable, variableType, upgradeAdditionValue, upgradeMultiplier, costMultiplier);
        UpdateUpgradeDetails();
    }

    public void InitializeVariable()
    {
        if(PlayerPrefs.HasKey(variable)) { return; }
        switch (variableType)
        {
            case VariableType.Integer:
                PlayerPrefs.SetInt(variable, Mathf.RoundToInt(defaultValue));
                break;
            case VariableType.Float:
                PlayerPrefs.SetFloat(variable, defaultValue);
                break;
        }
    }

    void UpdateUpgradeDetails()
    {
        switch (variableType)
        {
            case VariableType.Integer:
                stats.text = PlayerPrefs.GetInt(variable) + " > " + Mathf.Round(PlayerPrefs.GetInt(variable) * upgradeMultiplier + upgradeAdditionValue);
                cost.text = MikeString.ConvertNumberToString(Mathf.Round(PlayerPrefs.GetInt(variable) * costMultiplier));
                break;

            case VariableType.Float:
                stats.text = MikeString.ConvertNumberToString(MikeMath.Round(PlayerPrefs.GetFloat(variable), 2)) + " > " + MikeString.ConvertNumberToString(MikeMath.Round(PlayerPrefs.GetFloat(variable) * upgradeMultiplier + upgradeAdditionValue, 2));
                cost.text = MikeString.ConvertNumberToString(Mathf.Round(PlayerPrefs.GetFloat(variable) * costMultiplier));
                break;
        }
    }
}
