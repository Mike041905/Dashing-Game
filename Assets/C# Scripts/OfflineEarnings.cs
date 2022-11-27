using System;
using UnityEngine;
using TMPro;
using Mike;
using System.Net;
using System.Threading.Tasks;
using Unity.Mathematics;

public class OfflineEarnings : MonoBehaviour
{
    static OfflineEarnings _instance;
    public static OfflineEarnings Instance { get => _instance; }

    private void Awake()
    {
        if(Instance != null) { DestroyImmediate(gameObject); return; }

        _instance = this;
    }


    [SerializeField] private GameObject popUp;
    [SerializeField] private TextMeshProUGUI coinsGained;
    [SerializeField] private int _maxOfflineTime;


    const string LastLoginDateSaveKey = "Last Login Date";

    DateTime UTCNow { get => GetUTCNowInternet(); }
    DateTime LastLoginDate { get => StorageManager.GetDate(LastLoginDateSaveKey, UTCNow); set => StorageManager.SaveDate(LastLoginDateSaveKey, value); }
    TimeSpan TimeDifference { get => UTCNow - LastLoginDate; }


    void Start()
    {
        DontDestroyOnLoad(gameObject);

        // Run on background thread as web request may take too long
        ShowCoinsGained();
    }

    private void OnApplicationQuit() => SetNewLastLoginDate();


    //------------------------


    public DateTime GetUTCNowInternet()
    {
        HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create("http://google.com");
        myHttpWebRequest.Timeout = 10000; // 10s timeout
        WebResponse response = myHttpWebRequest.GetResponse();
        string res = response.Headers["date"];
        response.Dispose();
        return DateTime.Parse(res).ToUniversalTime();
    }

    void ShowCoinsGained()
    {
        if(popUp == null) { return; }

        coinsGained.text = MikeString.ConvertNumberToString(math.round(CalculateOfflineCoins(TimeDifference, _maxOfflineTime)));

        popUp.SetActive(coinsGained.text != "0");
    }

    public void GiveOfflineEarnings()
    {
        GameManager.Insatnce.AddCoins(CalculateOfflineCoins(TimeDifference, _maxOfflineTime));

        SetNewLastLoginDate();
    }

    double CalculateOfflineCoins(TimeSpan time, float maxHours = Mathf.Infinity)
    {
        return (time.TotalHours > maxHours ? maxHours : time.TotalHours) * Upgrade.GetUpgrade("Offline Earnings", UpgradeData.VariableType.Float);
    }

    void SetNewLastLoginDate()
    {
        LastLoginDate = UTCNow;
        NotificationManager.Instance.SendCollectCoinsNotification();
    }
}
