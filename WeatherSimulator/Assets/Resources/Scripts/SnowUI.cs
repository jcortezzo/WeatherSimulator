using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SunUI : MonoBehaviour
{
    Image i;

    // Start is called before the first frame update
    void Start()
    {
        i = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        i.fillAmount = 1f - (float)GlobalManager.Instance.player.scd / 1;
    }
}