using System.Linq;
using UnityEngine;

public class PullPowerUp : PowerUp
{
    [SerializeField] float _force;
    [SerializeField] ParticleSystem _particles;

    float? _size;
    float Size { get => _size ??= GetStat("Size").statValue; set => _size = GetStat("Size").statValue; }
    
    Dash PlayerDash { get => Player.Instance.PlayerDash; }

    public override void UpgradePowerUp(int times = 1)
    {
        base.UpgradePowerUp(times);

        Size = 0;
    }

    private void Start()
    {
        PlayerDash.OnStartDash += Pull;
    }

    void Pull()
    {
        transform.rotation = Player.Instance.SpriteRenderer.transform.rotation;
        _particles.Play();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(PlayerDash.transform.position + Player.Instance.SpriteRenderer.transform.up * Size, Size);

        foreach (Collider2D collider in colliders)
        {
            if (!collider.CompareTag("Enemy")) { continue; }
            collider.attachedRigidbody.AddForce((collider.transform.position - (PlayerDash.transform.position + Player.Instance.SpriteRenderer.transform.up * PlayerDash.DashDistance)).normalized * -_force * Vector2.Distance(PlayerDash.transform.position, collider.transform.position), ForceMode2D.Impulse);
        }
    }
}
