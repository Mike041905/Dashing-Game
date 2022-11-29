using EZCameraShake;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] string[] _damageWhitelist = { "Enemy" };
    public float damage = 1;
    public float radius = 2;
    [SerializeField] float damageDropOffMultiplier = 1;

    private void Start()
    {
        transform.localScale = Vector3.one * radius / 1.5f;
        Explode();
    }

    void Explode()
    {
        //get and array of all affected objects
        Collider2D[] affectedObject = Physics2D.OverlapCircleAll(transform.position, radius);

        DealDamageToObjects(affectedObject);

        Destroy(gameObject, 5f);
    }

    /// <summary>
    /// Deals damage by distance the farther from the object the less damage is dealt
    /// </summary>
    /// <param name="objects"></param>
    void DealDamageToObjects(Collider2D[] objects)
    {
        foreach (Collider2D collider in objects)
        {
            if (_damageWhitelist.Contains(collider.tag) && collider.GetComponent<Health>() != null)//check for health component
            {
                //calculate final damage
                float finalDamage = (radius - Vector2.Distance(transform.position, collider.ClosestPoint(transform.position)) * damageDropOffMultiplier / radius) * damage * Upgrade.GetUpgrade("Damage", UpgradeData.VariableType.Float);

                //deal damage
                collider.GetComponent<Health>().TakeDamage(finalDamage, gameObject);
                CameraShaker.Instance.ShakeOnce(1 + radius * .3f, 4 + radius * .5f, .05f * radius, .1f * radius);
                HapticFeedback.Vibrate(Mathf.RoundToInt(100 + 50 * radius));
            }
        }
    }
}
