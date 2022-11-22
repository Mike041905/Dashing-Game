using UnityEngine;

public class Follow : MonoBehaviour
{
    [Header("Essential")]
    [SerializeField] private Transform _target;

    [Header("Options")]
    [SerializeField] private bool _smoothFollow = true;
    [SerializeField] private bool _useUnscaledTime = true;
    [SerializeField] private bool _useFixedUpdate = false;

    [SerializeField] private float _minSpeed = 1;
    [SerializeField] private float _speedDistanceExponent = 0;
    [SerializeField] private float _speedDistanceMultiplier = 1;

    private Rigidbody2D _rb;
    public Rigidbody2D Rb
    {
        get 
        { 
            if(_rb == null) 
            { 
                if(TryGetComponent(out Rigidbody2D rb))
                {
                    _rb = rb;
                }
                else
                {
                    _rb = gameObject.AddComponent<Rigidbody2D>();
                    Debug.LogWarning($"Rigidbody2D not found on {name}! Adding Rigidbody2D.");
                }
            } 

            return _rb;
        }
    }


    private void Update()
    {
        if (_useFixedUpdate) { return; }

        if (_smoothFollow)
        {
            float dist = Vector2.Distance(transform.position, _target.position) * _speedDistanceMultiplier;
            float speed = (_minSpeed + dist * Mathf.Pow(dist, _speedDistanceExponent));

            transform.position = Vector2.MoveTowards(transform.position, _target.position, speed * (_useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime));
        }
        else
        {
            transform.position = _target.position;
        }
    }

    private void FixedUpdate()
    {
        if (!_useFixedUpdate) { return; }

        if (_smoothFollow)
        {
            float dist = Vector2.Distance(Rb.position, _target.position);
            float speed = _minSpeed + dist * Mathf.Pow(dist, _speedDistanceExponent) * _speedDistanceMultiplier;

            Rb.MovePosition(Vector2.MoveTowards(transform.position, _target.position, speed * (_useUnscaledTime ? Time.fixedUnscaledTime : Time.fixedDeltaTime)));
        }
        else
        {
            Rb.MovePosition(_target.position);
        }
    }
}
