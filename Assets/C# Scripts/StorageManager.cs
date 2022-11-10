using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StorageManager
{
    const string STARTING_LEVEL_SAVEKEY = "Starting Level";
    const string HAPTICS_SAVEKEY = "Starting Level";
    public static int StartingLevel { get => (int) Upgrade.GetUpgrade(STARTING_LEVEL_SAVEKEY, UpgradeData.VariableType.Integer); }
    
    static bool? hapticFeedback = null;
    public static bool HapticFeedback { get { hapticFeedback ??= PlayerPrefs.GetInt(HAPTICS_SAVEKEY, 1) == 1; return hapticFeedback.Value; } set { hapticFeedback = value; PlayerPrefs.SetInt(HAPTICS_SAVEKEY, HapticFeedback ? 1 : 0); } }
}
