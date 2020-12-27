using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiWeather : MonoBehaviour
{
    [SerializeField] private Weather weather;
    [SerializeField] public bool selected;

    private Image image;

    void Start()
    {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        image.color = selected ? Color.yellow : Color.white;

    }


}
