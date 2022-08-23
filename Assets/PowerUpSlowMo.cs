using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSlowMo : PowerUp
{
    public float slowMoTime = .5f;
    public float slowMoUpgradeValue = 0.1f;

    void Start()
    {
        PowerUpSlowMo[] slowMos = transform.parent.GetComponentsInChildren<PowerUpSlowMo>();
        if (slowMos.Length > 0)
        {
            foreach (PowerUpSlowMo slowMo in slowMos)
            {
                slowMo.slowMoTime *= 1 - slowMoUpgradeValue;
            }

            Destroy(gameObject);
        }
        else
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<Dash>().OnAiming += SlowMotion;
        }
    }

    void SlowMotion()
    {
        Time.timeScale = slowMoTime;
    }
}
