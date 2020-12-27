using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPiece : Piece
{
    private List<GameObject> nextMoves = new List<GameObject>();
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
        moveCoroutine = StartCoroutine(MovePiece(GlobalManager.Instance.GetWorldPos(nextMove)));
        GlobalManager.Instance.GameBoard.playerLocation = nextMove;
        DrawNextDirections();
    }

    // This is SLOW! But there's no better way rn :(
    private void DrawNextDirections(int numMoves = 7)
    {
        ClearArrows();
        var nextPath = GetNextPath(numMoves);
        for (int i = 0; i < nextPath.Count - 1; i++)
            nextMoves.Add(GenerateArrow(nextPath[i], nextPath[i + 1]));
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

    private GameObject GenerateArrow(Vector2Int curr, Vector2Int next)
    {
        var diff = next - curr;
        // Idk why... but need to flip b/c our X, Y are actually backwards
        var maybeAngle = Arrow.GetAngle(new Vector2Int(diff.y, diff.x));
        if (!maybeAngle.HasValue)
            return null;
        var arrow = Instantiate(
            arrowPrefab.gameObject,
            GlobalManager.Instance.GetWorldPos(curr),
            maybeAngle.Value
        ).GetComponent<Arrow>();
        return arrow.gameObject;
    }

    private void ClearArrows()
    {
        while (nextMoves.Count > 0)
        {
            var currArrow = nextMoves[0];
            nextMoves.RemoveAt(0);
            Destroy(currArrow);
        }
        // At the end our list should be fully cleared
    }

    void OnDestroy()
    {
        Debug.Log("The player has DIED!!");
    }
}
