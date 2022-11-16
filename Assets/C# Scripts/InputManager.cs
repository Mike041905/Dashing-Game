using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using Mike;
using UnityEngine.Events;

public class InputManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinCounter;
    [SerializeField] private TextMeshProUGUI gemCounter;
    [SerializeField] private TextMeshProUGUI scoreCounter;
    [SerializeField] private TextMeshProUGUI levelCounter;

    static InputManager _instance;
    public static InputManager Instance { get => _instance; }

    private void Awake()
    {
        _instance = this;
    }


    //---------------------------------------

    public event UnityAction OnUpgrade;


    Transform _camTarget;
    Transform CameraTarget { get { if (_camTarget == null) { _camTarget = GameObject.FindGameObjectWithTag("CameraTarget").transform; } return _camTarget; } }


    //---------------------------------------


    private void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }


    //--------------------------------------


    internal void UpdateUI()
    {
        if (coinCounter != null) coinCounter.text = MikeString.ConvertNumberToString(GameManager.Insatnce.Coins);
        if (gemCounter != null) gemCounter.text = MikeString.ConvertNumberToString(GameManager.Insatnce.Gems);
        if (levelCounter != null) levelCounter.text = "Level: " + GameManager.Insatnce.Level.ToString();
    }



    public void LoadScene(string scene)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(scene);
    }

    public void Upgrade()
    {
        OnUpgrade?.Invoke();
    }

    public void SetTimeScale(float scale = 1)
    {
        Time.timeScale = scale;
    }

    public void Retry()
    {
        PlayerPrefs.SetInt("Current Level", PlayerPrefs.GetInt("Starting Level", 1));
        LoadScene("Level");
    }

    public UnityEvent OnOpenMap;
    Coroutine _map = null;
    public void OpenMap()
    {
        if(_map != null) { StopCoroutine(_map); }
        _map = StartCoroutine(SetMap(true));

        OnOpenMap?.Invoke();
    }

    public UnityEvent OnCloseMap;
    public void CloseMap()
    {
        if(_map != null) { StopCoroutine(_map); }
        _map = StartCoroutine(SetMap(false));

        OnCloseMap?.Invoke();
    }

    IEnumerator SetMap(bool open)
    {
        // TODO: Don't hardcode this!
        float targetSize = open ? 150 : 7;
        Vector2? targetPos = open ? Vector2.zero : null;

        while (true)
        {
            if (open)
            {
                Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, 150, Time.deltaTime * 4);
                CameraTarget.position = Vector2.MoveTowards(CameraTarget.position, Vector2.zero, 1 * Time.deltaTime + Time.deltaTime * Vector2.Distance(CameraTarget.position, Vector2.zero));
            }
            else
            {
                Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, 7, Time.deltaTime * 4);
            }

            if(Camera.main.orthographicSize < targetSize + 0.1f && Camera.main.orthographicSize > targetSize - 0.1f)
            {
                Camera.main.orthographicSize = targetSize;

                if(targetPos != null && CameraTarget.position == targetPos)
                {
                    yield break;
                }
                else
                {
                    yield break;
                }
            }

            yield return null;
        }
    }
}
