using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject _pauseMenuHolder;
    bool _pause = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) { Pause(); }
    }

    public void Pause()
    {
        _pause = !_pause;
        ForcePauseUpdate();
    }

    public void ForcePauseUpdate() => ForcePauseUpdate(_pause);

    public void ForcePauseUpdate(bool pauseState)
    {
        Time.timeScale = pauseState ? 0 : 1;
        _pauseMenuHolder.SetActive(pauseState);
        Player.Instance.PlayerDash.enabled = !pauseState;
    }
}
