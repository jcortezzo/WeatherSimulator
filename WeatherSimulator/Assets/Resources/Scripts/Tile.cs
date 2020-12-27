using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour, Ticable
{
    private ISet<Piece> pieces;
    [SerializeField] private TileType type;
    [SerializeField] private TileEffect effect;
    public Vector2Int position;
    private bool selected;
    private GameObject selection;
    private SpriteRenderer sr;
    private Dictionary<TileType, Sprite> tileSprites;
    public Vector2Int tornadoDir;

    private void Awake()
    {
        pieces = new HashSet<Piece>();
        type = TileType.DEFAULT;
        tornadoDir = Vector2Int.zero;
        tileSprites = new Dictionary<TileType, Sprite>
        {
            { TileType.DEFAULT, Resources.Load<Sprite>("Sprites/Grass")},
            { TileType.HOT, Resources.Load<Sprite>("Sprites/Grass")}, // TODO
            { TileType.ICE, Resources.Load<Sprite>("Sprites/Ice")},
            { TileType.WATER, Resources.Load<Sprite>("Sprites/Water")},
        };
    }

    // Start is called before the first frame update
    void Start()
    {
        selection = transform.Find("Selection").gameObject;
        selection.transform.position = new Vector3(selection.transform.position.x,
                                                   selection.transform.position.y,
                                                   this.transform.position.z - 1);
        selected = false;
        sr = GetComponent<SpriteRenderer>();
        sr.sortingLayerName = "Ground";
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
            //Debug.Log("piece added to tile");
            pieces.Add(p);
            //p.tilemap.Add(this);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Piece p = collision.GetComponent<Piece>();
        if (p != null)
        {
            //Debug.Log("piece added to tile");
            pieces.Add(p);
            //p.tilemap.Add(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Piece p = collision.GetComponent<Piece>();
        if (p != null)
        {
            pieces.Remove(p);
            //p.tilemap.Remove(this);
        }
    }

    public void Tic()
    {
        //Debug.Log(type);
        //ApplyEffect(this.type, this.effect);
    }

    public void ChangeType(Weather weather)
    {
        if (weather == Weather.LIGHTNING)
        {
            this.effect = TileEffect.ELECTRIC;
            //Debug.Log("Lightning Tile");
            GetComponent<SpriteRenderer>().color = Color.blue;
        } else if (weather == Weather.RAIN)
        {
            sr.sprite = tileSprites[TileType.WATER];
        } else if(weather == Weather.SNOW)
        {
            sr.sprite = tileSprites[TileType.ICE];
        } else if(weather == Weather.SUN)
        {
            sr.sprite = tileSprites[TileType.HOT];
        }
        //ApplyEffect(this.type, this.effect);
    }

    public (TileType type, TileEffect effect, Vector2Int position) DescribeTile()
    {
        return (type, effect, position);
    }
}
