using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DevCodes : MonoBehaviour
{
    public string Code { set;  get; }

    public void EnterCode()
    {
        string rawCode = Code;
        Code = ToLowerNoSpaces(Code);

        if (Code.StartsWith("playerprefs.setint("))
        {
            ModifyPlayerPrefsInt(rawCode);
            return;
        }
        else if (Code.StartsWith("playerprefs.setfloat("))
        {
            ModifyPlayerPrefsFloat(rawCode);
            return;
        }
        else if (Code.StartsWith("playerprefs.setstring("))
        {
            ModifyPlayerPrefsString(rawCode);
            return;
        }
        else if (Code == "reloadscene()")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            return;
        }
        else if (Code.StartsWith("coins="))
        {
            GameManager.Insatnce.SetCoins(double.Parse(Code.Remove(0, Code.IndexOf("=") + 1)));
            return;
        }
        else if (Code.StartsWith("gems="))
        {
            GameManager.Insatnce.AddGems(ulong.Parse(Code.Remove(0, Code.IndexOf("=") + 1)) - GameManager.Insatnce.Gems);
            return;
        }
        else if (Code == "resetprogress()")
        {
            StorageManager.ResetProgress();
            SceneManager.LoadScene(0);
            return;
        }

        Debug.LogError("There is no such DevCode as: \"" + Code + "\"");
    }

    void ModifyPlayerPrefsInt(string code)
    {
        string variable;
        string val;

        variable = code.Remove(0, code.IndexOf("\"") + 1);
        variable = variable.Remove(variable.IndexOf("\""));

        val = code.Remove(0, code.IndexOf(",") + 1);
        val = val.Remove(val.IndexOf(")"));

        PlayerPrefs.SetInt(variable, int.Parse(val));
    }

    void ModifyPlayerPrefsFloat(string code)
    {
        string variable;
        string val;

        variable = code.Remove(0, code.IndexOf("\"") + 1);
        variable = variable.Remove(variable.IndexOf("\""));

        val = code.Remove(0, code.IndexOf(",") + 1);
        val = val.Remove(val.IndexOf(")"));

        PlayerPrefs.SetFloat(variable, float.Parse(val));
    }
    
    void ModifyPlayerPrefsString(string code)
    {
        string variable;
        string val;

        variable = code.Remove(0, code.IndexOf("\"") + 1);
        variable = variable.Remove(variable.IndexOf("\""));

        val = code.Remove(0, code.IndexOf("\"") + 1);
        val = val.Remove(val.IndexOf("\""));

        PlayerPrefs.SetString(variable, val);
    }
    public string ToLowerNoSpaces(string val)
    {
        val = val.ToLower();
        val = string.Concat(val.Where(c => !char.IsWhiteSpace(c)));

        return val;
    }
}
