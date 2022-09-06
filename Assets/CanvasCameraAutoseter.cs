using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasCameraAutoseter : MonoBehaviour
{
    private void Start()
    {
        SceneManager.activeSceneChanged += OnSceneChange;
    }

    void OnSceneChange(Scene s1, Scene s2)
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChange;

    }
}
