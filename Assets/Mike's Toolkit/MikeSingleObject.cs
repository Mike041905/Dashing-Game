using UnityEngine;

namespace Mike
{
    public class MikeSingleObject : MonoBehaviour
    {
        [Space(15)]
        [Header("Note: This script uses tags to identify duplicates!")]
        [SerializeField] bool requireSameName = true;

        private void Awake()
        {
            tag = "SingleObject";

            RemoveIfDuplicate();
        }

        void RemoveIfDuplicate()
        {
            if (!CheckIfIsDuplicates()) { return; }
            Destroy(gameObject);
        }

        bool CheckIfIsDuplicates()
        {
            if (!requireSameName && GameObject.FindGameObjectsWithTag("SingleObject").Length > 1) { return true; }

            GameObject[] possibleDuplicates = GameObject.FindGameObjectsWithTag("SingleObject");
            foreach (GameObject item in possibleDuplicates)
            {
                if(item != gameObject && item.name == name) { return true; }
            }

            return false;
        }
    }
}