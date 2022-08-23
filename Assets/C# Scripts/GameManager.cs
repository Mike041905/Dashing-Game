using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Mike;

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

    [SerializeField] private TextMeshProUGUI coinConter;
    [SerializeField] private TextMeshProUGUI gemConter;
    [SerializeField] private TextMeshProUGUI scoreCounter;
    [SerializeField] private TextMeshProUGUI levelCounter;

    [SerializeField] private float difficultyPreRoomMultiplier;
    [SerializeField] private float difficultyPreLevel;

    private float coinsF;

    //----------------------------------------------


    public int Coins { get; private set; }
    public int Gems { get; private set; }
    public int Level { get; private set; }
    public float DifficultyPreRoomMultiplier { get; private set; }
    public float DifficultyPreLevel { get; private set; }


    //----------------------------------------------
    /* Notes:
     int32 can hold a number up to about: 2 bilion
     */
    //----------------------------------------------


    private void Awake()
    {
        _Instance = this;

        Initialize();
        UpdateUI();
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

        Gems = PlayerPrefs.GetInt("Gems", 0);
        Coins = PlayerPrefs.GetInt("Coins", 0);
        coinsF = Coins;//bruh I'm retarded ;)
    }

    void SaveCoins()
    {
        PlayerPrefs.SetInt("Coins", Coins);
    }
    
    void SaveGems()
    {
        PlayerPrefs.SetInt("Gems", Gems);
    }

    public void AddCoins(int ammount)
    {
        if (Coins + ammount > 2000000000)
        {
            Coins = 2000000000;
            SaveCoins();
            UpdateUI();
            return;
        }

        if (Coins + ammount < 0)
        {
            Coins = 0;
            SaveCoins();
            UpdateUI();
            return;
        }

        Coins += ammount;

        SaveCoins();
        UpdateUI();
    }

    public void AddGems(int ammount)
    {
        Gems += ammount;
        SaveGems();
        UpdateUI();
    }

    void UpdateUI()
    {
        if (coinConter != null) coinConter.text = MikeString.ConvertNumberToString(Coins);
        if (gemConter != null) gemConter.text = MikeString.ConvertNumberToString(Gems);
        if (levelCounter != null) levelCounter.text = "Level: " + Level.ToString();
    }

    public void MoveToNextLevelIfEligible(Room sender)
    {
        //insert code in "MoveToNextLevel()" mothode NOT here!

        foreach (Room room in LevelGanerator.Instance.rooms)
        {
            if (room != sender && room.enabled) { return; }
        }

        MoveToNextLevel();
    }

    void MoveToNextLevel()
    {
        GameObject go = Instantiate(new GameObject());

        go.name = "Temp";
        go.AddComponent<MikeDestroy>();
        RunUpgrades temp = go.AddComponent<RunUpgrades>();
        temp.currentRunUpgrades = GameObject.FindGameObjectWithTag("Player").GetComponent<RunUpgrades>().currentRunUpgrades;
        DontDestroyOnLoad(go);

        PlayerPrefs.SetInt("Current Level", PlayerPrefs.GetInt("Current Level", 1) + 1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
