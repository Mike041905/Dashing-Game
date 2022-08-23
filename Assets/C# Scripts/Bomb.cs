using UnityEngine;
using Mike;

public class Bomb : MonoBehaviour
{
    public Vector2 targetPosition;
    public float damage = 3;
    public float radius = 2;
    public float speed = 20;
    [SerializeField] private ParticleSystem explosionEffect;

    void Start()
    {
        transform.position = targetPosition + Vector2.one * (speed * Random.Range(1f, 2f));
    }

    private void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if((Vector2) transform.position == targetPosition)
        {
            DealDamageToObjects(Physics2D.OverlapCircleAll(targetPosition, radius));
            explosionEffect.transform.parent = null;
            explosionEffect.Play();
            StartCoroutine(MikeScreenShake.Shake(Camera.main.transform, .02f, 5, 1));
            Destroy(gameObject);
            Destroy(explosionEffect.gameObject, 1);
        }
    }

    /// <summary>
    /// Deals damage by distance the farther from the object the less damage is dealt
    /// </summary>
    /// <param name="objects"></param>
    void DealDamageToObjects(Collider2D[] objects)
    {
        foreach (Collider2D collider in objects)
        {
            if (!collider.CompareTag("Player") && collider.GetComponent<Health>() != null)//check for health component
            {
                //calculate final damage
                float finalDamage = (radius - Vector2.Distance(transform.position, collider.transform.position) * 1 / radius) * damage * PlayerPrefs.GetFloat("Damage");

                //deal damage
                collider.GetComponent<Health>().TakeDamage(finalDamage);
            }
        }
    }
}
