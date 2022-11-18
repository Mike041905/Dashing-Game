using UnityEngine;

namespace Mike
{
    public static class GameObjectExtention
    {
        #region GetAddComponent
        public static T GetAddComponent<T>(this GameObject parent) where T : Component => MikeGameObject.GetAddComponent<T>(parent);
        public static T GetAddComponent<T>(this Transform parent) where T : Component => MikeGameObject.GetAddComponent<T>(parent.gameObject);
        #endregion

        #region GetAddComponentInChild
        public static T GetAddComponentInChild<T>(this GameObject parent, string name) where T : Component => MikeGameObject.GetAddComponentInChild<T>(parent.transform, name);
        public static T GetAddComponentInChild<T>(this Transform parent, string name) where T : Component => MikeGameObject.GetAddComponentInChild<T>(parent, name);
        #endregion

        #region GetAddChild
        public static Transform GetAddChild(this Transform parent) => MikeGameObject.GetAddChild(parent);
        public static Transform GetAddChild(this GameObject parent) => MikeGameObject.GetAddChild(parent.transform);

        public static Transform GetAddChild(this Transform parent, int index) => MikeGameObject.GetAddChild(parent, index);
        public static Transform GetAddChild(this GameObject parent, int index) => MikeGameObject.GetAddChild(parent.transform, index);

        public static Transform GetAddChild(this Transform parent, string name) => MikeGameObject.GetAddChild(parent, name);
        public static Transform GetAddChild(this GameObject parent, string name) => MikeGameObject.GetAddChild(parent.transform, name);
        #endregion
    }

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

        public static T GetAddComponentInChild<T>(Transform parent, string name) where T : Component
        {
            return GetAddComponent<T>(GetAddChild(parent, name).gameObject);
        }
        public static T GetAddComponent<T>(GameObject parent) where T : Component
        {
            if (parent.TryGetComponent(out T comp)) { return comp; }
            else { return parent.AddComponent<T>(); }
        }

        public static Transform GetAddChild(Transform parent)
        {
            return GetAddChild(parent, 0);
        }
        public static Transform GetAddChild(Transform parent, int childIndex)
        {
            Transform child = parent.GetChild(childIndex);
            if (child == null)
            {
                child = new GameObject("GameObject").transform;
                child.parent = parent;
                return child;
            }
            else
            {
                return child;
            }
        }
        public static Transform GetAddChild(Transform parent, string name)
        {
            Transform child = parent.Find(name);
            if (child == null)
            {
                child = new GameObject(name).transform;
                child.parent = parent;
                return child;
            }
            else
            {
                return child;
            }
        }
    }
}