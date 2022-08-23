using UnityEngine;

public class RunUpgrades : MonoBehaviour
{
    [System.Serializable]
    public struct RunUpgrade
    {
        public string name;
        public GameObject spawnObject;
    }


    //----------------------------


    public RunUpgrade[] currentRunUpgrades;


    //----------------------------


    private bool slowMoInitialized = false;


    //----------------------------


    private void Start()
    {
        if (GameObject.Find("Temp") != null)
        {
            currentRunUpgrades = GameObject.Find("Temp").GetComponent<RunUpgrades>().currentRunUpgrades;
        }
    }

    private void Update()
    {
        RunRunUpgrades();
    }


    //----------------------------

    void RunRunUpgrades()
    {
        foreach (RunUpgrade item in currentRunUpgrades)
        {
            if (item.name != "Explode" && item.name != "ChainLightning")
            {
                Invoke(item.name, 0);
            }
        }
    }

    public void AddUpgrade(RunUpgrade upgrade)//A simple Append function for the "RunUpgrade" variable Type
    {
        for (int i = 0; i < currentRunUpgrades.Length; i++)
        {
            if (currentRunUpgrades[i].name == upgrade.name)
            {
                return;
            }
        }

        currentRunUpgrades = Mike.MikeArray.Append(currentRunUpgrades, upgrade);
    }

    float missileTimer = 0;
    public void Missile()
    {
        if (missileTimer < 1) { missileTimer += Time.deltaTime; return; }//makes the method run at a certain interval
        else { missileTimer = 0; }//reset timer
        if (Random.Range(0f, 1f) < 0.75f) { return; }//roll dice
        if (GameObject.FindGameObjectWithTag("Enemy") == null) { return; }//check if any enemy alive

        Missile missile = Instantiate(GetRunUpgrade("Missile").spawnObject, transform.position, Quaternion.identity).GetComponent<Missile>();//spawn missile

        missile.target = GetClosestTargetWithTag2D(missile.transform.position, "Enemy");//set missile target

        //BRUH! I'M SUPER TIRED. I SLEPT FOR ONLY 3H! SO IDK WHAT I'M DOING.
    }

    float bombardmentTimer = 0;
    public void Bombardment()
    {
        if (bombardmentTimer < 1) { bombardmentTimer += Time.deltaTime; return; }//makes the method run at a certain interval
        else { bombardmentTimer = 0; }//reset timer
        if (Random.Range(0f, 1f) < 0.75f) { return; }//roll dice
        if (GameObject.FindGameObjectWithTag("Enemy") == null) { return; }//check if any enemy alive

        GameObject bombPrefab = GetRunUpgrade("Bombardment").spawnObject;
        Vector2 currentRoom = GetClosestTargetWithTag2D(transform.position, "Room").position;

        for (int i = 0; i < 25; i++)
        {
            Bomb bomb = Instantiate(bombPrefab, transform.position, Quaternion.identity).GetComponent<Bomb>();//spawn bomb

            bomb.targetPosition = currentRoom + Mike.MikeRandom.RandomVector2(-20, 20, -20, 20);
        }
    }

    public void Explode()
    {
        if (GetRunUpgrade("Explode").name != null) Instantiate(GetRunUpgrade("Explode").spawnObject, transform.position, Quaternion.identity);
    }
    
    public void ChainLightning()
    {
        if (GetRunUpgrade("ChainLightning").name != null) Instantiate(GetRunUpgrade("ChainLightning").spawnObject, transform.position, Quaternion.identity);
    }

    public void SlowMo()
    {
        if (slowMoInitialized) return;
        slowMoInitialized = true;
        GetComponent<Dash>().slowMoUpgradeEnabled = true;
    }

    /// <summary>
    /// Loops through all runUpgrades and returns the first variable that matches the name param
    /// </summary>
    /// <param name="name"></param>
    /// <returns>Returns RunUpgrade by string</returns>
    RunUpgrade GetRunUpgrade(string name)
    {
        foreach (RunUpgrade item in currentRunUpgrades)
        {
            if (item.name == name) { return item; }
        }

        return new RunUpgrade();
    }

    /// <summary>
    /// Loops through all GameObjects with the tag of the param "tag" and compares their distances
    /// </summary>
    /// <param name="currentPosition"></param>
    /// <param name="tag"></param>
    /// <returns>Closest Transform with the "tag" tag relative to the "currentPosition" parameter</returns>
    public static Transform GetClosestTargetWithTag2D(Vector2 currentPosition, string tag)
    {
        Transform best = null;
        GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);

        foreach (GameObject item in targets)
        {
            if(best == null) { best = item.transform; }
            else if(Vector2.Distance(currentPosition, best.position) > Vector2.Distance(currentPosition, item.transform.position)) { best = item.transform; }
        }

        return best;
    }
}
