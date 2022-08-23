using UnityEngine;

namespace Mike
{
    public class MikeCircleTrigger : MonoBehaviour
    {
        public float radius = .5f;
        public bool useOffsetRelativeToRotation = true;
        public Vector2 offset = Vector2.zero;
        [Tooltip("If set to false, script will use Update instead")] public bool useFixedUpdate = false;
        [SerializeField] private bool showGizmos = false;

        private Vector2 lastPosition;

        private void Start()
        {
            lastPosition = transform.position;
        }

        private void Update()
        {
            if (useFixedUpdate) { return; }

            Vector2 finalOffset = useOffsetRelativeToRotation ? offset.y * (Vector2)transform.up + offset.x * (Vector2)transform.right : offset;

            RaycastHit2D[] hits = Physics2D.CircleCastAll(lastPosition + finalOffset, radius, (Vector2)transform.position - lastPosition + finalOffset, Vector2.Distance(lastPosition + finalOffset, (Vector2)transform.position + finalOffset));

            foreach (RaycastHit2D hit in hits)
            {
                if (hit.transform != transform) { SendMessage("MikeTrigger", hit, SendMessageOptions.DontRequireReceiver); break; }
            }

            lastPosition = transform.position;
        }

        private void FixedUpdate()
        {
            if (!useFixedUpdate) { return; }

            Vector2 finalOffset = useOffsetRelativeToRotation ? offset.y * (Vector2)transform.up + offset.x * (Vector2)transform.right : offset;

            RaycastHit2D[] hits = Physics2D.CircleCastAll(lastPosition + finalOffset, radius, (Vector2)transform.position - lastPosition + finalOffset, Vector2.Distance(lastPosition + finalOffset, (Vector2)transform.position + finalOffset));

            foreach (RaycastHit2D hit in hits)
            {
                if (hit.transform != transform) { SendMessage("MikeTrigger", hit, SendMessageOptions.DontRequireReceiver); break; }
            }

            lastPosition = transform.position;
        }

        private void OnDrawGizmos()
        {
            if (!showGizmos) { return; }

            if (useOffsetRelativeToRotation)
            {
                Gizmos.DrawLine((Vector2)transform.position + offset.y * (Vector2)transform.up + offset.x * (Vector2)transform.right, lastPosition + offset.y * (Vector2)transform.up + offset.x * (Vector2)transform.right);
                Gizmos.DrawWireSphere(lastPosition + offset.y * (Vector2)transform.up + offset.x * (Vector2)transform.right, radius);
                Gizmos.DrawWireSphere((Vector2)transform.position - lastPosition + offset.y * (Vector2)transform.up + offset.x * (Vector2)transform.right, radius);
            }
            else
            {
                Gizmos.DrawLine((Vector2)transform.position, lastPosition);
                Gizmos.DrawWireSphere(lastPosition, radius);
                Gizmos.DrawWireSphere((Vector2)transform.position - lastPosition, radius);
            }
        }
    }
}