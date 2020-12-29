using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIScore : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    // Start is called before the first frame update
    
    // Update is called once per frame
    void Update()
    {
        scoreText.text = "Score\n" + GlobalManager.Instance.point;
    }
}
