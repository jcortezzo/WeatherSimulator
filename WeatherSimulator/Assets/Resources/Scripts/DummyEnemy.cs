﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyEnemy : Piece
{
    private GameObject arrow;
    private Vector2Int? fieldNextMove;

    protected override void Start()
    {
        base.Start();
        fieldNextMove = GetNextMove(GetLocation(),
                                    GlobalManager.Instance.GameBoard.playerLocation,
                                    GlobalManager.Instance.GameBoard.occupiedBoard);
        if (fieldNextMove == null)
        {
            return;
        }

        var nextMove = fieldNextMove.Value;
        var newPos = GlobalManager.Instance.GetWorldPos(nextMove);
        if (arrow != null) Destroy(arrow);
        arrow = GenerateArrow(GetLocation(), nextMove, "Enemy");
        arrow.transform.position = newPos;//transform.position;
        arrow.transform.parent = null;
        //arrow.transform.parent = transform; // make that arrow a child of our enemy
    }

    public override Vector2Int GetLocation()
    {
        return GlobalManager.Instance.GameBoard.enemyLocations[this];
    }

    public override void Tic()
    {
        base.Tic();

        if (fieldNextMove == null)
        {
            return;
        }
        var nextMove = fieldNextMove.Value;
        var newPos = GlobalManager.Instance.GetWorldPos(nextMove);
        // if (moveCoroutine != null) StopCoroutine(moveCoroutine); Shouldn't need this
        moveCoroutine = StartCoroutine(MovePiece(newPos));
        GlobalManager.Instance.GameBoard.enemyLocations[this] = nextMove;

        fieldNextMove = GetNextMove(
                                    GetLocation(),
                                    GlobalManager.Instance.GameBoard.playerLocation,
                                    GlobalManager.Instance.GameBoard.occupiedBoard);
        if (fieldNextMove == null)
        {
            return;
        }
        nextMove = fieldNextMove.Value;
        newPos = GlobalManager.Instance.GetWorldPos(nextMove);
        // Generate our nextup arrow:

        if (arrow != null) Destroy(arrow);
        arrow = GenerateArrow(GetLocation(), nextMove, "Enemy");
        arrow.transform.position = newPos;//transform.position;
        arrow.transform.parent = null;
        //arrow.transform.parent = transform; // make that arrow a child of our enemy
    }

    //public override void Tic()
    //{
    //    base.Tic();
    //    Vector2Int? maybeNextMove = GetNextMove(
    //                                    GetLocation(),
    //                                    GlobalManager.Instance.GameBoard.playerLocation,
    //                                    GlobalManager.Instance.GameBoard.occupiedBoard);
    //    if (maybeNextMove == null)
    //        return;

    //    // Generate our nextup arrow:
    //    var nextMove = maybeNextMove.Value;
    //    if (arrow != null) Destroy(arrow);
    //    arrow = GenerateArrow(GetLocation(), nextMove);
    //    arrow.transform.position = transform.position;
    //    arrow.transform.parent = transform; // make that arrow a child of our enemy

    //    var newPos = GlobalManager.Instance.GetWorldPos(nextMove);
    //    // if (moveCoroutine != null) StopCoroutine(moveCoroutine); Shouldn't need this
    //    moveCoroutine = StartCoroutine(MovePiece(newPos));
    //    GlobalManager.Instance.GameBoard.enemyLocations[this] = nextMove;
    //}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        MainPiece piece = collision.gameObject.GetComponent<MainPiece>();
        if (piece != null)
        {
            Destroy(piece.gameObject);
        }

        DummyEnemy dummy = collision.gameObject.GetComponent<DummyEnemy>();
        if (dummy != null)
        {

            Destroy(this.gameObject);
            Destroy(dummy.gameObject);
        }
    }

    void OnDestroy()
    {
        GlobalManager.Instance.GameBoard.enemyLocations.Remove(this);
    }

}
