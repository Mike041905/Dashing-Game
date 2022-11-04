using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistenceManager : MonoBehaviour
{
    [SerializeField] bool includeCilldern = false;
    [SerializeField] GameObject[] persistingObjects = new GameObject[0];
    [SerializeField] string[] persistingObjectsWithTags = new string[0];


    static PersistenceManager _instance;
    public static PersistenceManager Instance { get => _instance; }

    private void Awake()
    {
        _instance = this;
    }

    public void WipeLevel()
    {
        GameObject[] objects = FindObjectsOfType<GameObject>(true);

        for (int i = 0; i < objects.Length; i++)
        {
            if (CompareTags(objects[i], persistingObjectsWithTags)) { continue; }
            if (CompareObjects(objects[i], persistingObjects)) { continue; }
            if (!includeCilldern && objects[i].transform.parent != null) { continue; }

            Destroy(objects[i]);
        }
    }

    static bool CompareObjects(GameObject go, GameObject[] objects)
    {
        foreach (GameObject compared in objects)
        {
            if (go == compared) { return true; }
        }

        return false;
    }
    static bool CompareTags(GameObject go, string[] tags)
    {
        foreach (string tag in tags)
        {
            if (go.CompareTag(tag)) { return true; }
        }

        return false;
    }
}
