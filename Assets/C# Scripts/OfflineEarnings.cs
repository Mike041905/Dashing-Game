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
        DontDestroyOnLoad(gameObject);
    }


    [SerializeField] private GameObject popUp;
    [SerializeField] private TextMeshProUGUI coinsGained;
    [SerializeField] private int _maxOfflineTime;


    const string LastLoginDateSaveKey = "Last Login Date";

    DateTime _UTCNow;
    DateTime LastLoginDate { get => StorageManager.GetDate(LastLoginDateSaveKey, _UTCNow); set => StorageManager.SaveDate(LastLoginDateSaveKey, value); }
    TimeSpan TimeDifference { get => _UTCNow - LastLoginDate; }

    double? _coins;

    async void Start()
    {
        if (Instance != this) { return; }

        _UTCNow = await Task.Run(GetUTCNowInternet);

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
        response.Close();
        response.Dispose();
        return DateTime.Parse(res).ToUniversalTime();
    }

    void ShowCoinsGained()
    {
        if(popUp == null) { return; }

        _coins = math.round(CalculateOfflineCoins(TimeDifference, _maxOfflineTime));
        coinsGained.text = MikeString.ConvertNumberToString(_coins.Value);

        popUp.SetActive(coinsGained.text != "0");
    }

    public void GiveOfflineEarnings()
    {
        if(_coins == null) { return; }
        GameManager.Insatnce.AddCoins(_coins.Value);

        SetNewLastLoginDate();
        _coins = null;
    }

    double CalculateOfflineCoins(TimeSpan time, float maxHours = Mathf.Infinity)
    {
        return (time.TotalHours > maxHours ? maxHours : time.TotalHours) * 60 * Upgrade.GetUpgrade("Offline Earnings", UpgradeData.VariableType.Float);
    }

    void SetNewLastLoginDate()
    {
        LastLoginDate = _UTCNow;
        NotificationManager.Instance.SendCollectCoinsNotification();
    }
}
