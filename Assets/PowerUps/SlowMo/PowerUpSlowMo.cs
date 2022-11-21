using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSlowMo : PowerUp
{
    void Start()
    {
        Player.Instance.PlayerDash.OnAiming += SlowMotion;
    }

    void SlowMotion()
    {
        if(Player.Instance.PlayerHealth.Dead) { Time.timeScale = 1; return; }
        Time.timeScale = GetStat("SlowMoTime").statValue;
    }
}
