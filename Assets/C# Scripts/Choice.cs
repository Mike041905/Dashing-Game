using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Choice : MonoBehaviour
{
    public ChoiceSelector selector;
    public int choiceIndex;
    
    public string ChoiceName { get => nameText.text; set => nameText.text = value; }
    public string ChoiceDescription { get => descriptionText.text; set => descriptionText.text = value; }
    public Sprite Icon { get => iconImg.sprite; set => iconImg.sprite = value; }

    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] Image iconImg;
    [SerializeField] Button button;

    public void Initialize(ChoiceSelector selector)
    {
        this.selector = selector;

        nameText = new GameObject("Name").AddComponent<TextMeshProUGUI>();
        nameText.transform.SetParent(transform);
        descriptionText = new GameObject("Description").AddComponent<TextMeshProUGUI>();
        descriptionText.transform.SetParent(transform);
        iconImg = new GameObject("Icon").AddComponent<Image>();
        iconImg.transform.SetParent(transform);

        button = gameObject.AddComponent<Button>();
        button.transform.SetParent(transform);

        button.targetGraphic = gameObject.AddComponent<Image>();
    }

    private void Start()
    {
        button.onClick.AddListener(Click);
    }

    void Click()
    {
        selector.OnChoiceSelected(choiceIndex);
        Destroy(gameObject);
    }

    public void SetAsChoice(ChoiceSelector.ChoiceData data, int index)
    {
        choiceIndex = index;
        ChoiceName = data.name;
        ChoiceDescription = data.description;
        Icon = data.iconSpr;
    }
}
