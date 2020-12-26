using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour, Ticable
{
    public Tile[,] board;

    [Header("Used by Enemies to pathfind")]
    public bool[,] movementBoard;
    public IDictionary<Piece, (int, int)> enemyLocations;

    [SerializeField] private Tile tilePrefab;
    [SerializeField] private int BOARD_SIZE;

    private void Awake()
    {
        board = new Tile[BOARD_SIZE, BOARD_SIZE];
        for (int i = 0; i < GetBoardHeight(); i++)
        {
            for (int j = 0; j < GetBoardWidth(); j++)
            {
                board[i, j] = Instantiate(tilePrefab.gameObject,
                                          new Vector3(j * GlobalManager.GAME_SCALE - 0.5f * j * GlobalManager.GAME_SCALE,
                                                      i * GlobalManager.GAME_SCALE - 0.5f * i * GlobalManager.GAME_SCALE,
                                                      transform.position.z),
                                          Quaternion.identity)
                                          .GetComponent<Tile>();
            }
        }
        movementBoard = new bool[BOARD_SIZE, BOARD_SIZE];
        enemyLocations = new Dictionary<Piece, (int, int)>();
    }

    public Tile GetTile((int, int) index)
    {
        return board[index.Item1, index.Item2];
    }

    public Tile GetTile(int i, int j)
    {
        return GetTile((i, j));
    }

    public Tile GetCenterTile()
    {
        return GetTile((BOARD_SIZE / 2, BOARD_SIZE / 2));
    }

    public void SpawnEnemy((int, int) index, Piece piecePrefab)
    {
        Piece p = Instantiate(piecePrefab.gameObject, 
                              board[index.Item1, index.Item2].transform.position, 
                              Quaternion.identity).GetComponent<Piece>();
        enemyLocations[p] = index;
        movementBoard[index.Item1, index.Item2] = true;
    }

    public int GetBoardHeight()
    {
        return board.GetLength(0);
    }

    public int GetBoardWidth()
    {
        return board.GetLength(1);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Tic()
    {
        foreach (Tile t in board) {
            t.Tic();
        }
    }

    void DisplayBoard()
    {
        Debug.Log(board);
    }
}
