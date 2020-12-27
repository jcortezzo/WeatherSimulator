using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour, Ticable
{
    public Tile[,] board;

    [Header("Used by Enemies to pathfind")]
    public bool[,] occupiedBoard;
    public IDictionary<Piece, (int, int)> enemyLocations;
    public (int, int) playerLocation;

    [SerializeField] private Tile tilePrefab;
    [SerializeField] private int BOARD_SIZE;

    [SerializeField] Piece playerPiece;

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
        occupiedBoard = new bool[BOARD_SIZE, BOARD_SIZE];
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
        occupiedBoard[index.Item1, index.Item2] = true;
    }

    public void SpawnPlayerPiece((int, int) index, Piece piecePrefab)
    {
        Piece p = Instantiate(piecePrefab.gameObject,
                              board[index.Item1, index.Item2].transform.position,
                              Quaternion.identity).GetComponent<Piece>();
        playerPiece = p;
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
        occupiedBoard = new bool[BOARD_SIZE, BOARD_SIZE];
        foreach (Piece piece in enemyLocations.Keys)
        {
            (int, int) location = enemyLocations[piece];
            occupiedBoard[location.Item1, location.Item2] = true;
        }

        //playerPiece.Tic();
    }

    void DisplayBoard()
    {
        Debug.Log(board);
    }
}
