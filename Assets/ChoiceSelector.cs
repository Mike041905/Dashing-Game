using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class ChoiceSelector : MonoBehaviour
{
    public struct Choice
    {
        string name;
        string description;
        Sprite iconSpr;
        Image iconImg;

        Action onSelect;

        public Choice(string name, string description, Sprite iconSpr, Image iconImg, Action onSelect)
        {
            this.name = name;
            this.description = description;
            this.iconSpr = iconSpr;
            this.iconImg = iconImg;
            this.onSelect = onSelect;
        }
    }

    public Transform choiceholder;
    public GameObject choiceTemplate;

    private void Awake()
    {
        Initialize();
    }

    void Initialize()
    {
        GameObject go = new("Background");
        go.AddComponent(typeof(Image));
        go.AddComponent(typeof(HorizontalLayoutGroup));

        go.transform.parent = transform;


        GameObject template = new("ChoiceTemplate");
        go.AddComponent(typeof(Image));
        go.AddComponent(typeof(Button));
        go.AddComponent(typeof(TextMeshProUGUI));
        go.AddComponent(typeof(TextMeshProUGUI));

        template.transform.parent = go.transform;
    }

    public void DisplayChoice(Choice[] choices)
    {

    }
}
