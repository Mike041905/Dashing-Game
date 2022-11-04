/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mike;
using UnityEngine.UI;
using System;
using EZCameraShake;

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
    [SerializeField] private bool usePcControls = false; // TODO: this should set automaticaly


    //-----------------------------


    public event Action OnStartAiming;
    public event Action OnAiming;
    public event Action OnEndAiming;

    public event Action OnStartDash;
    public event Action OnEndDash;

    public event Action<GameObject> OnHitEnemy;

    private Vector2? firstTouchPosition = null;
    private Vector2 secondTouchPosition;
    private Vector2 lastDashPosition;
    private float stamina = 0;

    public Coroutine currentDash;


    //-----------------------------

    private void Awake()
    {
        InitializeVariables();
    }

    void Update()
    {
        SetPositionsAndDash(usePcControls);
        RechargeStamina();
        ManageLineRenderer();
    }


    //-----------------------------


    void ManageLineRenderer()
    {
        if (lineRenderer == null) { return; }

        Vector2 pos1 = Camera.main.ScreenToWorldPoint(firstTouchPosition.Value);
        Vector2 pos2 = Camera.main.ScreenToWorldPoint(secondTouchPosition);

        if (Input.touchCount > 0 || Input.GetMouseButton(0))
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

        stamina = maxStamina;
        staminaSlider.maxValue = maxStamina;
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
            OnAiming?.Invoke();

            if (firstTouchPosition == null)
            {
                firstTouchPosition = pcControls == true ? (Vector2)Input.mousePosition : Input.GetTouch(0).position;
            }
            else
            {
                secondTouchPosition = pcControls == true ? (Vector2)Input.mousePosition : Input.GetTouch(0).position;

                cameraTarget.position = (secondTouchPosition - firstTouchPosition.Value).normalized * dashDistance / 2 + (Vector2)transform.position;

                if ((secondTouchPosition - firstTouchPosition.Value).normalized != Vector2.zero) transform.rotation = MikeTransform.Rotation.LookTwards(transform.position, (secondTouchPosition - firstTouchPosition.Value).normalized + (Vector2)transform.position);

                directionIndicator.enabled = true;
                directionIndicator.SetPosition(0, transform.position);
                directionIndicator.SetPosition(1, (secondTouchPosition - firstTouchPosition.Value).normalized * 3 + (Vector2)transform.position);
            }
        }
        else if (firstTouchPosition != null)
        {
            directionIndicator.enabled = false;
            UseDash(dashSpeed, dashDistance);
        }
    }

    void UseDash(float speed, float distance)//checks if player has enough stamina and prevents from using the dash multiple times
    {
        Time.timeScale = 1;
        if (stamina < staminaDrain) { return; }
        if (currentDash != null) StopCoroutine(currentDash);
        currentDash = StartCoroutine(StartDash(speed, distance));
    }

    IEnumerator StartDash(float speed, float distance)
    {
        cameraTarget.localPosition = Vector3.zero;

        stamina -= staminaDrain;
        Vector2 finalPosition = (secondTouchPosition - firstTouchPosition.Value).normalized * distance + (Vector2)transform.position;
        transform.rotation = MikeTransform.Rotation.LookTwards(transform.position, finalPosition);

        Collider2D lastHitBarrier = GetComponent<Collider2D>();

        while (true)
        {
            lastDashPosition = transform.position;
            transform.position = Vector2.MoveTowards(transform.position, finalPosition, speed * Time.deltaTime);
            RaycastHit2D[] collider2Ds = Physics2D.CircleCastAll(transform.position, .55f, lastDashPosition, Vector2.Distance(transform.position, lastDashPosition));
            foreach (RaycastHit2D item in collider2Ds)
            {
                if (item.transform.CompareTag("Enemy"))
                {
                    Health enemy = item.transform.GetComponent<Health>();
                    DealDamage(enemy, damage);
                    DealKnockback(item.rigidbody, (Vector2)item.transform.position - item.point, knockbackForce);
                    CameraShaker.Instance.ShakeOnce(1, 3, .1f, .1f);

                    OnHitEnemy?.Invoke(item.transform.gameObject);
                }
                if (item.transform.CompareTag("Barrier") && item.collider != lastHitBarrier)
                {
                    transform.position = item.centroid;
                    lastHitBarrier = item.collider;

                    CameraShaker.Instance.ShakeOnce(1, 3, .1f, .1f);
                    Vector2 bounceDir = Vector2.Reflect(transform.up, item.normal);

                    distance /= 2;
                    speed /= 2;
                    finalPosition = bounceDir * distance + (Vector2)transform.position;

                    transform.rotation = MikeTransform.Rotation.LookTwards(transform.position, finalPosition);
                }
            }
            if ((Vector2)transform.position == finalPosition) { currentDash = null; break; }
            yield return null;
        }

        // Prevent Being Stuck in a wall
        bool checkForBarriers = true;
        for (int i = 0; checkForBarriers; i++)
        {
            checkForBarriers = false;
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, .6f);
            foreach (Collider2D hit in hits)
            {
                if (!hit.CompareTag("Barrier")) { continue; }
                transform.position += (Vector3)((Vector2)transform.position - hit.ClosestPoint(transform.position)).normalized * .6f;
                checkForBarriers = true;
                break;
            }

            if (i > 10) { transform.position += (Vector3)((Vector2)MikeGameObject.GetClosestTargetWithTag(transform.position, "Room").transform.position - (Vector2)transform.position).normalized; }
            if (i > 100) { break; } //percaution
        }
    }

    public void OnPause()
    {
        if (GetComponent<Health>().Dead) { return; }
        enabled = false;
    }

    public void OnResume()
    {
        if (GetComponent<Health>().Dead) { return; }
        enabled = true;
    }

    public void OnDeath()
    {
        cameraTarget.position = transform.position;
        lineRenderer.enabled = false;
        directionIndicator.enabled = false;
        if (currentDash != null) { StopCoroutine(currentDash); }
        GetComponent<Collider2D>().enabled = false;
        enabled = false;
        PowerUpAdder.Instance.gameObject.SetActive(false);
    }

    public void OnRevive()
    {
        GetComponent<Collider2D>().enabled = true;
        PowerUpAdder.Instance.gameObject.SetActive(true);
    }
}*/