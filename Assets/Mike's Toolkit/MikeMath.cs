using UnityEngine;

namespace Mike
{
    public class MikeMath
    {
        public static float Round(float value, int decimalPlaces = 2)
        {
            if (!value.ToString().Contains(".")) { return value; }
            int indexOfPoint = value.ToString().IndexOf('.');

            if (value.ToString().Length <= indexOfPoint + decimalPlaces + 1) { return value; }
            value = float.Parse(value.ToString().Remove(indexOfPoint + decimalPlaces + 1));

            return value;
        }

        public static Vector2 PredictFuturePosition2D(Vector2 position, Vector2 heading, float speed, float time)
        {
            return position + heading * speed * time;
        }

        public static Vector2 InterceptionPrediction2D(Vector2 targetPos, Vector2 targetHeading, float targetSpeed, Vector2 startPos, float speed, int predictionIterations)
        {
            Vector2 predictedTargetPos = targetHeading * targetSpeed;
            float timeStamp = Vector2.Distance(startPos, targetPos + predictedTargetPos) / speed;

            for (int i = 0; i < predictionIterations; i++)
            {
                predictedTargetPos = targetHeading * targetSpeed * timeStamp;
                timeStamp = Vector2.Distance(startPos, targetPos + predictedTargetPos) / speed;
            }

            return predictedTargetPos + targetPos;
        }
    }
}
