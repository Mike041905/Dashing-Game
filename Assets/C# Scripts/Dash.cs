using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mike;
using UnityEngine.UI;
using System;
using EZCameraShake;
using UnityEngine.Events;

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


    public event UnityAction OnStartAiming;
    public event UnityAction OnAiming;
    public event UnityAction OnEndAiming;

    public event UnityAction OnStartDash;
    public event UnityAction OnEndDash;

    public event UnityAction<GameObject> OnHitEnemy;

    private bool isAiming = false;
    private Vector2 firstTouchPosition;
    private Vector2 secondTouchPosition;
    private Vector2 lastDashPosition;
    private float stamina = 0;

    public Coroutine currentDash;

    Rigidbody2D rb;
    Rigidbody2D Rb { get { if (rb == null) { rb = GetComponent<Rigidbody2D>(); } return rb; } }


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
        ManageDirectionIndicator();
    }


    //-----------------------------


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

    void ManageDirectionIndicator()
    {
        if (!isAiming) { directionIndicator.enabled = false; return; }

        directionIndicator.enabled = true;
        directionIndicator.SetPosition(0, transform.position);
        directionIndicator.SetPosition(1, (secondTouchPosition - firstTouchPosition).normalized * 3 + (Vector2)transform.position);
    }

    void InitializeVariables()
    {
        rb = GetComponent<Rigidbody2D>();

        damage = Upgrade.GetUpgrade("Damage", UpgradeData.VariableType.Float);
        staminaRecharge = Upgrade.GetUpgrade("Stamina Recharge", UpgradeData.VariableType.Float);

        stamina = maxStamina;
        staminaSlider.maxValue = maxStamina;
    }

    void RechargeStamina()
    {
        stamina += staminaRecharge * Time.deltaTime;
        if (stamina > maxStamina) stamina = maxStamina;
        staminaSlider.value = stamina;
    }

    public void SetPositionsAndDash(bool pcControls = false)
    {
        if (pcControls == true ? Input.GetMouseButton(0) : Input.touchCount > 0)
        {
            OnAiming?.Invoke();

            if (!isAiming)
            {
                isAiming = true;
                firstTouchPosition = pcControls == true ? (Vector2)Input.mousePosition : Input.GetTouch(0).position;
            }
            else
            {
                secondTouchPosition = pcControls == true ? (Vector2)Input.mousePosition : Input.GetTouch(0).position;

                cameraTarget.position = (secondTouchPosition - firstTouchPosition).normalized * dashDistance / 2 + (Vector2)transform.position;

                if ((secondTouchPosition - firstTouchPosition).normalized != Vector2.zero) transform.rotation = MikeTransform.Rotation.LookTwards(transform.position, (secondTouchPosition - firstTouchPosition).normalized + (Vector2)transform.position);
           }
        }
        else if(isAiming)
        {
            isAiming = false;
            directionIndicator.enabled = false;
            UseDash(dashSpeed, dashDistance);
        }
    }

    void UseDash(float speed, float distance)//checks if player has enough stamina and prevents from using the dash multiple times
    {
        Time.timeScale = 1;
        if (stamina < staminaDrain) 
        {
            staminaSlider.fillRect.GetComponent<Image>().StartColorTransion
            (
                new Color(1,1,1,.2f),
                .1f,
                () => 
                { 
                    staminaSlider.fillRect.GetComponent<Image>().StartColorTransion
                    (
                        new Color(1, 1, 1, 1f),
                        .1f,
                        () =>
                        {
                            staminaSlider.fillRect.GetComponent<Image>().StartColorTransion
                            (
                                new Color(1, 1, 1, .2f),
                                .1f, 
                                () =>
                                {
                                    staminaSlider.fillRect.GetComponent<Image>().StartColorTransion
                                    (
                                        new Color(1, 1, 1, 1f),
                                        .3f
                                    );
                                }
                            );
                        }
                    ); 
                }
            );

            return; 
        }
        if(currentDash != null) StopCoroutine(currentDash);

        currentDash = StartCoroutine(StartDash(speed, distance));
    }

    Vector2 dashTargetPosition;
    Vector2 dashStartPosition;
    IEnumerator StartDash(float speed, float distance)
    {
        cameraTarget.localPosition = Vector3.zero;
        Vector2 truePosition = transform.position;
        dashStartPosition = transform.position;

        stamina -= staminaDrain;
        dashTargetPosition = (secondTouchPosition - firstTouchPosition).normalized * distance + Rb.position;
        Rb.MoveRotation(MikeTransform.Rotation.LookTwards(Rb.position, dashTargetPosition));

        while (true)
        {
            truePosition = Vector2.MoveTowards(truePosition, dashTargetPosition, speed * Time.deltaTime);
            Rb.MovePosition(truePosition);
            if (Rb.position == dashTargetPosition) { currentDash = null; break; }

            yield return null;
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision == null) { return; }

        if(collision.transform.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<Health>().TakeDamage(damage, gameObject);
            CameraShaker.Instance.ShakeOnce(1, 3, .1f, .1f);
        }
        else if (collision.transform.CompareTag("Barrier"))
        {
            float distanceLeft = dashDistance - Vector2.Distance(dashStartPosition, collision.GetContact(0).point);
            dashTargetPosition = collision.GetContact(0).point - Vector2.Reflect(((Vector2)transform.position - dashTargetPosition).normalized, collision.GetContact(0).normal) * distanceLeft;
            rb.rotation = (MikeRotation.Vector2ToAngle(rb.position - dashTargetPosition) + 180);
        }
    }
}