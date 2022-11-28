using UnityEngine;
using Mike;

public class PowerUpAdder : MonoBehaviour
{
    public static PowerUp[] PowerUps { get; private set; } = new PowerUp[0];

    static PowerUpAdder _instance;
    public static PowerUpAdder Instance { get => _instance;  }

    private void Awake()
    {
        if(Instance != null) { return; }

        PowerUps = new PowerUp[0];
        _instance = this;
    }

    public PowerUp GetPowerUp(string name)
    {
        for (int i = 0; i < PowerUps.Length; i++)
        {
            if (PowerUps[i].powerUpName == name) { return PowerUps[i]; }
        }

        return null;
    }
    
    public bool TryGetPowerUp(string name, out PowerUp powerUp)
    {
        for (int i = 0; i < PowerUps.Length; i++)
        {
            if (PowerUps[i].powerUpName == name) { powerUp = PowerUps[i]; return true; }
        }

        powerUp = null;
        return false;
    }

    public PowerUp AddOrUpgradePowerUp(GameObject newPowerUp)
    {
        PowerUp powerUp = GetPowerUp(newPowerUp.GetComponent<PowerUp>().powerUpName);
        if(powerUp == null) // Add
        {
            PowerUps = MikeArray.Append(PowerUps, Instantiate(newPowerUp, transform).GetComponent<PowerUp>());
        }
        else // Upgrade
        {
            powerUp.Upgrade();
        }

        return powerUp != null ? powerUp : newPowerUp.GetComponent<PowerUp>();
    }

    public bool HasPowerUp(string name)
    {
        foreach (PowerUp powerUp in PowerUps)
        {
            if(powerUp.powerUpName == name) { return true; }
        }

        return false;
    }
}
