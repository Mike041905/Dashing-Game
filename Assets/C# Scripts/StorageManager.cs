using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StorageManager
{
    const string STARTING_LEVEL_SAVEKEY = "Starting Level";
    public static int StartingLevel { get => PlayerPrefs.GetInt(STARTING_LEVEL_SAVEKEY); set => PlayerPrefs.SetInt(STARTING_LEVEL_SAVEKEY, value); }
}
