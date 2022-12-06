using Mike;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Slice : MonoBehaviour
{
    [SerializeField] TrailRenderer _trail;
    [SerializeField] float _speed;

    public static Slice SpawnSlice(Slice prefab, Vector2 origin, Vector2 direction, float damage, float distance, float width, string[] hitBlacklist)
    {
        return Instantiate(prefab).Initialize(origin, direction, damage, distance, width, hitBlacklist);
    }

    public Slice Initialize(Vector2 origin, Vector2 direction, float damage, float distance, float width, string[] hitBlacklist)
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(origin, new(width, distance), MikeRotation.Vector2ToAngle(direction));

        foreach (Collider2D hit in hits)
        {
            if (hitBlacklist.Contains(hit.tag)) { continue; }
            if(!hit.TryGetComponent(out Health health)) { continue; }

            health.TakeDamage(damage);
        }

        _trail.emitting = false;
        transform.position = origin - distance / 2 * direction;
        _trail.emitting = true;

        DoSlashAnimation(origin + distance / 2 * direction);
        

        return this;
    }

    async void DoSlashAnimation(Vector2 endPos)
    {
        while ((Vector2) transform.position != endPos)
        {
            await Task.Yield();

            if(gameObject == null) { return; }

            transform.position = Vector2.MoveTowards(transform.position, endPos, _speed * Time.deltaTime);
        }
    }
}
