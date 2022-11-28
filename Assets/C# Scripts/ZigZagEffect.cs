using UnityEngine;


public class ZigZagEffect : MonoBehaviour
{
    [SerializeField] private float _maxRadius = 1;
    [SerializeField] private float _minSpeed = 3;
    [SerializeField] private float _maxSpeed = 5;

    private float _currentSpeed = 5;
    private Vector2 _currentPos;

    private void Start()
    {
        GetNewSpeedAndPos();
    }

    private void FixedUpdate()
    {
        transform.localPosition = Vector2.MoveTowards(transform.localPosition, _currentPos, _currentSpeed * Time.fixedDeltaTime);

        if ((Vector2)transform.localPosition == _currentPos) { GetNewSpeedAndPos(); }
    }

    private void GetNewSpeedAndPos()
    {
        _currentSpeed = Random.Range(_minSpeed, _maxSpeed);
        _currentPos = Random.insideUnitCircle * Random.Range(0, _maxRadius);
    }
}
