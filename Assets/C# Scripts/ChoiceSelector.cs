using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[ExecuteAlways]
public class ChoiceSelector : MonoBehaviour
{
    public struct ChoiceData
    {
        public string name;
        public string description;
        public Sprite iconSpr;

        public ChoiceData(string name, string description, Sprite iconSpr)
        {
            this.name = name;
            this.description = description;
            this.iconSpr = iconSpr;
        }
    }

    struct ChoiceQueueData
    {
        public ChoiceData[] data;
        public UnityAction<int> callback;

        public ChoiceQueueData(ChoiceData[] data, UnityAction<int> callback)
        {
            this.data = data;
            this.callback = callback;
        }
    }

    public GameObject choiceHolder;
    public GameObject background;
    public Choice choiceTemplate;

    /// <summary>
    /// Returns choice index
    /// </summary>
    public event UnityAction<int> OnSelect;
    [SerializeField] UnityEvent OnStartChoice;
    [SerializeField] UnityEvent OnEndChoice;

    bool activeChoice;
    List<ChoiceQueueData> queue = new();

    public void OnChoiceSelected(int choiceIndex)
    {
        for (int i = 0; i < choiceHolder.transform.childCount; i++)
        {
            Destroy(choiceHolder.transform.GetChild(i).gameObject);
        }

        activeChoice = false;
        Time.timeScale = 1;
        background.SetActive(false);

        OnSelect?.Invoke(choiceIndex);
        OnSelect -= OnSelect;
        OnEndChoice?.Invoke();
        DisplayChoiceFromQueue();
    }

    private void Awake()
    {
        Initialize();
    }

    private void Start()
    {
        if (!Application.IsPlaying(gameObject)) { return; }
        choiceTemplate.transform.SetParent(transform);
        choiceTemplate.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (activeChoice && Time.timeScale > 0.0001f)
        {
            Time.timeScale = 0.00001f;
        }
    }

    void Initialize()
    {
        if (Application.IsPlaying(gameObject)) { return; }
        if (transform.Find("Background") != null) { return; }

        background = new("Background");
        background.transform.SetParent(transform);
        background.AddComponent<Image>();
        background.transform.localPosition = Vector3.zero;
        background.transform.localScale = Vector3.one;


        choiceHolder = new GameObject("Holder");
        choiceHolder.transform.parent = background.transform;
        choiceHolder.AddComponent<HorizontalLayoutGroup>();

        choiceTemplate = new GameObject("Template").AddComponent<Choice>();
        choiceTemplate.transform.SetParent(choiceHolder.transform);
        choiceTemplate.Initialize(this);
    }

    /// <summary>
    /// Displays choices and uses <see cref="CallBack"/> to return choice Index
    /// </summary>
    /// <param name="choices"></param>
    /// <param name="CallBack"></param>
    public void DisplayChoice(ChoiceData[] choices, UnityAction<int> CallBack)
    {
        queue.Add(new ChoiceQueueData(choices, CallBack));
        DisplayChoiceFromQueue();
    }

    void DisplayChoiceFromQueue()
    {
        if(queue.Count <= 0) { return; }

        background.SetActive(true);
        activeChoice = true;

        OnStartChoice?.Invoke();
        OnSelect += queue[0].callback;

        for (int i = 0; i < queue[0].data.Length; i++)
        {
            Choice choice = Instantiate(choiceTemplate.gameObject, choiceHolder.transform).GetComponent<Choice>();
            choice.SetAsChoice(queue[0].data[i], i);
            choice.gameObject.SetActive(true);
        }

        queue.RemoveAt(0);
    }
}
