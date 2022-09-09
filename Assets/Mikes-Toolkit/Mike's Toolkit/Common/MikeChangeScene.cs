using UnityEngine;
using UnityEngine.SceneManagement;

public class MikeChangeScene : MonoBehaviour
{
    public void ChangeScene(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }

    public void ChangeScene(string name)
    {
        SceneManager.LoadScene(name);
    }
}
