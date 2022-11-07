using UnityEngine;
using TMPro;

public class GameVersion : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] TextMeshProUGUI textComponent;

    [Header("Options")]
    [SerializeField] string preffix = "v";
    [SerializeField] string suffix = "";

    void Start()
    {
        textComponent.text = preffix + Application.version + suffix;
    }
}
