using Mike;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static UnityEditor.Progress;

[RequireComponent(typeof(HorizontalLayoutGroup))]
public class OptionsMenu : MonoBehaviour
{
    // IDK why I'm making this script this way. Maybe I'm just lazy :/
    [Serializable]
    struct SettingsField
    {
        [SerializeField] Option _optionTemplate;
        Option _option;

        [SerializeField] string _optionName;
        [SerializeField] string _optionKey;
        [SerializeField] bool _requireApply;

        public void Spawn(OptionsMenu options)
        {
            _option = Instantiate(_optionTemplate, options.ScrollOptions.content);
            _option.Initialize(_optionName, _optionKey, _requireApply, options, options.OnApply);
            _option.gameObject.SetActive(true);

            _optionTemplate.gameObject.SetActive(false);
        }
    }

    [Serializable]
    struct Tab
    {
        [SerializeField] string _tabName;
        [SerializeField] SettingsField[] _fields;

        [Space(10)]
        [SerializeField] OptionsTab _tabTemplate;

        public void SpawnTab(OptionsMenu options)
        {
            OptionsTab _tabInstance = Instantiate(_tabTemplate, options.ScrollTabs.content);
            _tabInstance.gameObject.SetActive(true);
            _tabInstance.Initalize(_tabName, SpawnFields(options), options);

            _tabTemplate.gameObject.SetActive(false);
        }

        GameObject SpawnFields(OptionsMenu options)
        {
            GameObject _fieldInstanceHolder = new();
            _fieldInstanceHolder.transform.parent = options.ScrollOptions.content.transform;
            _fieldInstanceHolder.SetActive(false);
            _fieldInstanceHolder.AddComponent<VerticalLayoutGroup>();

            foreach (SettingsField field in _fields)
            {
                field.Spawn(options);
            }

            return _fieldInstanceHolder;
        }
    }

    [field: SerializeField] public ScrollRect ScrollOptions { get; private set; }
    [field: SerializeField] public ScrollRect ScrollTabs { get; private set; }
    [field: SerializeField] public Button ApplyButton { get; private set; }

    public event UnityAction OnApply;

    [SerializeField] Tab[] _tabs;

    private void Start()
    {
        if(ScrollTabs == null) ScrollTabs = gameObject.GetAddChild("Tabs").GetAddComponent<ScrollRect>();
        if(ScrollOptions == null) ScrollOptions = gameObject.GetAddChild("Options").GetAddComponent<ScrollRect>();
        if (ApplyButton == null) ApplyButton = gameObject.GetAddChild("ApplyButton").GetAddComponent<Button>();

        ApplyButton.gameObject.GetAddComponent<LayoutElement>().ignoreLayout = true;
        ApplyButton.onClick.AddListener(OnApply);

        InstatiateTemplates();
    }

    void InstatiateTemplates()
    {
        for (int i = 0; i < _tabs.Length; i++)
        {
            _tabs[i].SpawnTab(this);
        }
    }

    public void CloseAllTabs()
    {
        foreach (Transform tabContent in ScrollOptions.content)
        {
            tabContent.gameObject.SetActive(false);
        }
    }

    public void RequireApply()
    {
        ApplyButton.interactable = true;
    }
}
