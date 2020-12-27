using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour, Ticable
{
    public Tile[,] board;

    [Header("Used by Enemies to pathfind")]
    public bool[,] occupiedBoard;
    public IDictionary<Piece, Vector2Int> enemyLocations;
    public Vector2Int playerLocation;

    [SerializeField] private Tile tilePrefab;
    

    [SerializeField] Piece playerPiece;
    
    private void Awake()
    {
        board = new Tile[GlobalManager.Instance.BOARD_SIZE, GlobalManager.Instance.BOARD_SIZE];
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
        occupiedBoard = new bool[GlobalManager.Instance.BOARD_SIZE, GlobalManager.Instance.BOARD_SIZE];
        enemyLocations = new Dictionary<Piece, Vector2Int>();
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
        return GetTile((GlobalManager.Instance.BOARD_SIZE / 2, GlobalManager.Instance.BOARD_SIZE / 2));
    }

    public void SpawnEnemy(Vector2Int index, Piece piecePrefab)
    {
        Debug.Log(index);
        Debug.Log(board.Length);
        Piece p = Instantiate(piecePrefab.gameObject, 
                              board[index.x, index.y].transform.position, 
                              Quaternion.identity).GetComponent<Piece>();
        enemyLocations[p] = index;
        occupiedBoard[index.x, index.y] = true;
    }

    public void SpawnPlayerPiece(Vector2Int index, Piece piecePrefab)
    {
        Piece p = Instantiate(piecePrefab.gameObject,
                              board[index.x, index.y].transform.position,
                              Quaternion.identity).GetComponent<Piece>();
        playerPiece = p;
        playerLocation = index;
    }

    public int GetBoardHeight()
    {
        return board.GetLength(0);
    }

    public int GetBoardWidth()
    {
        return board.GetLength(1);
    }

    public void Tic()
    {
        foreach (Tile t in board) {
            t.Tic();
        }
        ISet<Piece> copy = new HashSet<Piece>(enemyLocations.Keys);
        foreach (Piece piece in copy)
        {
            piece.Tic();
        }
        occupiedBoard = new bool[GlobalManager.Instance.BOARD_SIZE, GlobalManager.Instance.BOARD_SIZE];
        foreach (Piece piece in enemyLocations.Keys)
        {
            Vector2Int location = enemyLocations[piece];
            occupiedBoard[location.x, location.y] = true;
        }

        playerPiece.Tic();
    }

    void DisplayBoard()
    {
        Debug.Log(board);
    }
}
