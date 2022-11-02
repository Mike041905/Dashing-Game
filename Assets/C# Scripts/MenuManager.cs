using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject upgrades;


    static MenuManager _instance;
    public static MenuManager Instance { get => _instance; }

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        InitializeUpgradeVariables();
    }

    public void SetCurrentLevel()
    {
        PlayerPrefs.SetInt("Current Level", PlayerPrefs.GetInt("Starting Level", 1));
    }

    void InitializeUpgradeVariables()
    {
        if (upgrades != null) SendMessageToAllDescendants(upgrades.transform, "InitializeVariable");
    }

    public void SendMessageToAllDescendants(Transform transform, string message)
    {
        foreach (Transform item in transform)
        {
            bool active = item.gameObject.activeSelf;

            item.gameObject.SetActive(true);

            item.SendMessage(message, SendMessageOptions.DontRequireReceiver);
            SendMessageToAllDescendants(item, message);

            item.gameObject.SetActive(active);
        }
    }
}
