using System;
using UnityEngine;
using TMPro;
using Mike;

public class OfflineEarnings : MonoBehaviour
{
    [SerializeField] private GameObject popUp;
    [SerializeField] private TextMeshProUGUI coinsGained;
    [SerializeField] private int _maxOfflineTime;

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

        if (Mathf.Ceil((float)(DateTime.UtcNow - DateTime.Parse(PlayerPrefs.GetString("Last Login Date"))).TotalHours) >= _maxOfflineTime)
        {
            coinsGained.text = MikeString.ConvertNumberToString(Mathf.RoundToInt(_maxOfflineTime * 60 * Upgrade.GetUpgrade("Offline Earnings", UpgradeData.VariableType.Float)));
        }
        else
        {
            coinsGained.text = MikeString.ConvertNumberToString(Mathf.RoundToInt((float)(DateTime.UtcNow - DateTime.Parse(PlayerPrefs.GetString("Last Login Date"))).TotalSeconds / 100 * Upgrade.GetUpgrade("Offline Earnings",UpgradeData.VariableType.Float)));
        }

        popUp.SetActive(coinsGained.text != "0");
    }

    public void GiveOfflineEarnings()
    {
        if(Mathf.RoundToInt((float)(DateTime.UtcNow - DateTime.Parse(PlayerPrefs.GetString("Last Login Date"))).TotalHours) >= 10f)
        {
            GameManager.Insatnce.AddCoins((ulong)Mathf.RoundToInt(432 * Upgrade.GetUpgrade("Offline Earnings", UpgradeData.VariableType.Float)));
        }
        else
        {
            GameManager.Insatnce.AddCoins((ulong)Mathf.RoundToInt( (float) (DateTime.UtcNow - DateTime.Parse(PlayerPrefs.GetString("Last Login Date")) ).TotalSeconds / 100 * Upgrade.GetUpgrade("Offline Earnings", UpgradeData.VariableType.Float)));
        }

        SetNewLastLoginDate();
    }

    void SetNewLastLoginDate()
    {
        PlayerPrefs.SetString("Last Login Date", DateTime.UtcNow.ToString("G") + "");
        NotificationManager.Instance.SendCollectCoinsNotification();
    }

    void InitializeOfflineEarnings()
    {
        if (!PlayerPrefs.HasKey("Last Login Date"))
        {
            PlayerPrefs.SetString("Last Login Date", DateTime.UtcNow.ToString("G") + "");
        }
    }
}
