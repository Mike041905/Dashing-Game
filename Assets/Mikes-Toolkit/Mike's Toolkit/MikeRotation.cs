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

        //WIP
        /*public static UnityEngine.Quaternion Vector3ToQuatenion(Vector3 vector3)
        {
            Vector3 resultVector = Vector3.zero;

            resultVector.x = Mathf.Atan2(vector3.normalized.y, vector3.normalized.x) * Mathf.Rad2Deg + 90;
            resultVector.y = Mathf.Atan2(vector3.normalized.x, vector3.normalized.z) * Mathf.Rad2Deg + 90;
            resultVector.z = Mathf.Atan2(vector3.normalized.y, vector3.normalized.z) * Mathf.Rad2Deg + 90;

            return Quaternion.Euler(resultVector);
        }*/
    }
}