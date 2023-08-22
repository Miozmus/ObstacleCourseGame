using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashBar : MonoBehaviour
{
    public Slider slider;

    [Tooltip("Czas cooldownu umiijêtnoœci.")]
    public int CooldownTime = 100;
    private float StartCooldownTime;
    public bool StartLevel = false;

    public void ResetTime()
    {
        slider.value = 0;
    }

    public void StartdashBar()
    {
        StartLevel = true;
    }

    public void SetMaxCooldown(float seconds)
    {
        slider.maxValue = seconds;
    }

    public void SetCurrentValue(float seconds)
    {
        if(StartLevel)
        slider.value = seconds;
    }


}
