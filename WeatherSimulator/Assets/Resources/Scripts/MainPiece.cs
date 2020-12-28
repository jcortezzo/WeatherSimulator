using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPiece : Piece
{
    private List<GameObject> arrowPath = new List<GameObject>();
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

        fieldNextMove = GetNextMove(GetLocation(), destination, playerBoard);
        GlobalManager.Instance.CreateTreasure(GlobalManager.Instance.GameBoard.GetTile(destination).transform.position);//destination);

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
        // update new location if 
        if (GetLocation().Equals(destination))
        {
            destination = GenerateRandomPos();
            GlobalManager.Instance.CreateTreasure(GlobalManager.Instance.GameBoard.GetTile(destination).transform.position);
        }

        if(fieldNextMove == null)
        {
            fieldNextMove = GetNextMove(GetLocation(), destination, playerBoard);
            return;
        }
        //Vector2Int? maybeNextMove = GetNextMove(GetLocation(), destination, playerBoard);
        //if (!maybeNextMove.HasValue)
        //    return;
        
        Vector2Int nextMove = fieldNextMove.Value;
        Debug.Log("Player next move: " + nextMove);
        // if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MovePiece(GlobalManager.Instance.GetWorldPos(nextMove)));
        UpdatePiecePosition(nextMove);
        DrawNextDirections();

        fieldNextMove = GetNextMove(GetLocation(), destination, playerBoard);
        //if(fieldNextMove == null) { return;  }


    }

    //public override void Tic()
    //{
    //    base.Tic();
    //    // update new location if 
    //    if (GetLocation().Equals(destination))
    //        destination = GenerateRandomPos();

    //    Vector2Int? maybeNextMove = GetNextMove(GetLocation(), destination, playerBoard);
    //    if (!maybeNextMove.HasValue)
    //        return;

    //    Vector2Int nextMove = maybeNextMove.Value;
    //    // if (moveCoroutine != null) StopCoroutine(moveCoroutine);
    //    moveCoroutine = StartCoroutine(MovePiece(GlobalManager.Instance.GetWorldPos(nextMove)));
    //    GlobalManager.Instance.GameBoard.playerLocation = nextMove;
    //    DrawNextDirections();
    //}

    public override void UpdatePiecePosition(Vector2Int newPos)
    {
        GlobalManager.Instance.GameBoard.playerLocation = newPos;
    }

    // This is SLOW! But there's no better way rn :(
    private void DrawNextDirections(int numMoves = 7)
    {
        ClearArrows();
        var nextPath = GetNextPath(numMoves);
        for (int i = 1; i < nextPath.Count - 1; i++)
        {
            var arrow = GenerateArrow(nextPath[i], nextPath[i + 1], 0);
            arrow.transform.position = GlobalManager.Instance.GetWorldPos(nextPath[i]);
            arrowPath.Add(arrow);
        }
    }


    private List<Vector2Int> GetNextPath(int maxLeng)
    {
        var path = new List<Vector2Int>();
        var currPos = GetLocation();
        path.Add(currPos);
        for (int i = 0; i < maxLeng; i++)
        {
            var maybeNextMove = GetNextMove(currPos, destination, playerBoard);
            if (!maybeNextMove.HasValue)
                return path; // reached a deadend or dest
            var nextPos = maybeNextMove.Value;
            currPos = nextPos;
            path.Add(currPos);
        }
        return path;
    }

    private void ClearArrows()
    {
        while (arrowPath.Count > 0)
        {
            var currArrow = arrowPath[0];
            arrowPath.RemoveAt(0);
            Destroy(currArrow);
        }
        // At the end our list should be fully cleared
    }

    void OnDestroy()
    {
        ClearArrows();
        Debug.Log("The player has DIED!!");
    }


}
