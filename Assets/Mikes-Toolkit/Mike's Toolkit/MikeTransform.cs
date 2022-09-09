using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mike
{
    public class MikeTransform
    {
        public struct Rotation
        {
            public static Quaternion LookTwards(Vector2 currentPosition, Vector2 targetPosition)
            {
                ///<summary>
                ///Returns the Quaternion facing twards the target position
                ///</summary>
                return Quaternion.Euler(0, 0, Mathf.Atan2((targetPosition - currentPosition).normalized.y, (targetPosition - currentPosition).normalized.x) * Mathf.Rad2Deg + 270);
            }
        }

        public static Transform GetParentOfParents(Transform transform)
        {
            Transform parent = transform;

            while (parent.parent != null)
            {
                parent = parent.parent;
            }

            return parent;
        }
        
        public static Transform[] GetChildren(Transform transform)
        {
            Transform[] children = new Transform[transform.childCount];

            for (int i = 0; i < transform.childCount; i++)
            {
                children[i] = transform.GetChild(i);
            }

            return children;
        }
    }

}
