using UnityEngine;

namespace Mike
{
    public class MikeGameObject
    {
        /// <summary>
        /// Loops through all GameObjects with the tag of the param "tag" and compares their distances
        /// </summary>
        /// <param name="currentPosition"></param>
        /// <param name="tag"></param>
        /// <returns>Closest Transform with the "tag" tag relative to the "currentPosition" parameter</returns>
        public static GameObject GetClosestTargetWithTag(Vector2 currentPosition, string tag, GameObject[] exeptions = null)
        {
            Transform best = null;
            GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);

            foreach (GameObject item in targets)
            {
                if(exeptions == null || !GameObjectArrayContains(item, exeptions))
                {
                    if (best == null) { best = item.transform; }
                    else if (Vector2.Distance(currentPosition, best.position) > Vector2.Distance(currentPosition, item.transform.position)) { best = item.transform; }
                }
            }

            return best == null ? null : best.gameObject;
        }

        public static bool GameObjectArrayContains(GameObject gameObject, GameObject[] array)
        {
            foreach (GameObject item in array)
            {
                if (gameObject == item) return true;
            }

            return false;
        }
    }
}