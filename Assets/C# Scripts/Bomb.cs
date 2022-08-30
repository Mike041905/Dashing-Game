using UnityEngine;
using Mike;
using EZCameraShake;

public class Bomb : MonoBehaviour
{
    public Vector2 targetPosition;
    public float damage = 3;
    public float radius = 2;
    public float speed = 20;
    [SerializeField] private ParticleSystem explosionEffect;
    [SerializeField] private ParticleSystem trail;
    [SerializeField] private SpriteRenderer targetDesignator;

    Vector2 startPos;

    void Start()
    {
        startPos = targetPosition + Vector2.one * (speed * Random.Range(3f, 6f));
        transform.position = startPos;
    }

    private void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        targetDesignator.transform.position = targetPosition;
        targetDesignator.transform.localScale = ((transform.position.x - targetPosition.x) / (startPos.x - targetPosition.x)) * 25 * Vector2.one;
        targetDesignator.color = new Color(1,0,0, .5f - ((transform.position.x - targetPosition.x) / (startPos.x - targetPosition.x)));

        if((Vector2) transform.position == targetPosition)
        {
            DealDamageToObjects(Physics2D.OverlapCircleAll(targetPosition, radius));
            explosionEffect.transform.parent = null;
            trail.transform.parent = null;
            trail.loop = false;
            explosionEffect.Play();
            CameraShaker.Instance.ShakeOnce(.5f, 7, .1f, .15f);
            Destroy(gameObject);
            Destroy(explosionEffect.gameObject, 1);
            Destroy(trail.gameObject, 1);
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
