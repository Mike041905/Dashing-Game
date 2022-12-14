using UnityEngine;
using UnityEngine.Events;

namespace Mike
{
    public class MikeRandom : MonoBehaviour
    {
        public static Vector2 RandomVector2(float minX, float maxX, float minY, float maxY)
        {
            return new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
        }

        public static Vector2 RandomVector2(Vector2 min, Vector2 max)
        {
            return new Vector2(Random.Range(min.x, max.x), Random.Range(min.y, max.y));
        }

        public static Vector2 RandomVector2(float min, float max)
        {
            return new Vector2(Random.Range(min, max), Random.Range(min, max));
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
        public static int RandomIntByWeights<T>(T[] objects, System.Func<T, float> getWeight)
        {
            float[] weights = new float[objects.Length];
            for (int i = 0; i < objects.Length; i++)
            {
                weights[i] = getWeight(objects[i]);
            }

            return RandomIntByWeights(weights);
        }

        public static Quaternion RandomAngle(float minDeg, float maxDeg)
        {
            return Quaternion.Euler(Vector3.forward * Random.Range(minDeg, maxDeg));
        }
    }
}
