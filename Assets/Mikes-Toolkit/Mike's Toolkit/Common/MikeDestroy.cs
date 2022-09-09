using UnityEngine;

namespace Mike
{
    public class MikeDestroy : MonoBehaviour
    {
        [Header("Options")]
        [Tooltip("Determines how much time should pass before the object should be destroyed after it was activated")]
        [SerializeField] private float delay = 1;

        void Start()
        {
            Destroy(gameObject, delay);
        }
    }

}