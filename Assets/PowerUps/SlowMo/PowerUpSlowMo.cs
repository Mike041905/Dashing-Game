using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSlowMo : PowerUp
{
    void Start()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<Dash>().OnAiming += SlowMotion;
    }

    void SlowMotion()
    {
        Time.timeScale = GetStat("SlowMoTime").statValue;
    }
}
