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
    RectTransform rectTrans;
    private Vector3 ogPos;
    private Vector3 selectPos;
    private GameObject img;
    private Button button;
    [SerializeField] private int selectOffset = 16;

    void Start()
    {
        image = GetComponent<Image>();
        //rectTrans = GetComponent<RectTransform>();
        img = transform.Find("Image").gameObject;
        rectTrans = img.GetComponent<RectTransform>();
        ogPos = rectTrans.localPosition;
        selectPos = new Vector2(ogPos.x + selectOffset, ogPos.y);
        
    }

    // Update is called once per frame
    void Update()
    {
        image.color = selected ? Color.yellow : Color.white;
        childImage.sprite = selected ? selectedSprite : unselectedSprite;
        rectTrans.localPosition = selected ? selectPos : ogPos;
    }

    public void Select()
    {
        GlobalManager.Instance.player.selectedWeather = weather;
    }

}
