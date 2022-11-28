using System.Collections.Generic;
using UnityEngine;

public class DisplayActivePowerUps : MonoBehaviour
{
    [SerializeField] Transform _activePowerUpsGrid;
    [SerializeField] PowerUpDisplayElement _powerUpTemplate;

    List<PowerUpDisplayElement> _powerUps = new();

    private void Start()
    {
        _powerUpTemplate.gameObject.SetActive(false);
    }

    public void UpdatePowerUps()
    {
        for (int i = 0; i < Mathf.Max(PowerUpAdder.PowerUps.Count, _powerUps.Count); i++)
        {
            if(_powerUps.Count <= i) { CreateNewPowerUpDisplayElement(); }
            else if(PowerUpAdder.PowerUps.Count <= i) { Destroy(_powerUps[i].gameObject); _powerUps.RemoveAt(i); continue; }

            _powerUps[i].SetPowerUp(PowerUpAdder.PowerUps[i]);
        }
    }

    void CreateNewPowerUpDisplayElement()
    {
        PowerUpDisplayElement element = Instantiate(_powerUpTemplate, _activePowerUpsGrid);
        element.gameObject.SetActive(true);
        _powerUps.Add(element);
    }
}
