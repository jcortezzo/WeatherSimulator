using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPiece : Piece
{
    private bool[,] playerBoard;
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        playerBoard = new bool[GlobalManager.Instance.BOARD_SIZE, GlobalManager.Instance.BOARD_SIZE];
        GenerateFinalPosition(GlobalManager.Instance.GameBoard.GetBoardHeight(),
                                 GlobalManager.Instance.GameBoard.GetBoardWidth());
    }

    public void GenerateFinalPosition(int row, int col)
    {
        finalDestination = new Vector2Int(Random.Range(0, row), Random.Range(0, col));
    }

    public override void Tic()
    {
        if (GlobalManager.Instance.GameBoard.playerLocation.Equals(finalDestination))
        {
            GenerateFinalPosition(GlobalManager.Instance.GameBoard.GetBoardHeight(),
                                 GlobalManager.Instance.GameBoard.GetBoardWidth());
        }
        Vector2Int playPos = GlobalManager.Instance.GameBoard.playerLocation;
        Vector2Int? maybeNextMove = GetNextMove(GlobalManager.Instance.GameBoard,
                                        playPos,
                                        finalDestination,
                                        playerBoard);
        if (maybeNextMove == null)
            return;
        Vector2Int nextMove = maybeNextMove.Value;

        //Debug.Log("piece tic");
        Tile tile = GlobalManager.Instance.GameBoard.GetTile(nextMove.x, nextMove.y);
        //Debug.Log(tile.transform.position);

        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MovePiece(tile.transform.position));
        GlobalManager.Instance.GameBoard.playerLocation = nextMove;
    }
}
