using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalManager : MonoBehaviour
{
    public static GlobalManager Instance;
    public float globalTimer;
    public const int GAME_SCALE = 2;

    public GameBoard GameBoard { get { return gameBoard; } }
    [SerializeField] public float TIC_TIME = 2f;
    [SerializeField] private GameBoard gameBoard;
    [SerializeField] private Piece piece;
    [SerializeField] private Piece mainPiece;

    public Camera cam;

    private void Awake()
    {
        globalTimer = 0;
        if(Instance == null)
        {
            Instance = this;
        } else
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
        //for (int i = 0; i < 3; i++)
        //{
        //    gameBoard.SpawnEnemy((i, 0), piece);  // TODO: give piece prefab from Nguyen
        //}
        gameBoard.SpawnPlayerPiece((8, 8), mainPiece);

        //gameBoard.SpawnEnemy((0, 0), piece);
        //gameBoard.SpawnEnemy((4, 4), piece);
        
        Vector3 centerPos = gameBoard.GetCenterTile().transform.position;
        cam.transform.position = new Vector3(centerPos.x, centerPos.y, cam.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        globalTimer += Time.deltaTime;
        if (globalTimer >= TIC_TIME)
        {
            globalTimer = 0f;
            gameBoard.Tic();
        }
    }

}
