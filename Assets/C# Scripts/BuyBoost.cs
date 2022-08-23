using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class BuyBoost : MonoBehaviour
{
    [Header("Required")]
    [SerializeField] private GameObject button;

    [Header("Settings")]
    [SerializeField] private string boostName;
    [SerializeField] private float boostDuration = 1800;
    [SerializeField] private int cost = 25;

    private void Update()
    {
        UpdateUI();
    }



    void UpdateUI()
    {
        if (Boosts.Instance.HasBoost(boostName))
        {
            button.GetComponent<Button>().interactable = false;
            button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Boosts.Instance.GetBoost(boostName).duration.ToString("HH:mm:ss");
        }
    }

    public void AddBoost()
    {

    }
}
