using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour, Ticable
{
    private ISet<Piece> pieces;
    [SerializeField] private TileType type;
    [SerializeField] private TileEffect effect;
    private bool selected;
    private GameObject selection;

    private void Awake()
    {
        pieces = new HashSet<Piece>();
        type = TileType.DEFAULT;
    }

    // Start is called before the first frame update
    void Start()
    {
        selection = transform.Find("Selection").gameObject;
        selection.transform.position = new Vector3(selection.transform.position.x,
                                                   selection.transform.position.y,
                                                   this.transform.position.z - 1);
        selected = false;
    }

    // Update is called once per frame
    void Update()
    {
        selection.gameObject.SetActive(selected);
    }

    public void Select()
    {
        selected = true;
    }

    public void Unselect()
    {
        selected = false;
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
        //Debug.Log(type);
        ApplyEffect(this.type, this.effect);
    }

    public void ChangeType(Weather weather)
    {
        if (weather == Weather.LIGHTNING)
        {
            this.effect = TileEffect.ELECTRIC;
            Debug.Log("Lightning Tile");
            GetComponent<SpriteRenderer>().color = Color.blue;
        }
        ApplyEffect(this.type, this.effect);
    }

    private void ApplyEffect(TileType type, TileEffect effect)
    {
        ISet<Piece> deepCopy = new HashSet<Piece>();
        if (effect == TileEffect.ELECTRIC)
        {
            // Shock
            Debug.Log(deepCopy);
            ISet<Piece> toDestroy = new HashSet<Piece>();
            foreach (Piece p in deepCopy)
            {
                Destroy(p.gameObject);
                Debug.Log("piece destroyed by lightning!");
            }
        }
    }

    // TODO: separate effects like tornados from base tiles like default
    private enum TileType
    {
        DEFAULT,
        WATER,
        HOT,
        ICE,
    }

    private enum TileEffect
    {
        ELECTRIC,
        TORNADO,
        FIRE,
    }
}
