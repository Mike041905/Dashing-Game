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

    private double _coinsDouble;


    //----------------------------------------------

    public long Coins { get => (long)_coinsDouble; }
    public ulong Gems { get; private set; }
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

        Gems = ulong.Parse(PlayerPrefs.GetString("Gems", "0"));
        _coinsDouble = double.Parse(PlayerPrefs.GetString("Coins", "0"));
        _coinsDouble = Coins;
    }

    /// <summary>
    /// Saves Coins
    /// </summary>
    private void SaveCoins()
    {
        PlayerPrefs.SetString("Coins", math.round(_coinsDouble).ToString("G20"));
    }

    /// <summary>
    /// Saves Gems
    /// </summary>
    private void SaveGems()
    {
        PlayerPrefs.SetString("Gems", Gems.ToString("G20"));
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
        _coinsDouble = ammount;

        SaveCoins();
        InputManager.Instance.UpdateUI();
    }

    public void AddCoins(double ammount)
    {
        // NOTE: double "-" operator returns a double and (double.MaxValue + 1 < 0) as it overloads the variable!
        if (_coinsDouble + ammount > double.MaxValue)
        {
            _coinsDouble = 0;
            SaveCoins();
            InputManager.Instance.UpdateUI();
            return;
        }

        if (_coinsDouble + ammount < 0)
        {
            _coinsDouble = double.MaxValue;
            SaveCoins();
            InputManager.Instance.UpdateUI();
            return;
        }

        _coinsDouble += ammount;

        SaveCoins();
        InputManager.Instance.UpdateUI();
    }

    public void RemoveCoins(double ammount)
    {
        // NOTE: double "-" operator returns a double and (double.MaxValue + 1 < 0) as it overloads the variable!
        if (_coinsDouble - ammount > double.MaxValue)
        {
            _coinsDouble = double.MaxValue;
            SaveCoins();
            InputManager.Instance.UpdateUI();
            return;
        }

        if (_coinsDouble - ammount < 0)
        {
            _coinsDouble = 0;
            SaveCoins();
            InputManager.Instance.UpdateUI();
            return;
        }

        _coinsDouble -= ammount;

        SaveCoins();
        InputManager.Instance.UpdateUI();
    }

    public void AddGems(ulong ammount)
    {
        if (Gems + ammount > ulong.MaxValue)
        {
            Gems = ulong.MaxValue;
            SaveGems();
            InputManager.Instance.UpdateUI();
            return;
        }

        if (Gems + ammount < 0)
        {
            Gems = 0;
            SaveGems();
            InputManager.Instance.UpdateUI();
            return;
        }

        Gems += ammount;

        SaveGems();
        InputManager.Instance.UpdateUI();
    }

    public bool TrySpawnPortal(Room sender)
    {
        for (int i = 0; i < LevelGanerator.Instance.rooms.Length; i++)
        {
            if (LevelGanerator.Instance.rooms[i].enabled && LevelGanerator.Instance.rooms[i] != sender) { return false; }
        }

        _portalInstance = Instantiate(_portal, sender.transform.position, Quaternion.identity);
        return true;
    }

    public void MoveToNextLevel()
    {
        StartCoroutine(NextLevel());
    }
}
