using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StorageManager
{
    const string STARTING_LEVEL_SAVEKEY = "Starting Level";
    public static int StartingLevel { get => (int) Upgrade.GetUpgrade(STARTING_LEVEL_SAVEKEY, UpgradeData.VariableType.Integer); }
}
