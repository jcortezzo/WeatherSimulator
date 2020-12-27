using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyEnemy : Piece
{
    

    public override Vector2Int GetLocation()
    {
        return GlobalManager.Instance.GameBoard.enemyLocations[this];
    }

    public override void Tic()
    {
        base.Tic();
        Vector2Int? maybeNextMove = GetNextMove(
                                        GetLocation(),
                                        GlobalManager.Instance.GameBoard.playerLocation,
                                        GlobalManager.Instance.GameBoard.occupiedBoard);
        if (maybeNextMove == null)
            return;
        var nextMove = maybeNextMove.Value;
        var newPos = GlobalManager.Instance.GetWorldPos(nextMove);
        // if (moveCoroutine != null) StopCoroutine(moveCoroutine); Shouldn't need this
        moveCoroutine = StartCoroutine(MovePiece(newPos));
        GlobalManager.Instance.GameBoard.enemyLocations[this] = nextMove;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        MainPiece piece = collision.gameObject.GetComponent<MainPiece>();
        if (piece != null)
        {
            Destroy(piece.gameObject);
        }

        DummyEnemy dummy = collision.gameObject.GetComponent<DummyEnemy>();
        if (dummy != null) { 

            Destroy(this.gameObject);
            Destroy(dummy.gameObject);
        }
    }

    void OnDestroy()
    {
        GlobalManager.Instance.GameBoard.enemyLocations.Remove(this);
    }

}
