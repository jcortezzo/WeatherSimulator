using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasure : MonoBehaviour
{
    [SerializeField] private GameObject floatingText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        MainPiece mp = collision.gameObject.GetComponent<MainPiece>();
        if (mp != null)
        {
            ShowFloatingText();
        }
    }

    private void ShowFloatingText()
    {
        GameObject floating = Instantiate(floatingText, this.transform.position, Quaternion.identity);
        floating.GetComponent<Renderer>().sortingLayerName = "Lightning";
        Jukebox.Instance.PlaySFX("Treasure Ding", 0.75f, 1f);
        Destroy(floating, 2f);
        Destroy(this.gameObject);
    }

    private void OnDestroy()
    {
        GlobalManager.Instance.point += 100;
    }
}
