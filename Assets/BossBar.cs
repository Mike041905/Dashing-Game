using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class BossBar : MonoBehaviour
{
    static BossBar _instance;
    public static BossBar Instance { get => _instance; }

    private void Awake()
    {
        _instance = this;
    }

    //-----------------

    [SerializeField] private TextMeshPro _bossName;
    [SerializeField] private Slider _bossHealthBar;
    [SerializeField] private CanvasGroup _holder;

    //----------------

    /// <summary>
    /// The currently tracked boss
    /// </summary>
    public BossAI TrackedBoss { get; private set; }
    /// <summary>
    /// Returnes if the boss bar is displayed
    /// </summary>
    public bool BossBarActive { get => TrackedBoss != null; }

    //---------------------

    /// <summary>
    /// Shows Boss Bar and asigns the boss's name to it. if already active it will override last tracked boss
    /// </summary>
    /// <param name="boss"></param>
    public void DisplayBar(BossAI boss)
    {
        TrackedBoss = boss;
        _bossName.text = TrackedBoss.BossName;
        _bossHealthBar.maxValue = TrackedBoss.EnemyHealth.maxhealth;
        _bossHealthBar.value = TrackedBoss.EnemyHealth.CurrentHealth;
        _holder.gameObject.SetActive(true);

        TrackedBoss.EnemyHealth.OnHealthChanged += SetBossBarHealth;
        ChangeAlpha(1, 1);
    }

    //-------------------------------

    Coroutine _healthLerp = null;
    void SetBossBarHealth(float health)
    {
        if(_healthLerp != null) { StopCoroutine(_healthLerp); }
        _healthLerp = StartCoroutine(LerpHealthBarValue(health));

        if(health <= 0) { ChangeAlpha(0, 1, () => { _holder.gameObject.SetActive(false); TrackedBoss = null; }); }
    }
    IEnumerator LerpHealthBarValue(float value)
    {
        while (_bossHealthBar.value > value + .1f || _bossHealthBar.value < value - .1f)
        {
            _bossHealthBar.value = Mathf.Lerp(_bossHealthBar.value, value, Time.deltaTime * 4);

            yield return null;
        }

        _bossHealthBar.value = value;
    }


    Coroutine _alphaCoroutine = null;
    void ChangeAlpha(float targetAlpha, float speed, UnityAction onFinish = null)
    {
        if(_alphaCoroutine!= null) { StopCoroutine(_alphaCoroutine);}
        _alphaCoroutine = StartCoroutine(ChangeAlphaCoroutine(targetAlpha, speed, onFinish));
    }
    IEnumerator ChangeAlphaCoroutine(float targetAlpha, float speed, UnityAction onFinish = null)
    {
        while (_holder.alpha != targetAlpha)
        {
            _holder.alpha = Mathf.Lerp(_holder.alpha, targetAlpha, Time.deltaTime * speed);

            yield return null;
        }

        onFinish?.Invoke();
    }
}
