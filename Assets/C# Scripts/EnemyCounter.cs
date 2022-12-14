using UnityEngine;
using TMPro;
using Mike;

public class EnemyCounter : MonoBehaviour
{
    static EnemyCounter _instance;
    public static EnemyCounter Instance { get => _instance; }

    private void Awake()
    {
        _instance = this;
    }


    //-------------------


    [SerializeField] string preffix = "Enemies Remaining: ";
    [SerializeField] TextMeshProUGUI counterText;

    public void ChangeAmmount(int enemiesLeft)
    {
        if (this == null) { return; }
        if (counterText == null) { return; }
        if (!enabled) { return; }
        if (!gameObject.activeSelf) { return; }
        if (!isActiveAndEnabled) { return; }

        counterText.StartColorTransion(new(1,1,1, Mathf.Clamp01(enemiesLeft)), 2);
        counterText.text = preffix + enemiesLeft;
    }
}
