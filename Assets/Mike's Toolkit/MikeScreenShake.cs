using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mike
{
    public class MikeScreenShake
    {
        public static IEnumerator Shake(Transform cameraTansform, float delayBeweenShakes, float magnitude, int shakes)
        {
            //initalize local variables
            int i = 0;
            float timer = 0;
            Vector2 pos = new Vector2(cameraTansform.position.x + Random.Range(-.1f, .1f) * magnitude, cameraTansform.position.y + Random.Range(-.1f, .1f) * magnitude);

            //loop 
            while (i < shakes)
            {
                //causes the loop to run only after fixed update
                yield return new WaitForFixedUpdate();

                if(cameraTansform == null) { yield break; }

                if (timer >= delayBeweenShakes)
                {
                    i++;
                    timer = 0;
                    pos = new Vector2(cameraTansform.position.x + Random.Range(-.1f, .1f) * magnitude, cameraTansform.position.y + Random.Range(-.1f, .1f) * magnitude);
                }
                else
                {
                    timer += Time.fixedDeltaTime;
                    cameraTansform.position = Vector2.MoveTowards(cameraTansform.position, pos, Vector2.Distance(cameraTansform.position, pos) / delayBeweenShakes * Time.fixedDeltaTime);
                }
            }

            cameraTansform.localPosition = Vector3.zero;
        }
    }
}
