using System.Threading.Tasks;
using UnityEngine;

public class PhoenixPowerUp : PowerUp
{
    [SerializeField] Explosion _explosion;
    [SerializeField] bool _oneTimeUse = true;

    bool _used = false;


    private void Start()
    {
        Player.Instance.PlayerHealth.OnDeath += Trigger;
    }

    async void Trigger()
    {
        if(_used) return;
        _used = true;

        await Task.Delay(2500);

        Player.Instance.PlayerHealth.Revive();
        Instantiate(_explosion, transform.position, Quaternion.identity);

        if (!_oneTimeUse) { PowerUpAdder.Instance.RemovePowerUp(this); }
    }

    private void OnDestroy()
    {
        Player.Instance.PlayerHealth.OnDeath -= Trigger;
    }
}
