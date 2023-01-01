using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AdaptivePerformance;
using UnityEngine.Advertisements;
using UnityEngine.Android;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    #region Instance
    private static GameManager _instance;
    public static GameManager Insatnce
    {
        get { return _instance; }
    }
    #endregion


    //----------------------------------------------

    // I'm unsure if i should use BigInt or double/ulong instead of int (I'm lazy and dont want to refactor more code)

    public GameObject CoinPrefab;
    [SerializeField] private GameObject _portal;

    [SerializeField] int _bossSpawnLevelInterval = 5;
    [SerializeField] int _bossSpawnLevelOffset = 0;

    [SerializeField] int _levelSaveInterval = 5;
    [SerializeField] int _levelSaveOffset = 1;

    private GameObject _portalInstance;


    //----------------------------------------------

    public double Coins { get => CurrencyManager.Instance.GetCurrency(0).Value; private set { CurrencyManager.Instance.GetCurrency(0).Set(value); InputManager.Instance.UpdateUI(); } }
    public ulong Gems { get => (ulong)CurrencyManager.Instance.GetCurrency(1).Value; private set { CurrencyManager.Instance.GetCurrency(1).Set(value); InputManager.Instance.UpdateUI(); } }
    public int Level { get; private set; }
    public bool IsBossLevel { get => Level % _bossSpawnLevelInterval == 0 + _bossSpawnLevelOffset; }
    [field: SerializeField] private float _coinsPerDifficultyMultiplier = .7f;
    public float CoinsPerDifficulty { get => Difficulty * _coinsPerDifficultyMultiplier;}
    [field: SerializeField] public float StartingDifficulty { get; private set; }
    [field: SerializeField] public float DifficultyPerLevelAddition { get; private set; }
    [field: SerializeField] public float DifficultyPerLevelMultiplier { get; private set; }
    [field: SerializeField] public float DifficultyPerLevelOffset { get; private set; }
    public float Difficulty { get => Mathf.Pow(DifficultyPerLevelMultiplier, Level - 1) * StartingDifficulty + DifficultyPerLevelOffset + Mathf.Pow(DifficultyPerLevelAddition, Level); }

    //----------------------------------------------

    public event UnityAction<int> OnLevelChanged;
    public UnityEvent OnLevelChangedEvent;

    //----------------------------------------------


    private void Awake()
    {
        _instance = this;
        Application.targetFrameRate = Screen.currentResolution.refreshRate;

        Initialize();

        if(Holder.Instance != null)
        {
            IDevicePerformanceControl ctrl = Holder.Instance.DevicePerformanceControl;
            ctrl.CpuLevel = ctrl.MaxCpuPerformanceLevel;
            ctrl.GpuLevel = ctrl.MaxGpuPerformanceLevel;
            ctrl.CpuPerformanceBoost = true;
            ctrl.GpuPerformanceBoost = true;
        }
    }

    private void Start()
    {
        InputManager.Instance.UpdateUI();
        if (LevelGanerator.Instance != null) { LevelGanerator.Instance.GenerateLevel(); }
    }

    private void OnApplicationQuit()
    {
        SaveCoins();
        SaveGems();
    }


    //----------------------------------------------


    #region Private methods
    /// <summary>
    /// Sets up GameManager
    /// </summary>
    private void Initialize()
    {
        Level = StorageManager.Game.StartingLevel;
    }

    /// <summary>
    /// Saves Coins
    /// </summary>
    private void SaveCoins()
    {
        CurrencyManager.Instance.GetCurrency(0).Save();
    }

    /// <summary>
    /// Saves Gems
    /// </summary>
    private void SaveGems()
    {
        CurrencyManager.Instance.GetCurrency(1).Save();
    }

    /// <summary>
    /// handles level transition and acounts for all variables involved
    /// </summary>
    private IEnumerator NextLevel()
    {
        EZCameraShake.CameraShaker.Instance.ShakeOnce(15, 25, 4f, 1f);
        if (Player.Instance.PlayerDash.CurrentDash != null) { StopCoroutine(Player.Instance.PlayerDash.CurrentDash); }
        Player.Instance.PlayerDash.enabled = false;
        yield return new WaitForSeconds(2f);

        Player.Instance.transform.position = Vector3.zero;

        Level++;
        SaveStartingLevel();
        OnLevelChanged?.Invoke(Level);
        OnLevelChangedEvent?.Invoke();

        InputManager.Instance.UpdateUI();
        Destroy(_portalInstance);
        LevelGanerator.Instance.RegenerateLevel();
        Player.Instance.PlayerDash.enabled = true;
    }

    void SaveStartingLevel()
    {
        if (Level % _levelSaveInterval == 0 + _levelSaveOffset) { StorageManager.Game.StartingLevel = Level; }
    }
    #endregion


    //----------------------------------------------


    public void SetCoins(double ammount)
    {
        CurrencyManager.Instance.GetCurrency(0).Set(ammount);

        InputManager.Instance.UpdateUI();
    }

    public void AddCoins(double ammount)
    {
        CurrencyManager.Instance.GetCurrency(0).Add(ammount);

        InputManager.Instance.UpdateUI();
    }

    public void RemoveCoins(double ammount)
    {
        CurrencyManager.Instance.GetCurrency(0).Remove(ammount);

        InputManager.Instance.UpdateUI();
    }

    public void AddGems(ulong ammount)
    {
        CurrencyManager.Instance.GetCurrency(1).Add(ammount);

        InputManager.Instance.UpdateUI();
    }

    public void SpawnPortal(Room portalRoom)
    {
        _portalInstance = Instantiate(_portal, portalRoom.transform.position, Quaternion.identity);
    }

    public void MoveToNextLevel()
    {
        StartCoroutine(NextLevel());
    }
}
