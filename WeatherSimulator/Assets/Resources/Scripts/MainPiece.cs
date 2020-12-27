using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPiece : Piece
{
    private Vector2Int destination;
    private bool[,] playerBoard;

    public override Vector2Int GetLocation()
    {
        return GlobalManager.Instance.GameBoard.playerLocation;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        playerBoard = new bool[GlobalManager.Instance.BOARD_SIZE, GlobalManager.Instance.BOARD_SIZE];
        destination = GenerateRandomPos();
    }

    private Vector2Int GenerateRandomPos()
    {
        int w = GlobalManager.Instance.GameBoard.GetBoardWidth();
        int h = GlobalManager.Instance.GameBoard.GetBoardHeight();
        return new Vector2Int(Random.Range(0, w), Random.Range(0, h));
    }

    public override void Tic()
    {
        base.Tic();
        if (GetLocation().Equals(destination))
            destination = GenerateRandomPos();
        Vector2Int? maybeNextMove = GetNextMove(GetLocation(), destination, playerBoard);
        if (!maybeNextMove.HasValue)
            return;
        Vector2Int nextMove = maybeNextMove.Value;
        // if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MovePiece(GlobalManager.Instance.GetWorldPos(nextMove), GlobalManager.Instance.TIC_TIME / 10));
    }
}
