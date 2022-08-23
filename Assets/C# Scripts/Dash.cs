using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mike;
using UnityEngine.UI;

public class Dash : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LineRenderer directionIndicator;
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform cameraTarget;

    [Header("Stats")]
    public float damage = 100;
    public float knockbackForce = 10;
    public float dashSpeed = 1;
    public float dashDistance = 1;
    public float staminaDrain = 1;
    public float maxStamina = 10;
    public float staminaRecharge = 2;

    [Header("Options")]
    [SerializeField] private bool usePcControls = false;


    //-----------------------------


    [HideInInspector] public bool slowMoUpgradeEnabled = false;
    [HideInInspector] public bool explodeUpgradeEnabled = false;
    [HideInInspector] public bool chainLightningUpgradeEnabled = false;

    private Vector2 firstTouchPosition;
    private Vector2 secondTouchPosition;
    private bool setFirstTouchPosition = false;
    private Vector2 lastDashPosition;
    private float stamina = 0;

    private Coroutine currentDash;


    //-----------------------------

    private void Awake()
    {
        InitializeVariables();

        stamina = maxStamina;
        staminaSlider.maxValue = maxStamina;
    }

    void Update()
    {
        SetPositionsAndDash(usePcControls);
        RechargeStamina();
        ManageLineRenderer();
        UpdateTriggerRunUpgradeCooldownTimer();
    }


    //-----------------------------


    void UpdateTriggerRunUpgradeCooldownTimer()
    {
       triggerRunUpgradeCooldown += Time.deltaTime;
    }

    void ManageLineRenderer()
    {
        if(lineRenderer == null) { return; }

        Vector2 pos1 = Camera.main.ScreenToWorldPoint(firstTouchPosition);
        Vector2 pos2 = Camera.main.ScreenToWorldPoint(secondTouchPosition);

        if(Input.touchCount > 0 || Input.GetMouseButton(0))
        {
            lineRenderer.enabled = true;

            lineRenderer.SetPositions(new Vector3[2] { pos1, pos2 });
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }

    void InitializeVariables()
    {
        damage = PlayerPrefs.GetFloat("Damage");
        staminaRecharge = PlayerPrefs.GetFloat("Stamina Recharge");
    }

    void RechargeStamina()
    {
        stamina += staminaRecharge * Time.deltaTime;
        if (stamina > maxStamina) stamina = maxStamina;
        staminaSlider.value = stamina;
    }

    void DealKnockback(Rigidbody2D targetRigidbody2D, Vector2 direction, float force)
    {
        targetRigidbody2D.AddForce(-direction * force, ForceMode2D.Impulse);
    }

    void DealDamage(Health target, float damage)
    {
        target.TakeDamage(damage);
    }

    public void SetPositionsAndDash(bool pcControls = false)
    {

        if (pcControls == true ? Input.GetMouseButton(0) : Input.touchCount > 0)
        {
            //slowMo upgrade
            if (slowMoUpgradeEnabled) Time.timeScale = .2f;

            if (!setFirstTouchPosition) firstTouchPosition = pcControls == true ? (Vector2) Input.mousePosition : Input.GetTouch(0).position;
            secondTouchPosition = pcControls == true ? (Vector2)Input.mousePosition : Input.GetTouch(0).position;
            setFirstTouchPosition = true;

            cameraTarget.position = (secondTouchPosition - firstTouchPosition).normalized * dashDistance / 2 + (Vector2)transform.position;

            if ((secondTouchPosition - firstTouchPosition).normalized != Vector2.zero) transform.rotation = MikeTransform.Rotation.LookTwards(transform.position, (secondTouchPosition - firstTouchPosition).normalized + (Vector2)transform.position);

            directionIndicator.enabled = true;
            directionIndicator.SetPosition(0, transform.position);
            directionIndicator.SetPosition(1, (secondTouchPosition - firstTouchPosition).normalized * 3 + (Vector2)transform.position);
        }
        else if(setFirstTouchPosition)
        {
            directionIndicator.enabled = false;
            setFirstTouchPosition = false;
            UseDash(dashSpeed, dashDistance);
        }
    }

    void UseDash(float speed, float distance)//checks if player has enough stamina and prevents from using the dash multiple times
    {
        Time.timeScale = 1;
        if (stamina < staminaDrain) { return; }
        if(currentDash != null) StopCoroutine(currentDash);
        currentDash = StartCoroutine(StartDash(speed, distance));
    }

    IEnumerator StartDash(float speed, float distance)
    {
        cameraTarget.localPosition = Vector3.zero;

        stamina -= staminaDrain;
        Vector2 finalPosition = (secondTouchPosition - firstTouchPosition).normalized * distance + (Vector2)transform.position;
        transform.rotation = MikeTransform.Rotation.LookTwards(transform.position, finalPosition);

        while (true)
        {
            lastDashPosition = transform.position;
            transform.position = Vector2.MoveTowards(transform.position, finalPosition, speed * Time.deltaTime);
            RaycastHit2D[] collider2Ds = Physics2D.CircleCastAll(transform.position, .5f, lastDashPosition, Vector2.Distance(transform.position, lastDashPosition));
            foreach (RaycastHit2D item in collider2Ds)
            {
                if (item.transform.CompareTag("Enemy"))
                {
                    Health enemy = item.transform.GetComponent<Health>();
                    DealDamage(enemy, damage);
                    DealKnockback(item.rigidbody, (Vector2) item.transform.position - item.point, knockbackForce);
                    StartCoroutine(MikeScreenShake.Shake(Camera.main.transform, .02f, 3, 4));

                    TriggerRunUpgradesOnCollision();
                }
                if (item.transform.CompareTag("Barrier")) 
                {
                    StartCoroutine(MikeScreenShake.Shake(Camera.main.transform, .02f, 3, 3));

                    transform.position = lastDashPosition;
                    finalPosition -= (Vector2) transform.position;
                    finalPosition *= -.5f;
                    finalPosition += (Vector2) transform.position;
                    speed *= .5f;
                }
                
            }
            if ((Vector2)transform.position == finalPosition) { currentDash = null; break; }
            yield return null;
        }
    }

    float triggerRunUpgradeCooldown = 0;
    void TriggerRunUpgradesOnCollision()
    {
        if(triggerRunUpgradeCooldown < .2f) { return; }

        triggerRunUpgradeCooldown = 0;
        RunUpgrades runUpgrades = GetComponent<RunUpgrades>();

        runUpgrades.Explode();
        runUpgrades.ChainLightning();
    }
}