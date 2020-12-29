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
    private GameObject fire;
    private GameObject tornado;

    private SpriteRenderer sr;
    private Dictionary<TileType, Sprite> tileSprites;
    public Vector2Int? tornadoDir;

    public int typeResetTic;
    public int effectResetTic;

    private Animator waterAnim;

    private void Awake()
    {
        pieces = new HashSet<Piece>();
        type = TileType.DEFAULT;
        tornadoDir = Vector2Int.zero;
        tileSprites = new Dictionary<TileType, Sprite>
        {
            { TileType.DEFAULT, Resources.Load<Sprite>("Sprites/Grass")},
            { TileType.HOT, Resources.Load<Sprite>("Sprites/SunTile")},
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

        fire = transform.Find("Fire").gameObject;
        fire.SetActive(effect == TileEffect.FIRE);

        tornado = transform.Find("Tornado").gameObject;
        tornado.SetActive(effect == TileEffect.TORNADO);

        waterAnim = GetComponent<Animator>();
        waterAnim.enabled = (type == TileType.WATER);

        selected = false;
        sr = GetComponent<SpriteRenderer>();
        sr.sortingLayerName = "Ground";
    }

    // Update is called once per frame
    void Update()
    {
        selection.gameObject.SetActive(selected);
        electric.SetActive(effect == TileEffect.ELECTRIC);
        fire.SetActive(effect == TileEffect.FIRE);
        tornado.SetActive(effect == TileEffect.TORNADO);
        tornadoDir = tornadoDir != null ? tornadoDir : null;
        waterAnim.enabled = (type == TileType.WATER);
        CheckInteractions();
    }

    private void CheckInteractions()
    {
        if (type == TileType.ICE)
        {
            if (effect == TileEffect.FIRE)
            {
                ChangeType(Weather.RAIN);
                effect = TileEffect.FIRE;
                effectResetTic = 1;
            }
        }
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
        if(typeResetTic > 0) typeResetTic--;
        if (effectResetTic > 0) effectResetTic--;
        if (typeResetTic == 0) ChangeType(Weather.NONE);
        if (effectResetTic == 0) effect = TileEffect.NONE;
    }

    public void ChangeType(Weather weather)
    {
        if (weather == Weather.LIGHTNING)
        {
            this.effect = TileEffect.ELECTRIC;
            effectResetTic = 5;
            ISet<Tile> neighbors = GlobalManager.Instance.GameBoard.GetNeighbors(this);
            foreach (Tile t in neighbors)
            {
                var info = t.DescribeTile();
                if (info.type == TileType.WATER &&
                    info.effect != TileEffect.ELECTRIC)
                {
                    t.ChangeType(Weather.LIGHTNING);
                }
                else if (info.type == TileType.HOT &&
                         info.effect != TileEffect.FIRE)
                {
                    if (Random.Range(0f, 1f) <= 0.5f)
                    {
                        t.ChangeType(Weather.SUN);
                        t.effect = TileEffect.FIRE;
                        t.effectResetTic = 5;
                    }
                } else if (info.type != TileType.WATER)
                {
                    if (Random.Range(0f, 1f) <= 0.05f)
                    {
                        t.effect = TileEffect.FIRE;
                        t.effectResetTic = 5;
                        Jukebox.Instance.PlaySFX("Fire", 1f, 1f);
                    }
                }
            }
        }
        else if (weather == Weather.RAIN)
        {
            type = TileType.WATER;
            sr.sprite = tileSprites[TileType.WATER];
            typeResetTic = -1;
            ISet<Tile> neighbors = GlobalManager.Instance.GameBoard.GetNeighbors(this);
            foreach (Tile t in neighbors)
            {
                var info = t.DescribeTile();
                if (t.type == TileType.WATER && t.effect == TileEffect.ELECTRIC)
                {
                    this.effect = TileEffect.ELECTRIC;
                    this.effectResetTic = t.effectResetTic;
                }
            }
            Jukebox.Instance.PlaySFX("Rain", .25f, 1f);
        }
        else if(weather == Weather.SNOW)
        {
            type = TileType.ICE;
            sr.sprite = tileSprites[TileType.ICE];
            typeResetTic = -1;
            Jukebox.Instance.PlaySFX("Blizzard", .25f, 1f);
            GenerateTornados(TileType.HOT);
        }
        else if(weather == Weather.SUN)
        {
            type = TileType.HOT;
            sr.sprite = tileSprites[TileType.HOT];
            typeResetTic = 10;

            GenerateTornados(TileType.ICE);
        }
        else
        {
            //Debug.Log("reset tile");
            type = TileType.DEFAULT;
            //effect = TileEffect.NONE;  no ty
            sr.sprite = tileSprites[TileType.DEFAULT];
        }
        //ApplyEffect(this.type, this.effect);
    }

    private void GenerateTornados(TileType desiredType)
    {
        Vector2Int coords = GlobalManager.Instance.GameBoard.GetCoordsFromTile(this);
        if (coords != null)
        {
            Tile up = GlobalManager.Instance.GameBoard.GetTile(Vector2.up + coords);
            Tile down = GlobalManager.Instance.GameBoard.GetTile(Vector2.down + coords);
            Tile left = GlobalManager.Instance.GameBoard.GetTile(Vector2.left + coords);
            Tile right = GlobalManager.Instance.GameBoard.GetTile(Vector2.right + coords);

            Tile up2 = GlobalManager.Instance.GameBoard.GetTile(Vector2.up * 2 + coords);
            Tile down2 = GlobalManager.Instance.GameBoard.GetTile(Vector2.down * 2 + coords);
            Tile left2 = GlobalManager.Instance.GameBoard.GetTile(Vector2.left * 2 + coords);
            Tile right2 = GlobalManager.Instance.GameBoard.GetTile(Vector2.right * 2 + coords);

            IList<Tile> first = new List<Tile>() { up, down, left, right };
            IList<Tile> second = new List<Tile>() { up2, down2, left2, right2 };
            IDictionary<Tile, Tile> tiles = new Dictionary<Tile, Tile>(); ;
            for (int i = 0; i < first.Count; i++)
            {
                if (first[i] == null || second[i] == null)
                {
                    continue;
                }
                else
                {
                    tiles[second[i]] = first[i];
                }
            }

            // = new Dictionary<Tile, Tile>() { { up2, up }, { down2, down }, { left2, left }, { right2, right }, };
            foreach (Tile t in tiles.Keys)
            {
                if (t == null) continue;

                var info = t.DescribeTile();
                if (t.type == desiredType)
                {
                    if (tiles[t] == null) continue;
                    tiles[t].effect = TileEffect.TORNADO;
                    tiles[t].effectResetTic = 5;
                    // up / down / left / right (inverse of line 224-227)
                    tiles[t].tornadoDir = GlobalManager.Instance.GameBoard.GetCoordsFromTile(tiles[t]) - coords;
                    if (desiredType == TileType.ICE)
                    {
                        tiles[t].tornadoDir *= -1;
                    }
                }
            }
        }
    }

    public (TileType type, TileEffect effect, Vector2Int position) DescribeTile()
    {
        return (type, effect, position);
    }
}
