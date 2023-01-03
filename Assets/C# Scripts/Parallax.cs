using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] Material _mat;
    [SerializeField] Vector2 _movementMul = Vector2.one;
    [SerializeField] Transform _parallaxSampler;

    private void LateUpdate()
    {
        _mat.mainTextureOffset = _parallaxSampler.position * _movementMul * _mat.mainTextureScale * .02f;
    }
}
