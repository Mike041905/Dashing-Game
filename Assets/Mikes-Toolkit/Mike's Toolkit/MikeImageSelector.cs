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

        int _currentImageIndex = 0;
        int CurrentImageIndex { get => _currentImageIndex % Images.Length; set => _currentImageIndex = value % Images.Length; }


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
            UpdateImage();
        }

        public void AddImages(Sprite[] images, bool deletePrevious = false)
        {
            if (deletePrevious) { Images = new Sprite[0]; }

            foreach (Sprite image in images)
            {
                AddImage(image);
            }
        }

        public void ShowNextImage()
        {
            CurrentImageIndex++;
            UpdateImage();
        }

        public void ShowPreviousImage()
        {
            CurrentImageIndex--;
            UpdateImage();
        }

        public void UpdateImage()
        {
            ImageDisplay.sprite = Images[CurrentImageIndex % Images.Length];
            ImageNumber.text = (CurrentImageIndex + 1) + "/" + Images.Length;
        }
    }
}
