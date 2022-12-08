using System.Collections;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;

public class FightStartFinish : MonoBehaviour
{
    static FightStartFinish _instance;
    public static FightStartFinish Instance { get => _instance; }

    private void Awake()
    {
        _instance = this;
    }

    [SerializeField] private TextMeshProUGUI text;

    Task _activeText;
    string textString;
    Vector2 initialSize;
    Color initialColor;
    Vector2[] phaseSizeSpeeds;
    Color[] phaseColorSpeeds;
    float deactivationDelay;
    Vector2[] phaseChangeSizeThresholds;

    public void StartFight()
    {
        textString = "FIGHT!";
        initialSize = new Vector2(1.5f, 1.5f);
        initialColor = new Color(255, 0, 0, 0);
        phaseSizeSpeeds = new Vector2[2] { Vector2.one, Vector2.one };
        phaseColorSpeeds = new Color[2] { new Color(0, 0, 0, 2), new Color(0, 0, 0, -2) };
        deactivationDelay = 2;
        phaseChangeSizeThresholds = new Vector2[1] { new Vector2(2, 2) };

        _reset = true;
        _activeText ??= Animation();
    }
    
    public void EndFight()
    {
        textString = "ROOM COMPLETE!";
        initialSize = new Vector2(1.5f, 1.5f);
        initialColor = new Color(255, 255, 0, 0);
        phaseSizeSpeeds = new Vector2[2] { Vector2.one, Vector2.one };
        phaseColorSpeeds = new Color[2] { new Color(0, 0, 0, 2), new Color(0, 0, 0, -2) };
        deactivationDelay = 2;
        phaseChangeSizeThresholds = new Vector2[1] { new Vector2(2, 2) };

        _reset = true;
        _activeText ??= Animation();
    }


    bool _reset = false;
    async Task Animation()
    {
        int phase = 0;
        Init();

        while (true)
        {
            if (_reset) Init();

            transform.localScale += (Vector3) phaseSizeSpeeds[phase] * Time.deltaTime;
            text.color += phaseColorSpeeds[phase] * Time.deltaTime;

            if(phaseChangeSizeThresholds.Length > phase && phaseChangeSizeThresholds[phase].x <= transform.localScale.x && phaseChangeSizeThresholds[phase].y <= transform.localScale.y)
            {
                phase++;
            }

            if (deactivationDelay < 0)
            {
                text.enabled = false;
                deactivationDelay = 0;
            }
            else
            {
                deactivationDelay -= Time.deltaTime;
            }

            await Task.Yield();
        }

        void Init()
        {
            _reset = false;
            
            phase = 0;

            text.enabled = true;
            transform.localScale = initialSize;
            text.color = initialColor;
            text.text = textString;
        }
    }
}
