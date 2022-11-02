using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using Mike;

public class InputManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinConter;
    [SerializeField] private TextMeshProUGUI gemConter;
    [SerializeField] private TextMeshProUGUI scoreCounter;
    [SerializeField] private TextMeshProUGUI levelCounter;

    static InputManager _instance;
    public static InputManager Instance { get => _instance; }

    private void Awake()
    {
        _instance = this;
    }


    //---------------------------------------


    private void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }


    //--------------------------------------


    internal void UpdateUI()
    {
        if (coinConter != null) coinConter.text = MikeString.ConvertNumberToString(GameManager.Insatnce.Coins);
        if (gemConter != null) gemConter.text = MikeString.ConvertNumberToString(GameManager.Insatnce.Gems);
        if (levelCounter != null) levelCounter.text = "Level: " + GameManager.Insatnce.Level.ToString();
    }



    public void LoadScene(string scene)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(scene);
    }

    public void Upgrade(string upgrade, Upgrade.VariableType variableType, float addValue, float upgradeMultiplier = 2, float costMultiplier = 2)
    {
        switch (variableType)
        {
            case global::Upgrade.VariableType.Integer:

                //check if player has enough coins
                if (GameManager.Insatnce.Coins - Mathf.Round(PlayerPrefs.GetInt(upgrade, 10) * costMultiplier) >= 0)
                {
                    //remove coins
                    GameManager.Insatnce.AddCoins((ulong)Mathf.RoundToInt(-PlayerPrefs.GetInt(upgrade, 10) * costMultiplier));

                    //change value
                    PlayerPrefs.SetInt(upgrade, Mathf.RoundToInt(PlayerPrefs.GetInt(upgrade) * upgradeMultiplier + addValue));
                }
                break;

            case global::Upgrade.VariableType.Float:

                if (GameManager.Insatnce.Coins - Mathf.Round(PlayerPrefs.GetFloat(upgrade, 10) * costMultiplier) >= 0)
                {
                    //remove coins
                    GameManager.Insatnce.AddCoins((ulong)Mathf.RoundToInt(-PlayerPrefs.GetFloat(upgrade, 10) * costMultiplier));

                    //change value
                    PlayerPrefs.SetFloat(upgrade, PlayerPrefs.GetFloat(upgrade) * upgradeMultiplier + addValue);
                }
                break;
        }

        PlayerPrefs.SetInt("Current Level", PlayerPrefs.GetInt("Starting Level", 1));
    }

    public void SetTimeScale(float scale = 1)
    {
        Time.timeScale = scale;
    }

    public void Retry()
    {
        PlayerPrefs.SetInt("Current Level", PlayerPrefs.GetInt("Starting Level", 1));
        LoadScene("Level");
    }
}
