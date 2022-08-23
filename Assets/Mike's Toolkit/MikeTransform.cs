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
    }

}
