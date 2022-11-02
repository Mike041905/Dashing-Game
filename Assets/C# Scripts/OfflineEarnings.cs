using System;
using UnityEngine;
using TMPro;
using Mike;

public class OfflineEarnings : MonoBehaviour
{
    [SerializeField] private GameObject popUp;
    [SerializeField] private TextMeshProUGUI coinsGained;

    void Start()
    {
        if (RemoveIfIsDuplicate()) return;
        DontDestroyOnLoad(gameObject);

        InitializeOfflineEarnings();
        ShowCoinsGained();
    }

    private void OnApplicationQuit() => SetNewLastLoginDate();


    //------------------------


    bool RemoveIfIsDuplicate()
    {
        if (GameObject.FindGameObjectsWithTag("OfflineEarner").Length > 1) { Destroy(gameObject); return true; }

        return false;
    }

    void ShowCoinsGained()
    {

        InitializeOfflineEarnings();
        if (Mathf.RoundToInt((float)(DateTime.UtcNow - DateTime.Parse(PlayerPrefs.GetString("Last Login Date"))).TotalHours) >= 10f)
        {
            coinsGained.text = MikeString.ConvertNumberToString(Mathf.RoundToInt(432 * PlayerPrefs.GetFloat("Offline Earnings")));
        }
        else
        {
            coinsGained.text = MikeString.ConvertNumberToString(Mathf.RoundToInt((float)(DateTime.UtcNow - DateTime.Parse(PlayerPrefs.GetString("Last Login Date"))).TotalSeconds / 100 * PlayerPrefs.GetFloat("Offline Earnings")));
        }

        if(coinsGained.text != "0")
        {
            popUp.SetActive(true);
        }
    }

    public void GiveOfflineEarnings()
    {
        if(Mathf.RoundToInt((float)(DateTime.UtcNow - DateTime.Parse(PlayerPrefs.GetString("Last Login Date"))).TotalHours) >= 10f)
        {
            GameManager.Insatnce.AddCoins((ulong)Mathf.RoundToInt(432 * PlayerPrefs.GetFloat("Offline Earnings")));
        }
        else
        {
            GameManager.Insatnce.AddCoins((ulong)Mathf.RoundToInt( (float) (DateTime.UtcNow - DateTime.Parse(PlayerPrefs.GetString("Last Login Date")) ).TotalSeconds / 100 * PlayerPrefs.GetFloat("Offline Earnings")));
        }

        SetNewLastLoginDate();
    }

    void SetNewLastLoginDate()
    {
        PlayerPrefs.SetString("Last Login Date", DateTime.UtcNow.ToString("G") + "");
    }

    void InitializeOfflineEarnings()
    {
        if (!PlayerPrefs.HasKey("Last Login Date"))
        {
            PlayerPrefs.SetString("Last Login Date", DateTime.UtcNow.ToString("G") + "");
        }
    }
}
