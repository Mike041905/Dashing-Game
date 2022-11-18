using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    [System.Serializable]
    public struct UpgradeTabData
    {
        public string name;
        public UpgradeData[] upgrades;
    }


    //-------------------------------------


    [Header("References")]
    [SerializeField] GameObject tabTemplate;
    [SerializeField] Upgrade upgradeTemplate;
    [SerializeField] ParticleSystem _playerLevelUpFX;

    [Space(5)]
    [SerializeField] Transform tabHolder;
    [SerializeField] Transform upgradeHolder;
    [SerializeField] Transform upgradeHolderTemplate;

    [Header("Stats")]
    public UpgradeTabData[] tabs;


    //-------------------------------------


    private void Start()
    {
        Initialize();
    }


    //-------------------------------------


    void Initialize()
    {
        upgradeTemplate.transform.SetParent(null, false);

        AddUpgrades();

        DestroyImmediate(upgradeTemplate.gameObject);
        DestroyImmediate(tabTemplate);
        DestroyImmediate(upgradeHolderTemplate.gameObject);

        tabHolder.GetChild(0).GetComponent<UpgradeTab>().SwitchTab(this);
    }

    void AddUpgrades()
    {
        foreach (UpgradeTabData tab in tabs)
        {
            Transform tabInstance = Instantiate(tabTemplate, tabHolder).transform;
            Transform upgradeCategoryHolder = Instantiate(upgradeHolderTemplate, upgradeHolder).transform;

            tabInstance.GetComponent<UpgradeTab>().upgradeCatergoryHolder = upgradeCategoryHolder;
            tabInstance.GetComponent<UpgradeTab>().upgradeName.text = tab.name;

            foreach (UpgradeData upgrade in tab.upgrades)
            {
                Upgrade upgradeInstance = Instantiate(upgradeTemplate.gameObject, upgradeCategoryHolder.GetComponent<ScrollRect>().content).GetComponent<Upgrade>();

                upgradeInstance.UpgradeData = upgrade;
                upgradeInstance.upgradeButton.onClick.AddListener(() => { _playerLevelUpFX.Play(); });
            }
        }
    }


    //---------------------------------


    public void OpenTab(GameObject tab)
    {
        foreach (Transform child in tabHolder)
        {
            child.GetComponent<UpgradeTab>().upgradeCatergoryHolder.gameObject.SetActive(false);
        }

        tab.GetComponent<UpgradeTab>().upgradeCatergoryHolder.gameObject.SetActive(true);
    }
}
