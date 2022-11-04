using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Mike;
using System.Collections;
using Unity.Mathematics;

public class GameManager : MonoBehaviour
{
    #region Instance
    static GameManager _Instance;
    public static GameManager Insatnce
    {
        get { return _Instance; }
    }
    #endregion


    //----------------------------------------------

    // I'm unsure if i should use BigInt or double/ulong instead of int (I'm lazy and dont want to refactor more code)

    public GameObject coin;

    [SerializeField] private GameObject portal;
    private GameObject portalInstance;

    [SerializeField] private float difficultyPreRoomMultiplier;
    [SerializeField] private float difficultyPreLevelMultiplier;
    [SerializeField] private float difficultyPreLevelOffset;

    private double coinsDouble;


    //----------------------------------------------

    /// <summary>
    /// Returns 
    /// </summary>
    public long Coins { get => (long) coinsDouble; }
    public ulong Gems { get; private set; }
    public int Level { get; private set; }
    public float DifficultyPreRoomMultiplier { get => difficultyPreRoomMultiplier; }
    public float DifficultyPreLevelMultiplier { get => difficultyPreLevelMultiplier; }
    public float DifficultyPreLevelOffset { get => difficultyPreLevelOffset; }
    public float Difficulty { get => DifficultyPreRoomMultiplier + DifficultyPreLevelOffset + Level * DifficultyPreLevelMultiplier; }

    public GameObject player;
    public GameObject Player { get { if (player == null) { player = GameObject.FindGameObjectWithTag("Player"); } return player; } }


    //----------------------------------------------


    private void Awake()
    {
        _Instance = this;

        Application.targetFrameRate = 9999;

        Initialize();
    }

    private void Start()
    {
        InputManager.Instance.UpdateUI();
        if(LevelGanerator.Instance != null) { LevelGanerator.Instance.GenerateLevel(); }
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
    void Initialize()
    {
        Level = StorageManager.StartingLevel;

        Gems = ulong.Parse(PlayerPrefs.GetString("Gems", "0"));
        coinsDouble = double.Parse(PlayerPrefs.GetString("Coins", "0"));
        coinsDouble = Coins;
    }

    /// <summary>
    /// Saves Coins
    /// </summary>
    void SaveCoins()
    {
        PlayerPrefs.SetString("Coins", math.round(coinsDouble).ToString("G20"));
    }

    /// <summary>
    /// Saves Gems
    /// </summary>
    void SaveGems()
    {
        PlayerPrefs.SetString("Gems", Gems.ToString("G20"));
    }

    /// <summary>
    /// handles level transition and acounts for all variables involved
    /// </summary>
    IEnumerator NextLevel()
    {
        EZCameraShake.CameraShaker.Instance.ShakeOnce(15, 25, 4f, 1f);
        Player.GetComponent<Dash>().enabled = false;
        yield return new WaitForSeconds(2f);

        Player.transform.position = Vector3.zero;
        if (Player.GetComponent<Dash>().currentDash != null) { StopCoroutine(Player.GetComponent<Dash>().currentDash); }

        Level++;
        InputManager.Instance.UpdateUI();
        Destroy(portalInstance);
        LevelGanerator.Instance.RegenerateLevel();
        Player.GetComponent<Dash>().enabled = true;

    }
    #endregion

    //----------------------------------------------


    public void SetCoins(double ammount)
    {
        coinsDouble = ammount;

        SaveCoins();
        InputManager.Instance.UpdateUI();
    }
    
    public void AddCoins(double ammount)
    {
        // NOTE: double "-" operator returns a double and (double.MaxValue + 1 < 0) as it overloads the variable!
        if (coinsDouble + ammount > double.MaxValue)
        {
            coinsDouble = 0;
            SaveCoins();
            InputManager.Instance.UpdateUI();
            return;
        }

        if (coinsDouble + ammount < 0)
        {
            coinsDouble = double.MaxValue;
            SaveCoins();
            InputManager.Instance.UpdateUI();
            return;
        }

        coinsDouble += ammount;

        SaveCoins();
        InputManager.Instance.UpdateUI();
    }
    
    public void RemoveCoins(double ammount)
    {
        // NOTE: double "-" operator returns a double and (double.MaxValue + 1 < 0) as it overloads the variable!
        if (coinsDouble - ammount > double.MaxValue)
        {
            coinsDouble = double.MaxValue;
            SaveCoins();
            InputManager.Instance.UpdateUI();
            return;
        }

        if (coinsDouble - ammount < 0)
        {
            coinsDouble = 0;
            SaveCoins();
            InputManager.Instance.UpdateUI();
            return;
        }

        coinsDouble -= ammount;

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

        portalInstance = Instantiate(portal, sender.transform.position, Quaternion.identity);
        return true;
    }

    public void MoveToNextLevel()
    {
        StartCoroutine(NextLevel());
    }
}
