using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiEffectSelect : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private UiWeather[] uiWeathers;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        SelectWeatherUI(player.selectedWeather);
    }

    private void SelectWeatherUI(Weather weather)
    {
        foreach(UiWeather ui in uiWeathers)
        {
            ui.selected = false;
        }
        uiWeathers[(int)weather].selected = true;
    }
}
