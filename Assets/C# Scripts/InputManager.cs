using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using Mike;
using UnityEngine.Events;

public class InputManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinCounter;
    [SerializeField] private TextMeshProUGUI gemCounter;
    [SerializeField] private TextMeshProUGUI scoreCounter;
    [SerializeField] private TextMeshProUGUI levelCounter;

    static InputManager _instance;
    public static InputManager Instance { get => _instance; }

    private void Awake()
    {
        _instance = this;
    }


    //---------------------------------------

    public event UnityAction OnUpgrade;

    //---------------------------------------


    private void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }


    //--------------------------------------


    internal void UpdateUI()
    {
        if (coinCounter != null) coinCounter.text = MikeString.ConvertNumberToString(GameManager.Insatnce.Coins);
        if (gemCounter != null) gemCounter.text = MikeString.ConvertNumberToString(GameManager.Insatnce.Gems);
        if (levelCounter != null) levelCounter.text = "Level: " + GameManager.Insatnce.Level.ToString();
    }



    public void LoadScene(string scene)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(scene);
    }

    public void Upgrade(UpgradeData data)
    {
        // I have no idea why im handling this in a seperate class ¯\_(?)_/¯

        switch (data.variableType)
        {
            case UpgradeData.VariableType.Integer:

                //check if player has enough coins
                if (GameManager.Insatnce.Coins - data.Cost >= 0)
                {
                    //remove coins
                    GameManager.Insatnce.RemoveCoins(data.Cost);

                    //change value
                    PlayerPrefs.SetInt(data.variableSaveKey, (int) data.NextUpgradeValue);
                }
                break;

            case UpgradeData.VariableType.Float:

                if (GameManager.Insatnce.Coins - data.Cost >= 0)
                {
                    //remove coins
                    GameManager.Insatnce.RemoveCoins(data.Cost);

                    //change value
                    PlayerPrefs.SetFloat(data.variableSaveKey, data.NextUpgradeValue);
                }
                break;
        }

        OnUpgrade?.Invoke();
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
