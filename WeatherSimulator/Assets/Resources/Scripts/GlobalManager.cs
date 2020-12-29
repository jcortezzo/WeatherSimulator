using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalManager : MonoBehaviour
{
    public static GlobalManager Instance;
    public float globalTimer;
    public const int GAME_SCALE = 2;
    public int BOARD_SIZE;
    public GameBoard GameBoard { get { return gameBoard; } }
    public int enemyNumbers;
    public GameObject chest;

    [SerializeField] public float TIC_TIME = 2f;
    [SerializeField] private GameBoard gameBoard;
    [SerializeField] private Piece piece;
    [SerializeField] private Piece mainPiece;

    [SerializeField] private bool isPaused;

    [SerializeField] private Vector2Int[] enemyLocations;

    [SerializeField] private GameObject treasureGo;

    public Camera cam;

    [SerializeField] private GameObject youLose;
    private bool hasLost;

    public Player player;

    public Queue<Piece> toBeExequeue;
    private void Awake()
    {
        globalTimer = 0;
        //if (Instance == null)
        //{
            Instance = this;
        //}
        //else
        //{
            //Destroy(this.gameObject);
            //return;
        //}
        //DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        Load();
        toBeExequeue = new Queue<Piece>();
    }

    public void Load()
    {   
        cam = Camera.main;
        gameBoard = Instantiate(gameBoard.transform).GetComponent<GameBoard>();

        gameBoard.SpawnPlayerPiece(new Vector2Int(8, 8), mainPiece);

        foreach (Vector2Int location in enemyLocations)
        {
            gameBoard.SpawnEnemy(location, piece);
        }
        Vector3 centerPos = gameBoard.GetCenterTile().transform.position;
        cam.transform.position = new Vector3(centerPos.x, centerPos.y, -10);

        isPaused = false;

        player = FindObjectOfType<Player>();

    }
    //private void OnLevelWasLoaded(int level)
    //{
    //    Start();
    //}

    // Update is called once per frame
    void Update()
    {

        if (hasLost)
        {
            if (Input.GetMouseButtonDown(0))
            {
                //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                hasLost = false;
                SceneManager.LoadScene("Abdul2Scene");
            }
        }

        if (gameBoard.playerPiece == null && !hasLost)
        {
            GameObject lost = Instantiate(youLose, gameBoard.GetCenterTile().transform.position, Quaternion.identity);
            lost.GetComponent<Renderer>().sortingLayerName = "Lightning";
            hasLost = true;
            return;
        }

        // Don't tic if the game is paused
        if (IsPaused())
        {
            return;
        }
        globalTimer += Time.deltaTime;
        if (globalTimer >= TIC_TIME)
        {
            globalTimer = 0f;
            gameBoard.Tic();
            if(gameBoard.enemyLocations.Count< enemyNumbers)
            {
                gameBoard.SpawnEnemy(new Vector2Int(Random.Range(0, BOARD_SIZE), Random.Range(0, BOARD_SIZE)), piece);
            }
        }

        foreach(Piece p in toBeExequeue)
        {
            GTFO(p);
        }
        
    }

    public void CreateTreasure(Vector2 pos)
    {
        chest = Instantiate(treasureGo, pos, Quaternion.identity);
    }

    public void Pause()
    {
        isPaused = true;
        Jukebox.Instance.PlaySFX("Menu Change", 0.5f, 1f);
    }

    public void Unpause()
    {
        isPaused = false;
        Jukebox.Instance.PlaySFX("Menu Change", 0.5f, 1f);
    }

    public bool IsPaused()
    {
        return isPaused;
    }

    // Returns the Vec3 world position of a given board location
    public Vector3 GetWorldPos(Vector2Int tilePos)
    {
        Tile tile = GameBoard.GetTile(tilePos.x, tilePos.y);
        return tile != null ? GameBoard.GetTile(tilePos.x, tilePos.y).transform.position : Vector3.negativeInfinity;
    }

    public void AddToDeadQueue(Piece p)
    {
        toBeExequeue.Enqueue(p);
    }

    public IEnumerator GTFO(Piece p, float duration = float.NegativeInfinity)
    {
        p.GetComponent<Collider2D>().enabled = false;

        Vector2 vec = new Vector2(Random.Range(0, 1), Random.Range(0, 1));
        Vector3 dir = vec.normalized * 10;

        Vector2 newPos = dir + p.transform.position;

        int numSteps = 20; // arbirtary, this is smooth though!
        float stepLength = duration / numSteps; // time leng of step

        for (float i = 0; i < 1; i += (1f / numSteps))
        {
            // We can apply a sinuosoid to this as well!
            this.transform.position = Vector2.Lerp(p.transform.position, newPos, 0.005f);
            yield return new WaitForSeconds(stepLength);
        }

        KillPiece(p.gameObject, 1f);
    }

    public void KillPiece(GameObject go, float afterSec = 0.0f)
    {
        StartCoroutine(Kill(go, afterSec));
    }

    private IEnumerator Kill(GameObject go, float afterSec)
    {
        yield return new WaitForSeconds(afterSec);
        Jukebox.Instance.PlaySFX("Enemy Death", 0.5f, 1f);
        Destroy(go);
    }
}
