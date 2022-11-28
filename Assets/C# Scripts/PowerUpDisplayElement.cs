using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpDisplayElement : MonoBehaviour
{
    PowerUp _displayedPowerUp;
    public PowerUp DisplayedPowerUp { get => _displayedPowerUp; set => SetPowerUp(value);  }

    [SerializeField] Image _image;
    [SerializeField] TextMeshProUGUI _level;

    public void SetPowerUp(PowerUp powerUp)
    {
        _displayedPowerUp = powerUp;

        _image.sprite = _displayedPowerUp.icon;
        _level.text = _displayedPowerUp.PowerUpLevel.ToString();
    }

    public void ShowPowerUpInfo()
    {
        PowerUpInfo.Instance.ShowPowerUpInfo(DisplayedPowerUp);
    }
}
