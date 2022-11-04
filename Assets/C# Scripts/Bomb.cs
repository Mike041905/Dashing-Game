using UnityEngine;
using Mike;
using EZCameraShake;

public class Bomb : MonoBehaviour
{
    [System.Serializable]
    struct HitPositionIndicator
    {
        public SpriteRenderer spriteRenderer;
        public Gradient gradient;
        [Tooltip("0 is 0, 255 is 1")] public Gradient sizeAlphaIsSize;
        public float sizeMultiplier;
    }

    public Vector2 targetPosition;
    public float damage = 3;
    public float radius = 2;
    public float speed = 20;
    [SerializeField] private ParticleSystem explosionEffect;
    [SerializeField] private ParticleSystem trail;

    [SerializeField] private HitPositionIndicator[] hitPositionIndicators;

    Vector2 startPos;

    public float DistanceToTargetPerc { get => ((startPos.x - transform.position.x) / (startPos.x - targetPosition.x)); }

    void Start()
    {
        startPos = targetPosition + Vector2.one * (speed * Random.Range(2f, 6f));
        transform.position = startPos;
    }

    private void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        UpdateTargetPositionIndicators(DistanceToTargetPerc);

        if((Vector2) transform.position == targetPosition)
        {
            DealDamageToObjects(Physics2D.OverlapCircleAll(targetPosition, radius));
            explosionEffect.transform.parent = null;
            trail.transform.parent = null;

            ParticleSystem.MainModule mainModule = trail.main;
            mainModule.loop = false;

            explosionEffect.Play();
            CameraShaker.Instance.ShakeOnce(.5f, 7, .1f, .15f);
            Destroy(gameObject);
            Destroy(explosionEffect.gameObject, 1);
            Destroy(trail.gameObject, 1);
        }
    }


    //-----------------------


    void UpdateTargetPositionIndicators(float percent)
    {
        foreach (HitPositionIndicator indicator in hitPositionIndicators)
        {
            indicator.spriteRenderer.color = indicator.gradient.Evaluate(percent);
            indicator.spriteRenderer.transform.localScale = indicator.sizeAlphaIsSize.Evaluate(percent).a * indicator.sizeMultiplier * Vector3.one;
            indicator.spriteRenderer.transform.position = targetPosition;
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
                collider.GetComponent<Health>().TakeDamage(finalDamage, gameObject);
            }
        }
    }
}
