using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPiece : Piece
{
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        GenerateFinalPosition(GlobalManager.Instance.GameBoard.GetBoardHeight(),
                                 GlobalManager.Instance.GameBoard.GetBoardWidth());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Tic()
    {
        if (this.transform.position.Equals(finalPosition))
        {
            GenerateFinalPosition(GlobalManager.Instance.GameBoard.GetBoardHeight(),
                                 GlobalManager.Instance.GameBoard.GetBoardWidth());
        }
        Vector2 nextMove = GetNextMove(GlobalManager.Instance.GameBoard, finalPosition);
        if (nextMove.Equals(Vector2.negativeInfinity)) { return; }

        Debug.Log("piece tic");
        Tile tile = GlobalManager.Instance.GameBoard.GetTile((int)nextMove.x, (int)nextMove.y);
        Debug.Log(tile.transform.position);

        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MovePiece(tile.transform.position));
        GlobalManager.Instance.GameBoard.playerLocation = ((int)nextMove.x, (int)nextMove.y);
    }
}
