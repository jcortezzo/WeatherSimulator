using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    ISet<Tile> tilemap;

    // Start is called before the first frame update
    void Start()
    {
        tilemap = new HashSet<Tile>();    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Tile t = collision.GetComponent<Tile>();
        if (t != null)
        {
            tilemap.Add(t);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Tile t = collision.GetComponent<Tile>();
        if (t != null)
        {
            tilemap.Remove(t);
        }
    }
}
