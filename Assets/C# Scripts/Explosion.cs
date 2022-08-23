using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] float damage = 10;
    public float radius = 2;
    [SerializeField] float damageDropOffMultiplier = 1;

    private void Start()
    {
        Explode();
    }

    void Explode()
    {
        //get and array of all affected objects
        Collider2D[] affectedObject = Physics2D.OverlapCircleAll(transform.position, radius);

        DealDamageToObjects(affectedObject);

        Destroy(gameObject, 1f);
    }

    /// <summary>
    /// Deals damage by distance the farther from the object the less damage is dealt
    /// </summary>
    /// <param name="objects"></param>
    void DealDamageToObjects(Collider2D[] objects)
    {
        foreach (Collider2D collider in objects)
        {
            if(!collider.CompareTag("Player") && collider.GetComponent<Health>() != null)//check for health component
            {
                //calculate final damage
                float finalDamage = (radius - Vector2.Distance(transform.position, collider.transform.position) * damageDropOffMultiplier / radius) * damage * PlayerPrefs.GetFloat("Damage");

                //deal damage
                collider.GetComponent<Health>().TakeDamage(finalDamage);
            }
        }
    }
}
