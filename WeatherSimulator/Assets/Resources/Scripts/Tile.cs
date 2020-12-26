using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour, Ticable
{
    private ISet<Piece> pieces;
    [SerializeField] private TileType type;

    private void Awake()
    {
        pieces = new HashSet<Piece>();
        type = TileType.DEFAULT;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Piece p = collision.GetComponent<Piece>();
        if (p != null)
        {
            pieces.Add(p);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Piece p = collision.GetComponent<Piece>();
        if (p != null)
        {
            pieces.Remove(p);
        }
    }

    public void Tic()
    {
        Debug.Log(type);
    }

    // TODO: separate effects like tornados from base tiles like default
    private enum TileType
    {
        DEFAULT,
        WATER,
        HOT,
        FIRE,
        ICE,
        LIGHTNING,
        ELECTRIC_WATER,
        TORNADO,
    }
}
