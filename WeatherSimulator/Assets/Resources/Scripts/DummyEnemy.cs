﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyEnemy : Piece
{

    protected override void Start()
    {
        base.Start();
        fieldNextMove = GetNextMove(GetLocation(),
                                    GlobalManager.Instance.GameBoard.playerLocation,
                                    GlobalManager.Instance.GameBoard.occupiedBoard);
        prevLocation = currLocation - Vector2Int.right;  // .___.
        if (fieldNextMove == null)
        {
            return;
        }

        var nextMove = fieldNextMove.Value;
        var newPos = GlobalManager.Instance.GetWorldPos(nextMove);
        if (arrow != null) Destroy(arrow);
        arrow = GenerateArrow(GetLocation(), nextMove, 1);
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
        //if(cancelTic)
        //{
        //    return;
        //}
        if (fieldNextMove == null)
        {
            fieldNextMove = GetNextMove(GetLocation(),
                                   GlobalManager.Instance.GameBoard.playerLocation,
                                   GlobalManager.Instance.GameBoard.occupiedBoard);
            return;
        }
        Vector2Int nextMove = fieldNextMove.Value;
        Vector3 newPos = GlobalManager.Instance.GetWorldPos(nextMove);
        StartCoroutine(MovePiece(newPos, false));

        UpdatePiecePosition(nextMove);

        fieldNextMove = GetNextMove(GetLocation(),
                                    GlobalManager.Instance.GameBoard.playerLocation,
                                    GlobalManager.Instance.GameBoard.occupiedBoard);
        if (fieldNextMove == null) { return; }

        nextMove = fieldNextMove.Value;
        newPos = GlobalManager.Instance.GetWorldPos(nextMove);
        // Generate our nextup arrow:

        if (arrow != null) Destroy(arrow);
        arrow = GenerateArrow(GetLocation(), nextMove, 1);
        if (arrow != null)
        {
            arrow.transform.position = newPos;
            arrow.transform.parent = null;
        }

        //arrow.transform.parent = transform; // make that arrow a child of our enemy
    }

    public override void UpdatePiecePosition(Vector2Int newPos)
    {
        prevLocation = GlobalManager.Instance.GameBoard.enemyLocations[this];
        GlobalManager.Instance.GameBoard.enemyLocations[this] = newPos;
        currLocation = newPos;
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
            KillPiece(piece.gameObject);
        }

        DummyEnemy dummy = collision.gameObject.GetComponent<DummyEnemy>();

        if (dummy != null)
        {
            KillPiece(dummy.gameObject);
            KillPiece(this.gameObject);


        }
    }

    public override void KillPiece(GameObject go, float afterSec = 0)
    {
        //GlobalManager.Instance.point += 100;
        Jukebox.Instance.PlaySFX("deathNoise", 2f, 1f);
        base.KillPiece(go, afterSec);
    }

    void OnDestroy()
    {
        if (arrow != null)
        {
            Destroy(arrow.gameObject);
        }
        GlobalManager.Instance.GameBoard.enemyLocations.Remove(this);
        GlobalManager.Instance.point += 100;
        //Jukebox.Instance.PlaySFX("deathNoise", 1f, 1f);
    }


}
