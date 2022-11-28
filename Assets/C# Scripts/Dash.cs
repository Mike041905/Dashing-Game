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
    [SerializeField] private GameObject _hitFX;
    // NOTE: _spriteTransform is used for a workaround
    // Issue: While rotating a moving rigidbody physics break
    // Workaround: Rotate sprite renderer (Collider must be a circle)

    [Header("Stats")]
    public float Damage = 1;
    public float DashSpeed = 1;
    public float DashDistance = 1;
    public float StaminaDrain = 1;
    public float MaxStamina = 10;
    public float StaminaRecharge = 1;
    public float KnockbackMultiplier = 1;

    [Header("Options")]
    [SerializeField] private bool _usePcControls = false;
    [SerializeField] public bool UseBounce = true;
    [SerializeField] float _shakeMagintudeHit = 1;
    [SerializeField] float _shakeRoughnessHit = 4;
    [SerializeField] float _shakeInTimeHit = .1f;
    [SerializeField] float _shakeOutTimeHit = .3f;
    [SerializeField] float _vibrateTime = .3f;


    //-----------------------------


    public event UnityAction OnStartAiming;
    public event UnityAction OnAiming;
    public event UnityAction OnEndAiming;

    public event UnityAction OnStartDash;
    public event UnityAction OnEndDash;

    public event UnityAction<EnemyAI> OnHitEnemy;
    public event UnityAction<EnemyAI> OnEnemyKilled;

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

        if (Input.touchCount > 0 || Input.GetMouseButton(0))
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

                if ((_secondTouchPosition - _firstTouchPosition).magnitude > .1f) Player.Instance.SpriteRenderer.transform.rotation = (MikeTransform.Rotation.LookTwards(transform.position, (_secondTouchPosition - _firstTouchPosition).normalized + (Vector2)transform.position));
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
            DoStaminDepletedAnimation();

            return;
        }
        if (CurrentDash != null) StopCoroutine(CurrentDash);

        CurrentDash = StartCoroutine(StartDash(speed, distance));

        void DoStaminDepletedAnimation()
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
        }
    }

    Vector2 _dashTargetPosition;
    Vector2 _dashStartPosition;
    IEnumerator StartDash(float speed, float distance)
    {
        _cameraTarget.localPosition = Vector3.zero;
        _dashStartPosition = transform.position;

        _stamina -= StaminaDrain;
        _dashTargetPosition = (_secondTouchPosition - _firstTouchPosition).normalized * distance + Rb.position;
        Player.Instance.SpriteRenderer.transform.rotation = (MikeTransform.Rotation.LookTwards(Rb.position, _dashTargetPosition));

        OnStartDash?.Invoke();

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
        SetCommon(false);

        _isAiming = false;

        _cameraTarget.position = transform.position;
        _lineRenderer.enabled = false;
        _directionIndicator.enabled = false;
        if (CurrentDash != null) { StopCoroutine(CurrentDash); }
    }

    public void OnRevive()
    {
        SetCommon(true);
    }

    void SetCommon(bool isAlive)
    {
        enabled = isAlive;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision == null) { return; }

        CameraShaker.Instance.ShakeOnce(_shakeMagintudeHit, _shakeRoughnessHit, _shakeInTimeHit, _shakeOutTimeHit);
        HapticFeedback.Vibrate((int)(_vibrateTime * 1000));

        if (collision.gameObject.TryGetComponent(out Health health))
        {
            health.TryGetComponent(out EnemyAI enemy);

            health.TakeDamage(Damage, gameObject);
            if (enemy != null) { OnHitEnemy?.Invoke(enemy); }

            if(!health.Dead)
            {
                if(_hitFX != null) 
                {
                    ContactPoint2D contact = collision.GetContact(0);
                    Instantiate(_hitFX, contact.point, MikeTransform.Rotation.LookTwards(contact.point, contact.normal)); 
                }
                if(UseBounce) { Bounce(collision); }
            }
            else
            {
                if (enemy != null) { OnEnemyKilled?.Invoke(enemy); }
            }
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
        Player.Instance.SpriteRenderer.transform.rotation = (MikeTransform.Rotation.LookTwards(Rb.position, _dashTargetPosition));
    }
}