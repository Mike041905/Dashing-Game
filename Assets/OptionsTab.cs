using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class OptionsTab : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _tabName;
    [SerializeField] Button _button;

    public void Initalize(string tabName, GameObject content, OptionsMenu menu)
    {
        if(_button == null) { _button = GetComponent<Button>(); }

        _tabName.text = tabName;
        _button.onClick.AddListener(() => { menu.CloseAllTabs(); content.SetActive(true); });
    }
}
