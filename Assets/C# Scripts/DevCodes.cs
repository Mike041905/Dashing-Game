using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DevCodes : MonoBehaviour
{
    public string Code { set; get; }

    public void EnterCode()
    {
        if (Code.StartsWith("PlayerPrefs.SetInt("))
        {
            ModifyPlayerPrefsInt(Code);
            return;
        }
        else if (Code.StartsWith("PlayerPrefs.SetFloat("))
        {
            ModifyPlayerPrefsFloat(Code);
            return;
        }
        else if (Code.StartsWith("PlayerPrefs.SetString("))
        {
            ModifyPlayerPrefsString(Code);
            return;
        }
        else if (Code == "ReloadScene()")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            return;
        }
        else if (Code.StartsWith("Coins="))
        {
            GameManager.Insatnce.AddCoins(int.Parse(Code.Remove(0, Code.IndexOf("=") + 1)) - GameManager.Insatnce.Coins);
            return;
        }
        else if (Code == "ResetProgress()")
        {
            PlayerPrefs.DeleteAll();
            SceneManager.LoadScene(0);
            return;
        }

        Debug.LogError("There is no such DevCode as: \"" + Code + "\"");
    }

    void ModifyPlayerPrefsInt(string code)
    {
        string variable;
        string value;

        variable = code.Remove(0, code.IndexOf("\"") + 1);
        variable = variable.Remove(variable.IndexOf("\""));

        value = code.Remove(0, code.IndexOf(",") + 1);
        value = value.Remove(value.IndexOf(")"));

        PlayerPrefs.SetInt(variable, int.Parse(value));
    }

    void ModifyPlayerPrefsFloat(string code)
    {
        string variable;
        string value;

        variable = code.Remove(0, code.IndexOf("\"") + 1);
        variable = variable.Remove(variable.IndexOf("\""));

        value = code.Remove(0, code.IndexOf(",") + 1);
        value = value.Remove(value.IndexOf(")"));

        PlayerPrefs.SetFloat(variable, float.Parse(value));
    }
    
    void ModifyPlayerPrefsString(string code)
    {
        string variable;
        string value;

        variable = code.Remove(0, code.IndexOf("\"") + 1);
        variable = variable.Remove(variable.IndexOf("\""));

        value = code.Remove(0, code.IndexOf("\"") + 1);
        value = value.Remove(value.IndexOf("\""));

        PlayerPrefs.SetString(variable, value);
    }
}
