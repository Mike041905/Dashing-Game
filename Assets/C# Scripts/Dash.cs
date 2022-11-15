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
    [SerializeField] private LineRenderer _directionIndicator;
    [SerializeField] private Slider _staminaSlider;
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private Transform _cameraTarget;

    [Header("Stats")]
    public float Damage = 1;
    public float DashSpeed = 1;
    public float DashDistance = 1;
    public float StaminaDrain = 1;
    public float MaxStamina = 10;
    public float StaminaRecharge = 1;

    [Header("Options")]
    [SerializeField] private bool _usePcControls = false;


    //-----------------------------


    public event UnityAction OnStartAiming;
    public event UnityAction OnAiming;
    public event UnityAction OnEndAiming;

    public event UnityAction OnStartDash;
    public event UnityAction OnEndDash;

    public event UnityAction<GameObject> OnHitEnemy;

    private bool _isAiming = false;
    private Vector2 _firstTouchPosition;
    private Vector2 _secondTouchPosition;
    private float _stamina = 0;

    public Coroutine CurrentDash;

    Rigidbody2D _rb;
    Rigidbody2D Rb { get { if (_rb == null) { _rb = GetComponent<Rigidbody2D>(); } return _rb; } }


    //-----------------------------

    private void Awake()
    {
        InitializeVariables();
    }

    void Update()
    {
        SetPositionsAndDash(_usePcControls);
        RechargeStamina();

        ManageLineRenderer();
        ManageDirectionIndicator();
    }


    //-----------------------------


    void ManageLineRenderer()
    {
        if(_lineRenderer == null) { return; }

        Vector2 pos1 = Camera.main.ScreenToWorldPoint(_firstTouchPosition);
        Vector2 pos2 = Camera.main.ScreenToWorldPoint(_secondTouchPosition);

        if(Input.touchCount > 0 || Input.GetMouseButton(0))
        {
            _lineRenderer.enabled = true;

            _lineRenderer.SetPositions(new Vector3[2] { pos1, pos2 });
        }
        else
        {
            _lineRenderer.enabled = false;
        }
    }

    void ManageDirectionIndicator()
    {
        if (!_isAiming) { _directionIndicator.enabled = false; return; }

        _directionIndicator.enabled = true;
        _directionIndicator.SetPosition(0, transform.position);
        _directionIndicator.SetPosition(1, (_secondTouchPosition - _firstTouchPosition).normalized * 3 + (Vector2)transform.position);
    }

    void InitializeVariables()
    {
        _rb = GetComponent<Rigidbody2D>();

        Damage *= Upgrade.GetUpgrade("Damage", UpgradeData.VariableType.Float);
        StaminaRecharge *= Upgrade.GetUpgrade("Stamina Recharge", UpgradeData.VariableType.Float);

        _stamina = MaxStamina;
        _staminaSlider.maxValue = MaxStamina;
    }

    void RechargeStamina()
    {
        _stamina += StaminaRecharge * Time.deltaTime;
        if (_stamina > MaxStamina) _stamina = MaxStamina;
        _staminaSlider.value = _stamina;
    }

    public void SetPositionsAndDash(bool pcControls = false)
    {
        if (Input.GetMouseButton(0)) { pcControls = true; }

        if (Input.GetMouseButton(0) || Input.touchCount > 0)
        {
            Vector2 touchPos = pcControls ? (Vector2)Input.mousePosition : Input.GetTouch(0).position;

            OnAiming?.Invoke();

            if (!_isAiming)
            {
                _isAiming = true;
                _firstTouchPosition = touchPos;
            }
            else
            {
                _secondTouchPosition = touchPos;

                _cameraTarget.position = (_secondTouchPosition - _firstTouchPosition).normalized * DashDistance / 2 + (Vector2)transform.position;

                if ((_secondTouchPosition - _firstTouchPosition).normalized != Vector2.zero) transform.rotation = MikeTransform.Rotation.LookTwards(transform.position, (_secondTouchPosition - _firstTouchPosition).normalized + (Vector2)transform.position);
           }
        }
        else if(_isAiming)
        {
            _isAiming = false;
            _directionIndicator.enabled = false;
            UseDash(DashSpeed, DashDistance);
        }
    }

    void UseDash(float speed, float distance)//checks if player has enough stamina and prevents from using the dash multiple times
    {
        Time.timeScale = 1;
        if (_stamina < StaminaDrain) 
        {
            _staminaSlider.fillRect.GetComponent<Image>().StartColorTransion
            (
                new Color(1,1,1,.2f),
                .1f,
                () => 
                { 
                    _staminaSlider.fillRect.GetComponent<Image>().StartColorTransion
                    (
                        new Color(1, 1, 1, 1f),
                        .1f,
                        () =>
                        {
                            _staminaSlider.fillRect.GetComponent<Image>().StartColorTransion
                            (
                                new Color(1, 1, 1, .2f),
                                .1f, 
                                () =>
                                {
                                    _staminaSlider.fillRect.GetComponent<Image>().StartColorTransion
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
        if(CurrentDash != null) StopCoroutine(CurrentDash);

        CurrentDash = StartCoroutine(StartDash(speed, distance));
    }

    Vector2 _dashTargetPosition;
    Vector2 _dashStartPosition;
    IEnumerator StartDash(float speed, float distance)
    {
        _cameraTarget.localPosition = Vector3.zero;
        _dashStartPosition = transform.position;

        _stamina -= StaminaDrain;
        _dashTargetPosition = (_secondTouchPosition - _firstTouchPosition).normalized * distance + Rb.position;
        Rb.MoveRotation(MikeTransform.Rotation.LookTwards(Rb.position, _dashTargetPosition));

        while (Rb.position != _dashTargetPosition)
        {
            Rb.MovePosition(Vector2.MoveTowards(Rb.position, _dashTargetPosition, speed * Time.fixedDeltaTime));

            yield return new WaitForFixedUpdate();
        }

        CurrentDash = null;
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
        _cameraTarget.position = transform.position;
        _lineRenderer.enabled = false;
        _directionIndicator.enabled = false;
        if (CurrentDash != null) { StopCoroutine(CurrentDash); }
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

        if(collision.gameObject.TryGetComponent(out EnemyAI enemy))
        {
            collision.gameObject.GetComponent<Health>().TakeDamage(Damage, gameObject);

            if(enemy is BossAI)
            {
                Bounce(collision);
            }
            else
            {
                CameraShaker.Instance.ShakeOnce(1, 4, .01f, .2f);
                HapticFeedback.Vibrate(100);
            }

            OnHitEnemy?.Invoke(collision.gameObject);
        }
        else
        {
            Bounce(collision);
        }

    }

    void Bounce(Collision2D collision)
    {
        float distanceLeft = DashDistance - Vector2.Distance(_dashStartPosition, collision.GetContact(0).point);
        _dashTargetPosition = collision.GetContact(0).point - Vector2.Reflect(((Vector2)transform.position - _dashTargetPosition).normalized, collision.GetContact(0).normal) * distanceLeft;
        _rb.rotation = (MikeRotation.Vector2ToAngle(_rb.position - _dashTargetPosition) + 180);

        CameraShaker.Instance.ShakeOnce(1, 4, .01f, .2f);
        HapticFeedback.Vibrate(100);
    }
}