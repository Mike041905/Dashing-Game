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

    [SerializeField] private float difficultyPreRoomMultiplier;
    [SerializeField] private float difficultyPreLevel;

    private double coinsDouble;

    
    //----------------------------------------------


    public ulong Coins { get; private set; }
    public ulong Gems { get; private set; }
    public int Level { get; private set; }
    public float DifficultyPreRoomMultiplier { get; private set; }
    public float DifficultyPreLevel { get; private set; }
    public float Difficulty { get => GameManager.Insatnce.DifficultyPreRoomMultiplier + Level * GameManager.Insatnce.DifficultyPreLevel; }


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
        DifficultyPreLevel = difficultyPreLevel;
        DifficultyPreRoomMultiplier = difficultyPreRoomMultiplier;

        Level = StorageManager.StartingLevel;

        Gems = ulong.Parse(PlayerPrefs.GetString("Gems", "0"));
        Coins = ulong.Parse(PlayerPrefs.GetString("Coins", "0"));
        coinsDouble = Coins;
    }

    /// <summary>
    /// Saves Coins
    /// </summary>
    void SaveCoins()
    {
        PlayerPrefs.SetString("Coins", math.round((double) coinsDouble).ToString("G20"));
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
        yield return new WaitForSeconds(2f);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = Vector3.zero;
        if (player.GetComponent<Dash>().currentDash != null) { StopCoroutine(player.GetComponent<Dash>().currentDash); }

        Level++;

        LevelGanerator.Instance.RegenerateLevel();

        //////////// AHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHH...
        /////ICANT CONCENTRATE!
        /////////MY HEAD HURTS!!
        ////// CLASS TOO LOUD!!!
    }
    #endregion

    //----------------------------------------------


    public void AddCoins(double ammount)
    {
        if (coinsDouble + ammount > double.MaxValue)
        {
            coinsDouble = double.MaxValue;
            SaveCoins();
            InputManager.Instance.UpdateUI();
            return;
        }

        if (coinsDouble + ammount < 0)
        {
            Coins = 0;
            SaveCoins();
            InputManager.Instance.UpdateUI();
            return;
        }

        coinsDouble += ammount;

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

    public void SpawnPortal(Room sender)
    {
        Instantiate(portal, sender.transform.position, Quaternion.identity);
    }

    public void MoveToNextLevel()
    {
        StartCoroutine(NextLevel());
    }
}
