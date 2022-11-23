using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A static class for storing data. WARNING: All data is stored in PlayerPrefs!
/// </summary>
public static class StorageManager
{
    /// <summary>
    /// Used by <see cref="ResetProgress()"/> to save non resetable data <seealso cref="_nonResetableValues"/>
    /// </summary>
    struct Pref
    {
        public enum PrefType
        {
            String,
            Int,
            Float
        }

        public string Key;
        public PrefType ValueType;
        public string Value;

        public Pref(string key, PrefType valueType, string value)
        {
            Key = key;
            ValueType = valueType;
            Value = value;
        }
    }


    //---------------

    /// <summary>
    /// Game values are nested in this class for convenience
    /// </summary>
    public static class Game
    {
        public const string StartingLevelSavekey = "Starting Level";
        public const string EarlyAccessPlayerSavekey = "Early Access";
        public const string EarlyAccessRewardCollectedSavekey = "Early Access Collect";

        public static int StartingLevel { get => PlayerPrefs.GetInt(StartingLevelSavekey, 1); set => PlayerPrefs.SetInt(StartingLevelSavekey, value); }
        public static bool EarlyAccessPlayer { get => PlayerPrefs.GetInt(EarlyAccessPlayerSavekey, 0) == 1; set => PlayerPrefs.SetInt(EarlyAccessPlayerSavekey, value ? 1 : 0); } 
        public static bool EarlyAccessRewardCollected { get => PlayerPrefs.GetInt(EarlyAccessRewardCollectedSavekey, 0) == 1; set => PlayerPrefs.SetInt(EarlyAccessRewardCollectedSavekey, value ? 1 : 0); }
    }

    /// <summary>
    /// Setting values are nested in this class for convenience
    /// </summary>
    public static class Settings
    {
        /// <summary>
        /// ControlSetting values are nested in this class for convenience
        /// </summary>
        public static class Controls
        {
            public const string HapticsSavekey = "Haptic Feedback";
            public const string InvertDashSavekey = "Invert Dash";


            static bool? _invertDash = null;
            /// <summary>
            /// Determines if dashing should be inverted. This value is cached
            /// </summary>
            public static bool InvertDash { get { _invertDash ??= PlayerPrefs.GetInt(InvertDashSavekey, 0) == 1; return _invertDash.Value; } set { _invertDash = value; PlayerPrefs.SetInt(InvertDashSavekey, InvertDash ? 1 : 0); } }

            static bool? _hapticFeedback = null;
            /// <summary>
            /// Determines if hapticfeedback should be executed. This value is cached
            /// </summary>
            public static bool HapticFeedback { get { _hapticFeedback ??= PlayerPrefs.GetInt(HapticsSavekey, 1) == 1; return _hapticFeedback.Value; } set { _hapticFeedback = value; PlayerPrefs.SetInt(HapticsSavekey, HapticFeedback ? 1 : 0); } }
        }

        /// <summary>
        /// VideoSetting values are nested in this class for convenience
        /// </summary>
        public static class Graphics
        {
            public const string BloomIntensitySavekey = "Bloom Intensity";
            public const string BloomSkipedIterationsSavekey = "Bloom Skip Iterations";
            public const string BloomScatterSaveKey = "Bloom Scatter";
            public const string BloomHighQualityFilterSaveKey = "Bloom Filter Quality";

            public static float BloomIntensity { get => PlayerPrefs.GetFloat(BloomIntensitySavekey, 1); set => PlayerPrefs.SetFloat(BloomIntensitySavekey, value); }
            public static int BloomSkipIterations { get => PlayerPrefs.GetInt(BloomSkipedIterationsSavekey, 5); set => PlayerPrefs.SetInt(BloomSkipedIterationsSavekey, value); }
            public static float BloomScatter { get => PlayerPrefs.GetFloat(BloomScatterSaveKey, .4f); set => PlayerPrefs.SetFloat(BloomScatterSaveKey, value); }
            public static bool BloomHighQualityFilter { get => PlayerPrefs.GetInt(BloomHighQualityFilterSaveKey, 0) == 1; set => PlayerPrefs.SetInt(BloomHighQualityFilterSaveKey, value ? 1 : 0); }
        }

        /// <summary>
        /// Miscellaneous values are nested in this class for convenience
        /// </summary>
        public static class Misc
        {
            public const string SupportAdsSavekey = "Support Ads";

            static bool? _supportAds = null;
            public static bool SupportAds { get { if (_supportAds == null) { _supportAds = PlayerPrefs.GetInt(SupportAdsSavekey, 0) == 1; } return _supportAds.Value; } set { _supportAds = value; PlayerPrefs.SetInt(SupportAdsSavekey, value ? 1 : 0); } }
        }

        public static void SaveOption<T>(string key, T value)
        {
            if(value == null) { return; }

            if (typeof(T) == typeof(float))
            {
                PlayerPrefs.SetFloat(key, (value as float?).Value);
            }
            else if (typeof(T) == typeof(int))
            {
                PlayerPrefs.SetInt(key, (value as int?).Value);
            }
            else if (typeof(T) == typeof(bool))
            {
                PlayerPrefs.SetInt(key, (value as bool?).Value ? 1 : 0);
            }
            else if (typeof(T) == typeof(string))
            {
                PlayerPrefs.SetString(key, value as string);
            }
        }
    }

    //---------------------------


    #region ResetProgress
    /// <summary>
    /// Used to temporarly store data when executing <see cref="ResetProgress()"/>
    /// </summary>
    static List<Pref> _nonResetableValues = new();

    // This is kinda stupid. I should propably make my own library to handle saving stuff :/
    /// <summary>
    /// Deletes all saved data with exceptions (e.g. EarlyAccessPlayer)
    /// </summary>
    public static void ResetProgress()
    {
        SaveValues
        (
            new Pref(Game.EarlyAccessPlayerSavekey, Pref.PrefType.Int, (Game.EarlyAccessPlayer ? 1 : 0).ToString())
        );

        PlayerPrefs.DeleteAll();

        LoadSavedValues();
    }

    /// <summary>
    /// Saves values to <see cref="_nonResetableValues"/> for them to be loaded in <see cref="LoadSavedValues()"/>
    /// </summary>
    /// <param name="values"></param>
    static void SaveValues(params Pref[] values)
    {
        foreach (Pref value in values)
        {
            _nonResetableValues.Add(value);
        }
    }

    /// <summary>
    /// Saves values to player prefs which were in <see cref="_nonResetableValues"/>. These values are put there by <see cref="SaveValues(Pref[])"/>
    /// </summary>
    static void LoadSavedValues()
    {
        foreach (Pref value in _nonResetableValues)
        {
            if (value.ValueType == Pref.PrefType.Int)
            {
                PlayerPrefs.SetInt(value.Key, int.Parse(value.Value));
            }
            else if (value.ValueType == Pref.PrefType.Float)
            {
                PlayerPrefs.SetFloat(value.Key, float.Parse(value.Value));
            }
            else if (value.ValueType == Pref.PrefType.String)
            {
                PlayerPrefs.SetString(value.Key, value.Value);
            }
        }
    }
    #endregion
}
