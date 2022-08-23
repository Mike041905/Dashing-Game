using UnityEngine;

namespace Mike
{
    public class MikeRandom : MonoBehaviour
    {
        public static Vector2 RandomVector2(float minX, float maxX, float minY, float maxY)
        {
            return new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
        }

        public static int RandomIntByWeights(float[] weights)
        {
            //sum all the weights
            float sum = 0;
            foreach (float item in weights)
            {
                sum += item;
            }

            //choose a random value 
            float rndValue = Random.Range(0, sum);

            //return index
            float currentValue = 0;
            int currentIndex = 0;
            foreach (float item in weights)
            {
                currentValue += item;
                if(currentValue > rndValue) { return currentIndex; }
                currentIndex++;
            }

            return weights.Length - 1;
        }

        public static Quaternion RandomAngle(float minDeg, float maxDeg)
        {
            return Quaternion.Euler(Vector3.forward * Random.Range(minDeg, maxDeg));
        }
    }
}
