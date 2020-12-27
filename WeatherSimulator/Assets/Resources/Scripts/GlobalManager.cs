using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalManager : MonoBehaviour
{
    public static GlobalManager Instance;
    public float globalTimer;
    public const int GAME_SCALE = 2;
    public int BOARD_SIZE;
    public GameBoard GameBoard { get { return gameBoard; } }
    public int enemyNumbers;
    [SerializeField] public float TIC_TIME = 2f;
    [SerializeField] private GameBoard gameBoard;
    [SerializeField] private Piece piece;
    [SerializeField] private Piece mainPiece;

    [SerializeField] private Vector2Int[] enemyLocations;

    public Camera cam;

    private void Awake()
    {
        globalTimer = 0;
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
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
    }

    // Update is called once per frame
    void Update()
    {
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
    }

    // Returns the Vec3 world position of a given board location
    public Vector3 GetWorldPos(Vector2Int tilePos)
    {
        return GameBoard.GetTile(tilePos.x, tilePos.y).transform.position;
    }
}
