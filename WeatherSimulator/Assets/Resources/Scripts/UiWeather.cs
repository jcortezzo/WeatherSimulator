using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiWeather : MonoBehaviour
{
    public bool selected;
    [SerializeField] private Weather weather;

    [SerializeField] private Sprite selectedSprite;
    [SerializeField] private Sprite unselectedSprite;
    [SerializeField] private Image childImage;
    private Image image;

    void Start()
    {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        image.color = selected ? Color.yellow : Color.white;
        childImage.sprite = selected ? selectedSprite : unselectedSprite;
    }


}
