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

    [SerializeField] private GameObject explosion;
    [SerializeField] private ParticleSystem trail;

    [SerializeField] private HitPositionIndicator[] hitPositionIndicators;

    Vector2 startPos;

    public float DistanceToTargetPerc { get => ((startPos.x - transform.position.x) / (startPos.x - targetPosition.x)); }

    void Start()
    {
        startPos = targetPosition + Vector2.one * (speed * Random.Range(1f, 3f));
        transform.position = startPos;
    }

    private void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        UpdateTargetPositionIndicators(DistanceToTargetPerc);

        if((Vector2) transform.position == targetPosition)
        {
            Instantiate(explosion, targetPosition, Quaternion.identity).GetComponent<Explosion>().radius = radius;
            trail.transform.parent = null;

            ParticleSystem.MainModule mainModule = trail.main;
            mainModule.loop = false;

            CameraShaker.Instance.ShakeOnce(.5f, 7, .1f, .15f);
            Destroy(gameObject);
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
}
