using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplayActivePowerUps : MonoBehaviour
{
    [SerializeField] GameObject activePowerUpsGrid;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowActivePowerUps()
    {
        GameObject emptyGameObject;
        foreach (PowerUp powerUp in PowerUpAdder.PowerUps)
        {
            emptyGameObject = new GameObject(powerUp.powerUpName);
            emptyGameObject.transform.parent = activePowerUpsGrid.transform;
            emptyGameObject.transform.localScale =  Vector3.one;
            emptyGameObject.transform.localPosition = Vector3.zero;
            emptyGameObject.AddComponent<Image>().sprite = powerUp.icon;
            print($"{powerUp.powerUpName}'s level is: {powerUp.powerUpLevel}");
        }
    }

    public void RemoveActivePowerUpsObjects()
    {
        for (int i = 0; i < activePowerUpsGrid.transform.childCount; i++)
        {
            Destroy(activePowerUpsGrid.transform.GetChild(i).gameObject);
        }
    }
}
