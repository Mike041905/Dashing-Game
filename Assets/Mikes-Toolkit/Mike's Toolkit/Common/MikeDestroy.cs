using UnityEngine;

namespace Mike
{
    public class MikeDestroy : MonoBehaviour
    {
        [Header("Options")]
        [Tooltip("Determines if the gameObject should start the countdoun on Start()")]
        [SerializeField] private bool _delayedDestroyOnStart = true;
        [Tooltip("Determines how much time should pass before the object should be destroyed after it was activated")]
        [SerializeField] private float delay = 1;

        void Start()
        {
            if (!_delayedDestroyOnStart) { return; }

            Destroy(gameObject, delay);
        }

        public void DestroyObject()
        {
            Destroy(gameObject);
        }
    }
}