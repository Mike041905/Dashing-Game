using UnityEngine;

namespace Mike
{
    public class MikeSphereTrigger : MonoBehaviour
    {
        public float radius = .5f;
        public bool useOffsetRelativeToRotation = true;
        public Vector3 offset = Vector3.zero;
        [Tooltip("If set to false, script will use Update instead")] public bool useFixedUpdate = false;
        [SerializeField] private bool showGizmos = false;
        [SerializeField] private bool disableOnHit = true;
        [SerializeField] private bool colideWithTriggers = false;

        private Vector3 lastPosition = Vector3.negativeInfinity;

        private void Awake()
        {
            lastPosition = transform.position;
        }

        private void Update()
        {
            if (useFixedUpdate) { return; }

            Vector3 finalOffset = useOffsetRelativeToRotation ? offset.y * transform.up + offset.x * transform.right + offset.z * transform.forward : offset;

            RaycastHit[] hits = Physics.SphereCastAll(lastPosition + finalOffset, radius, transform.position - lastPosition + finalOffset, Vector3.Distance(lastPosition + finalOffset, transform.position + finalOffset));

            if (hits.Length > 0)
            {
                RaycastHit bestHit = new RaycastHit();
                float bestDistance = Mathf.Infinity;

                foreach (RaycastHit hit in hits)
                {
                    if(!colideWithTriggers && hit.collider.isTrigger) { continue; }
                    if (Vector3.Distance(lastPosition, hit.point) < bestDistance) { bestHit = hit; bestDistance = Vector3.Distance(lastPosition, hit.point); }
                }

                if (bestDistance != Mathf.Infinity && bestHit.transform != transform) 
                {
                    SendMessage("OnMikeSphereTriggerEnter", bestHit, SendMessageOptions.DontRequireReceiver);
                    if (disableOnHit) { enabled = false; }
                } 
            }

            lastPosition = transform.position;
        }

        private void FixedUpdate()
        {
            if (!useFixedUpdate) { return; }

            Vector3 finalOffset = useOffsetRelativeToRotation ? offset.y * transform.up + offset.x * transform.right + offset.z * transform.forward : offset;

            RaycastHit[] hits = Physics.SphereCastAll(lastPosition + finalOffset, radius, transform.position - lastPosition + finalOffset, Vector3.Distance(lastPosition + finalOffset, transform.position + finalOffset));

            if (hits.Length > 0)
            {
                bool found = false;
                RaycastHit bestHit = hits[0];
                foreach (RaycastHit hit in hits)
                {
                    if (!colideWithTriggers && hit.collider.isTrigger) { continue; }
                    if (Vector3.Distance(lastPosition, hit.point) < Vector3.Distance(bestHit.point, lastPosition)) { bestHit = hit; found = true; }
                }

                if (found) SendMessage("OnMikeSphereTriggerEnter", bestHit, SendMessageOptions.DontRequireReceiver);
                if (disableOnHit) { enabled = false; }
            }

            lastPosition = transform.position;
        }

        private void OnDrawGizmos()
        {
            if (!showGizmos) { return; }

            if (useOffsetRelativeToRotation)
            {
                Gizmos.DrawLine(transform.position + offset.y * transform.up + offset.x * transform.right, lastPosition + offset.y * transform.up + offset.x * transform.right);
                Gizmos.DrawWireSphere(lastPosition + offset.y * transform.up + offset.x * transform.right, radius);
                Gizmos.DrawWireSphere(transform.position + offset.y * transform.up + offset.x * transform.right, radius);
            }
            else
            {
                Gizmos.DrawLine(transform.position, lastPosition);
                Gizmos.DrawWireSphere(lastPosition, radius);
                Gizmos.DrawWireSphere(transform.position, radius);
            }
        }
    }
}