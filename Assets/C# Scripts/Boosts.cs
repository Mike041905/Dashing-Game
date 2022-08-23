using UnityEngine;

public class Boosts : MonoBehaviour
{
    [System.Serializable]
    public struct Boost
    {
        public string name;
        public float duration;
        
        public Boost(string name, float duration)
        {
            this.name = name;
            this.duration = duration;
        }
    }

    public Boost[] activeBoosts = new Boost[0];

    static Boosts _Instance;

    public static Boosts Instance 
    {   
        get
        {
            return _Instance;
        } 
    }


    //--------------------------------------

    private void Awake()
    {
        _Instance = this;
    }

    private void Update()
    {
        RunBoosts();
    }

    private void Start()
    {
        LoadActiveBoosts();
    }


    //Boosts -------------------------------


    bool doubleCoinsBoostRan = false;
    void DoubleCoins()
    {
        if (!doubleCoinsBoostRan) { doubleCoinsBoostRan = true; }
        else { return; }

        PlayerPrefs.SetFloat("Coin Boost", 2);
    }

    void DeactivateDoubleCoins()
    {
        doubleCoinsBoostRan = false;

        PlayerPrefs.SetFloat("Coin Boost", 1);
    }


    //--------------------------------


    public bool HasBoost(string name)
    {
        foreach (Boost item in activeBoosts)
        {
            if(item.name == name) { return true; }
        }

        return false;
    }
    
    public Boost GetBoost(string name)
    {
        foreach (Boost item in activeBoosts)
        {
            if(item.name == name) { return item; }
        }

        return new Boost();
    }

    public void AddBoost(Boost boost)
    {
        Boost[] temp = activeBoosts;
        activeBoosts = new Boost[activeBoosts.Length + 1];
        temp.CopyTo(activeBoosts, 0);
        activeBoosts[activeBoosts.Length - 1] = boost;
    }

    public void RemoveBoost(string boostName)
    {
        if (activeBoosts.Length == 1) { activeBoosts = new Boost[0]; return; }
        if (activeBoosts.Length == 0) { return; }

        bool start = false;
        Boost[] temp = new Boost[activeBoosts.Length - 1];

        for (int i = 0; i < activeBoosts.Length; i++)
        {
            if (!start && activeBoosts[i].name == boostName) { start = true; }
            else if (start) { activeBoosts[i - 1] = activeBoosts[i]; }

            if (i > 0) temp[i - 1] = activeBoosts[i - 1];
        }

        activeBoosts = temp;
    }

    void RunBoosts()
    {
        for (int i = 0; i < activeBoosts.Length; i++)
        {
            Invoke(activeBoosts[i].name, 0);
            activeBoosts[i].duration -= Time.unscaledDeltaTime;
            if (activeBoosts[i].duration <= 0)
            {
                RemoveBoost(activeBoosts[i].name);
                Invoke("Deactivate" + activeBoosts[i].name, 0);
            }
        }

        SaveActiveBoosts();
    }

    void SaveActiveBoosts()
    {
        PlayerPrefs.DeleteKey("Active Boost Names");
        PlayerPrefs.DeleteKey("Active Boost Durations");

        foreach (Boost boost in activeBoosts)
        {
            PlayerPrefs.SetString("Active Boost Names", PlayerPrefs.GetString("Active Boost Names", "") + boost.name + ",");
            PlayerPrefs.SetString("Active Boost Durations", PlayerPrefs.GetString("Active Boost Duration", "") + boost.duration + ",");
        }
    }

    void LoadActiveBoosts()
    {
        if(PlayerPrefs.GetString("Active Boost Names", "") == "") { return; }
        string boostNames = PlayerPrefs.GetString("Active Boost Names", "");
        string boostDurations = PlayerPrefs.GetString("Active Boost Durations", "");

        for (int i = 0; i < 9999; i++)
        {
            if(boostNames == "") { break; }
            AddBoost(new Boost(boostNames.Remove(boostNames.IndexOf(',')), float.Parse(boostDurations.Remove(boostDurations.IndexOf(',')))));

            boostNames = boostNames.Remove(0, boostNames.IndexOf(',') + 1);
            boostDurations = boostDurations.Remove(0, boostDurations.IndexOf(',') + 1);
        }
    }
}
