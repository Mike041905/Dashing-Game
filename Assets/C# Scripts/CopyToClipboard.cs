using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyToClipboard : MonoBehaviour
{
    public void Copy(string str)
    {
        GUIUtility.systemCopyBuffer = str;
    }

    public void CopyVersion()
    {
        GUIUtility.systemCopyBuffer = Application.version;
    }
}
