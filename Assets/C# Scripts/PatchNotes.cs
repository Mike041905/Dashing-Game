using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatchNotes : MonoBehaviour
{
    [SerializeField] private GameObject updateNotes;

    private void Start()
    {
        ShowUpdateNotes();
    }

    void ShowUpdateNotes()
    {
        if (updateNotes == null) { return; }
        if (Application.version == PlayerPrefs.GetString("Last Version", "")) { return; }
        else { PlayerPrefs.SetString("Last Version", Application.version); PlayerPrefs.SetInt("Seen Update Notes", 0); }
        if (PlayerPrefs.GetInt("New Player", 1) == 1) { PlayerPrefs.SetInt("New Player", 0); return; }
        if (PlayerPrefs.GetInt("Seen Update Notes", 0) == 1) { return; }
        PlayerPrefs.SetInt("Seen Update Notes", 1);
        updateNotes.SetActive(true);
    }
}
