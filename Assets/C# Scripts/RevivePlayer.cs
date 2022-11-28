using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RevivePlayer : MonoBehaviour
{
    public void Revive()
    {
        // BRUH! IT TOOK ME AN HOUR TO FIGURE THIS OUT! HELP!
        Player.Instance.PlayerHealth.Revive(true, e => Player.Instance.PlayerDash.OnStartDash += e, e => Player.Instance.PlayerDash.OnStartDash -= e);
    }
}
