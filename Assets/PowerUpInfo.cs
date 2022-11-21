using Mike;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PowerUpInfo : MonoBehaviour
{
	static PowerUpInfo _instance;
	public static PowerUpInfo Instance { get => _instance; }

	private void Awake()
	{
		_instance = this;
	}

	[SerializeField] GameObject _contentHolder;

	[SerializeField] TextMeshProUGUI _powerUpNameText;
	[SerializeField] TextMeshProUGUI _powerUpDescriptionText;
	[SerializeField] MikeImageSelector _imageSelector;

	public void ShowPowerUpInfo(PowerUp powerUp)
    {
		_contentHolder.SetActive(true);

		_powerUpNameText.text = powerUp.powerUpName;
		_powerUpDescriptionText.text = powerUp.description + "\n\n" + powerUp.LongDescription;
		_imageSelector.AddImages(powerUp.Images, true);
    }
}
