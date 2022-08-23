using System.Collections;
using UnityEngine;
using TMPro;

public class FightStartFinish : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    public void StartFight()
    {
        //bruh this is a mess!
        StartCoroutine(Animation(
            "FIGHT!",
            new Vector2(1.5f, 1.5f),
            new Color(255, 0, 0, 0),
            new Vector2[2] { Vector2.one, Vector2.one },
            new Color[2] {new Color(0, 0, 0, 2), new Color(0, 0, 0, -2) },
            2,
            new Vector2[1] { new Vector2(2, 2) }
        ));
    }
    
    public void EndFight()
    {
        //bruh this is a mess!
        StartCoroutine(Animation(
            "ROOM COMPLETE!",
            new Vector2(1.5f, 1.5f),
            new Color(255, 255, 0, 0),
            new Vector2[2] { Vector2.one, Vector2.one },
            new Color[2] {new Color(0, 0, 0, 2), new Color(0, 0, 0, -2) },
            2,
            new Vector2[1] { new Vector2(2, 2) }
        ));
    }


    IEnumerator Animation(string textString, Vector2 initialSize, Color initialColor, Vector2[] phaseSizeSpeeds, Color[] phaseColorSpeeds, float deactivationDelay, Vector2[] phaseChangeSizeThresholds)
    {
        int phase = 0;

        text.enabled = true;
        transform.localScale = initialSize;
        text.color = initialColor;
        text.text = textString;

        while (true)
        {
            deactivationDelay -= Time.deltaTime;


            transform.localScale += (Vector3) phaseSizeSpeeds[phase] * Time.deltaTime;
            text.color += phaseColorSpeeds[phase] * Time.deltaTime;


            if(phaseChangeSizeThresholds.Length > phase && phaseChangeSizeThresholds[phase].x <= transform.localScale.x && phaseChangeSizeThresholds[phase].y <= transform.localScale.y)
            {
                phase++;
            }

            if (deactivationDelay <= 0)
            {
                text.enabled = false;
                yield break;
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
