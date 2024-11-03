using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FpsCapper : MonoBehaviour
{
    private void Start()
    {
        UpdateFPS();
    }

    public TMP_Dropdown dropdown;

    public List<int> frames;

    public void UpdateFPS()
    {
        Application.targetFrameRate = frames[(int)dropdown.value];
    }
}
