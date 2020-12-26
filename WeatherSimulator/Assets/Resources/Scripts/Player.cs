using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Weather selectedWeather;

    // Start is called before the first frame update
    void Start()
    {
        selectedWeather = (Weather) 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedWeather = (Weather) 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedWeather = (Weather) 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectedWeather = (Weather) 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            selectedWeather = (Weather) 3;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (selectedWeather == Weather.LIGHTNING)
            {

            }
        }
    }

    private enum Weather
    {
        LIGHTNING,
        RAIN,
        SUN,
        SNOW,
    }
}
