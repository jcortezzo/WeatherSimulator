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


    [SerializeField] public Piece playerPiece;

    private IDictionary<Vector2Int, Tile> coordsToTile;
    private IDictionary<Tile, Vector2Int> tileToCoords;

    private void Awake()
    {
        board = new Tile[GlobalManager.Instance.BOARD_SIZE, GlobalManager.Instance.BOARD_SIZE];
        coordsToTile = new Dictionary<Vector2Int, Tile>();
        tileToCoords = new Dictionary<Tile, Vector2Int>();
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
                board[i, j].position = new Vector2Int(i, j);
                coordsToTile[board[i, j].position] = board[i, j];
                tileToCoords[board[i, j]] = board[i, j].position;
            }
        }
        occupiedBoard = new bool[GlobalManager.Instance.BOARD_SIZE, GlobalManager.Instance.BOARD_SIZE];
        enemyLocations = new Dictionary<Piece, Vector2Int>();
    }

    public Tile GetTile(Vector2Int index)
    {
        return board[index.x, index.y];
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
        //Debug.Log(index);
        //Debug.Log(board.Length);
        Vector3 boardPos = board[index.x, index.y].transform.position;
        Piece p = Instantiate(piecePrefab.gameObject,
                              new Vector3(boardPos.x, boardPos.y, boardPos.z - 2),
                              Quaternion.identity).GetComponent<Piece>();
        enemyLocations[p] = index;
        occupiedBoard[index.x, index.y] = true;
        p.tileMap.Add(GetTile(index));
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
        foreach (Tile t in board)
        {
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
        if (playerPiece != null) playerPiece.Tic();
    }

    public ISet<Tile> GetNeighbors(Tile t)
    {
        ISet<Tile> ret = new HashSet<Tile>();
        List<Vector2Int> dirs = new List<Vector2Int>()
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right,
        };

        foreach (Vector2Int dir in dirs)
        {
            Vector2Int coords = tileToCoords[t] + dir;
            if (IsValidTile(coords))
            {
                ret.Add(coordsToTile[coords]);
            }
        }
        return ret;
    }

    private bool IsValidTile(Vector2Int coords)
    {
        return coords.x >= 0 && coords.x < GetBoardHeight() &&
               coords.y >= 0 && coords.y < GetBoardWidth();

    }

    void DisplayBoard()
    {
        // Debug.Log(board);
    }
}
