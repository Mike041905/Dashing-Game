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
        if(Insatnce != null && SceneManager.GetActiveScene().buildIndex != 0) { return; }

        _Instance = this;

        Application.targetFrameRate = Screen.currentResolution.refreshRate;
        SceneManager.activeSceneChanged += OnSceneChange;

        Initialize();
        UpdateUI();

        if(SceneManager.GetActiveScene().buildIndex == 0 && GameObject.FindGameObjectWithTag("Player") != null)
        {
            Destroy(GameObject.FindGameObjectWithTag("Player"));
        }
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
        StartCoroutine(NextLevel());
    }

    IEnumerator NextLevel()
    {
        yield return new WaitForSeconds(2f);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = Vector3.zero;
        if (player.GetComponent<Dash>().currentDash != null) { StopCoroutine(player.GetComponent<Dash>().currentDash); }

        PlayerPrefs.SetInt("Current Level", PlayerPrefs.GetInt("Current Level", 1) + 1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnSceneChange(Scene s, Scene s2)
    {
        Initialize();
        UpdateUI();
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChange;
    }
}
