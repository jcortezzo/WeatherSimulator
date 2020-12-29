using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiEffectSelect : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private UiWeather[] uiWeathers;
    private GameObject pauseScreen;

    // Start is called before the first frame update
    void Start()
    {
        pauseScreen = transform.Find("PauseScreen").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        SelectWeatherUI(player.selectedWeather);
        pauseScreen.SetActive(GlobalManager.Instance.IsPaused());
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
