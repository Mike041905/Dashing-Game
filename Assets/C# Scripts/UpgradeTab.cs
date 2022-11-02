using TMPro;
using UnityEngine;

public class UpgradeTab : MonoBehaviour
{
    public TextMeshProUGUI upgradeName;
    public Transform upgradeCatergoryHolder;

    public void SwitchTab(UpgradeManager um)
    {
        um.OpenTab(gameObject);
    }
}
