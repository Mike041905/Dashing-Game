using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class InputManager : MonoBehaviour
{
    [SerializeField] private GameObject updateNotes;
    [SerializeField] private GameObject upgrades;


    //---------------------------------------


    private void Start()
    {
        InitializeUpgradeVariables();

        ShowUpdateNotes();

        MenuExclusive();

        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }


    //--------------------------------------

    void MenuExclusive()
    {
        if (SceneManager.GetActiveScene().name == "Menu")
        {
            PlayerPrefs.SetInt("Current Level", PlayerPrefs.GetInt("Starting Level", 1)); 
        }
    }

    void InitializeUpgradeVariables()
    {
        if(upgrades != null) SendMessageToAllDescendants(upgrades.transform, "InitializeVariable");
    }

    void SendMessageToAllDescendants(Transform transform, string message)
    {
        foreach (Transform item in transform)
        {
            bool active = item.gameObject.activeSelf;

            item.gameObject.SetActive(true);

            item.SendMessage(message, SendMessageOptions.DontRequireReceiver);
            SendMessageToAllDescendants(item, message);

            item.gameObject.SetActive(active);
        }
    }

    void ShowUpdateNotes()
    {
        if(updateNotes == null) { return; }
        if (Application.version == PlayerPrefs.GetString("Last Version", "")) { return; }
        else { PlayerPrefs.SetString("Last Version", Application.version); PlayerPrefs.SetInt("Seen Update Notes", 0); }
        if(PlayerPrefs.GetInt("New Player", 1) == 1) { PlayerPrefs.SetInt("New Player", 0); return; }
        if(PlayerPrefs.GetInt("Seen Update Notes", 0) == 1) { return; }
            PlayerPrefs.SetInt("Seen Update Notes", 1);
            updateNotes.SetActive(true);
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
                    GameManager.Insatnce.AddCoins(Mathf.RoundToInt(-PlayerPrefs.GetInt(upgrade, 10) * costMultiplier));

                    //change value
                    PlayerPrefs.SetInt(upgrade, Mathf.RoundToInt(PlayerPrefs.GetInt(upgrade) * upgradeMultiplier + addValue));
                }
                break;

            case global::Upgrade.VariableType.Float:

                if (GameManager.Insatnce.Coins - Mathf.Round(PlayerPrefs.GetFloat(upgrade, 10) * costMultiplier) >= 0)
                {
                    //remove coins
                    GameManager.Insatnce.AddCoins(Mathf.RoundToInt(-PlayerPrefs.GetFloat(upgrade, 10) * costMultiplier));

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
