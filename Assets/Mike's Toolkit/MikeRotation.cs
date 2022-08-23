using UnityEngine;

namespace Mike
{
    class MikeRotation
    {
        public static UnityEngine.Quaternion Vector2ToAngle(float x, float y)
        {
            return UnityEngine.Quaternion.Euler(0, 0, Mathf.Atan2(new Vector2(x, y).normalized.y, new Vector2(x, y).normalized.x) * Mathf.Rad2Deg + 270);
        }

        public static float Vector2ToAngle(Vector2 vector2)
        {
            return Mathf.Atan2(vector2.normalized.y, vector2.normalized.x) * Mathf.Rad2Deg + 270;
        }
    }
}