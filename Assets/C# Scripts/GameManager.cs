using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Mike;
using System.Collections;

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


    public GameObject coin;

    [SerializeField] private GameObject portal;

    [SerializeField] private float difficultyPreRoomMultiplier;
    [SerializeField] private float difficultyPreLevel;

    private float coinsF;

    //----------------------------------------------


    public ulong Coins { get; private set; }
    public ulong Gems { get; private set; }
    public int Level { get; private set; }
    public float DifficultyPreRoomMultiplier { get; private set; }
    public float DifficultyPreLevel { get; private set; }


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


    void Initialize()
    {

        DifficultyPreLevel = difficultyPreLevel;
        DifficultyPreRoomMultiplier = difficultyPreRoomMultiplier;

        Level = PlayerPrefs.GetInt("Current Level");

        Gems = ulong.Parse(PlayerPrefs.GetString("Gems", "0"));
        Coins = ulong.Parse(PlayerPrefs.GetString("Coins", "0"));
        coinsF = Coins;//bruh I'm retarded ;)
    }

    void SaveCoins()
    {
        PlayerPrefs.SetString("Coins", Coins.ToString("G20"));
    }
    
    void SaveGems()
    {
        PlayerPrefs.SetString("Gems", Gems.ToString("G20"));
    }

    public void AddCoins(ulong ammount)
    {
        if (Coins + ammount > ulong.MaxValue)
        {
            Coins = ulong.MaxValue;
            SaveCoins();
            InputManager.Instance.UpdateUI();
            return;
        }

        if (Coins + ammount < 0)
        {
            Coins = 0;
            SaveCoins();
            InputManager.Instance.UpdateUI();
            return;
        }

        Coins += ammount;

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

    IEnumerator NextLevel()
    {
        yield return new WaitForSeconds(2f);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = Vector3.zero;
        if (player.GetComponent<Dash>().currentDash != null) { StopCoroutine(player.GetComponent<Dash>().currentDash); }

        PlayerPrefs.SetInt("Current Level", PlayerPrefs.GetInt("Current Level", 1) + 1);

        LevelGanerator.Instance.RegenerateLevel();

        //////////// AHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHH...
        /////ICANT CONCENTRATE!
        /////////MY HEAD HURTS!!
        ////// CLASS TOO LOUD!!!
    }
}
