using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMikeParsable
{
    public abstract T Parse<T>(string value);
}

public static class MikeSave
{
    /// <summary>
    /// Saves data under key using PlayerPrefs.SetString().
    /// || IMPORTANT: This method uses the ToString() method meaning the object from which its called should have the ToString() method ovverrider!
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ths"></param>
    /// <param name="key"></param>
    public static void Save<T>(this T ths, string key) => PlayerPrefs.SetString(key, ths.ToString());
    /// <summary>
    /// Loads data to the variable under key using PlayerPrefs.GetString() and parses it || IMPORTANT: <typeparamref name="T"/> must implement the IMikeParsable interface.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ths"></param>
    /// <param name="key"></param>
    public static void Load<T>(this T ths, string key) where T : IMikeParsable => ths.Parse<T>(PlayerPrefs.GetString(key));
}
