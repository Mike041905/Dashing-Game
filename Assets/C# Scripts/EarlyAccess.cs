using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarlyAccess : MonoBehaviour
{
    [SerializeField] int _earlyAccessGemReward = 500;

    private void Start()
    {
        if (int.Parse(Application.version.Split(".")[0]) >= 1) // IsReleased
        {
            if (StorageManager.Game.EarlyAccessPlayer && !StorageManager.Game.EarlyAccessRewardCollected)
            {
                GameManager.Insatnce.AddGems((ulong)_earlyAccessGemReward);
                StorageManager.Game.EarlyAccessRewardCollected = true;
            }
        }
        else // EarlyAccess
        {
            if (!StorageManager.Game.EarlyAccessPlayer)
            {
                StorageManager.Game.EarlyAccessPlayer = true;
                StorageManager.Game.EarlyAccessRewardCollected = false;
            }
        }
    }
}
