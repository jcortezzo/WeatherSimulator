using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyEnemy : Piece
{
    // Update is called once per frame
    void Update()
    {
        finalDestination = new Vector2Int(GlobalManager.Instance.GameBoard.playerLocation.x,
                                        GlobalManager.Instance.GameBoard.playerLocation.y);
    }

    public override void Tic()
    {
        base.Tic();
        //Debug.LogFormat("play pos: {0}", finalDestination);
        Vector2Int? maybeNextMove = GetNextMove(GlobalManager.Instance.GameBoard,
                                        GlobalManager.Instance.GameBoard.enemyLocations[this],
                                        finalDestination,
                                        GlobalManager.Instance.GameBoard.occupiedBoard);

        if (maybeNextMove == null)
            return;
        Vector2Int nextMove = maybeNextMove.Value;

        //Debug.Log("piece tic");
        Tile tile = GlobalManager.Instance.GameBoard.GetTile((int)nextMove.x, (int)nextMove.y);
        Debug.Log(tile.transform.position);

        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MovePiece(tile.transform.position));

        //this.transform.position = tile.transform.position;
        GlobalManager.Instance.GameBoard.enemyLocations[this] = nextMove;
    }
}
