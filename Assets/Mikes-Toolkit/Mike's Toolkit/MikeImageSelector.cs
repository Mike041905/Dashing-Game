using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mike
{
    [ExecuteAlways]
    public class MikeImageSelector : MonoBehaviour
    {
        [field: SerializeField] public Sprite[] Images { get; private set; } = new Sprite[0];
        [field: SerializeField] public Image ImageDisplay { get; private set; }

        [field: SerializeField] public Button ButtonNext { get; private set; }
        [field: SerializeField] public TextMeshProUGUI ImageNumber { get; private set; }
        [field: SerializeField] public Button ButtonPrevious { get; private set; }

        int currentImageIndex = 0;


        private void Awake()
        {
            Initialize();
        }

        private void Start()
        {
            UpdateImage();
        }


        void Initialize()
        {
            gameObject.GetAddComponent<VerticalLayoutGroup>();

            if(ImageDisplay == null) ImageDisplay = transform.GetAddChild("ImageDisplay").GetAddComponent<Image>();

            if(ButtonNext == null) ButtonNext = transform.GetAddChild("Selector").GetAddChild("NextButton").GetAddComponent<Button>();
            if(ImageNumber == null) ImageNumber = transform.GetAddChild("Selector").GetAddChild("Number").GetAddComponent<TextMeshProUGUI>();
            if(ButtonPrevious == null) ButtonPrevious = transform.GetAddChild("Selector").GetAddChild("PreviousButton").GetAddComponent<Button>();

            ButtonNext.onClick.AddListener(ShowNextImage);
            ButtonPrevious.onClick.AddListener(ShowPreviousImage);
        }

        public void AddImage(Sprite image)
        {
            Images = Images.Append(image);
        }

        public void AddImages(Sprite[] images)
        {
            foreach (Sprite image in images)
            {
                AddImage(image);
            }
        }

        public void ShowNextImage()
        {
            currentImageIndex++;
            UpdateImage();
        }

        public void ShowPreviousImage()
        {
            currentImageIndex--;
            UpdateImage();
        }

        public void UpdateImage() => ImageDisplay.sprite = Images[currentImageIndex % Images.Length];
    }
}
