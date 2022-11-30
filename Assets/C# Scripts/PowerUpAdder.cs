using UnityEngine;
using Mike;
using System.Collections.Generic;

public class PowerUpAdder : MonoBehaviour
{
    public static List<PowerUp> PowerUps { get; private set; } = new();

    static PowerUpAdder _instance;
    public static PowerUpAdder Instance { get => _instance;  }

    private void Awake()
    {
        _instance = this;
    }

    public PowerUp GetPowerUp(string name)
    {
        for (int i = 0; i < PowerUps.Count; i++)
        {
            if (PowerUps[i].powerUpName == name) { return PowerUps[i]; }
        }

        return null;
    }
    
    public bool TryGetPowerUp(string name, out PowerUp powerUp)
    {
        for (int i = 0; i < PowerUps.Count; i++)
        {
            if (PowerUps[i].powerUpName == name) { powerUp = PowerUps[i]; return true; }
        }

        powerUp = null;
        return false;
    }

    public PowerUp AddOrUpgradePowerUp(GameObject newPowerUp) => AddOrUpgradePowerUp(newPowerUp.GetComponent<PowerUp>());
    public PowerUp AddOrUpgradePowerUp(PowerUp newPowerUp)
    {
        if (!TryGetPowerUp(newPowerUp.powerUpName, out PowerUp powerUp)) // Add
        {
            PowerUps.Add(Instantiate(newPowerUp, transform)); ;
        }
        else // Upgrade
        {
            powerUp.UpgradePowerUp();
        }

        return powerUp != null ? powerUp : newPowerUp;
    }

    public void RemovePowerUp(PowerUp powerUp)
    {
        PowerUps.Remove(powerUp);

        Destroy(powerUp);
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
