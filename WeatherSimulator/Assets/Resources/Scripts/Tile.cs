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
    private GameObject electric;

    private SpriteRenderer sr;
    private Dictionary<TileType, Sprite> tileSprites;
    public Vector2Int tornadoDir;

    public int resetTic;

    private void Awake()
    {
        pieces = new HashSet<Piece>();
        type = TileType.DEFAULT;
        tornadoDir = Vector2Int.zero;
        tileSprites = new Dictionary<TileType, Sprite>
        {
            { TileType.DEFAULT, Resources.Load<Sprite>("Sprites/Grass")},
            { TileType.HOT, Resources.Load<Sprite>("Sprites/SunTile")}, // TODO
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
        electric = transform.Find("Electric").gameObject;
        electric.SetActive(effect == TileEffect.ELECTRIC);

        selected = false;
        sr = GetComponent<SpriteRenderer>();
        sr.sortingLayerName = "Ground";
    }

    // Update is called once per frame
    void Update()
    {
        selection.gameObject.SetActive(selected);
        electric.SetActive(effect == TileEffect.ELECTRIC);

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
        if(resetTic > 0) resetTic--;
        if (resetTic <= 0) ChangeType(Weather.NONE);
    }

    public void ChangeType(Weather weather)
    {
        if (weather == Weather.LIGHTNING)
        {
            this.effect = TileEffect.ELECTRIC;
            resetTic = 5;
            ISet<Tile> neighbors = GlobalManager.Instance.GameBoard.GetNeighbors(this);
            foreach (Tile t in neighbors)
            {
                if (t.DescribeTile().type == TileType.WATER &&
                    t.DescribeTile().effect != TileEffect.ELECTRIC)
                {
                    t.ChangeType(Weather.LIGHTNING);
                }
            }
        } else if (weather == Weather.RAIN)
        {
            type = TileType.WATER;
            sr.sprite = tileSprites[TileType.WATER];
            resetTic = 5;
        } else if(weather == Weather.SNOW)
        {
            type = TileType.ICE;
            sr.sprite = tileSprites[TileType.ICE];
            resetTic = 5;
        } else if(weather == Weather.SUN)
        {
            type = TileType.HOT;
            sr.sprite = tileSprites[TileType.HOT];
            resetTic = 5;
        } else
        {
            //Debug.Log("reset tile");
            type = TileType.DEFAULT;
            effect = TileEffect.NONE;
            sr.sprite = tileSprites[TileType.DEFAULT];
        }
        //ApplyEffect(this.type, this.effect);
    }

    public (TileType type, TileEffect effect, Vector2Int position) DescribeTile()
    {
        return (type, effect, position);
    }
}
