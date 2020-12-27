using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    ISet<Tile> tilemap;
    public static float EPSILON = 0.1f;

    private Coroutine moveCoroutine;
    
    // Start is called before the first frame update
    public void Start()
    {
        tilemap = new HashSet<Tile>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    //protected void GenerateFinalPosition(int row, int col)
    //{
    //    finalPosition = new Vector2(Random.Range(0, row), Random.Range(0, col));
    //}

    /// <summary>
    /// Get a mutable gameboard and generate a next available move
    /// </summary>
    /// <param name="board">Mutable GameBoard</param>
    /// <returns></returns>

    public Vector2Int? GetNextMove(GameBoard board)
    {
        int row = board.occupiedBoard.GetLength(0);
        int col = board.occupiedBoard.GetLength(1);
        Vector2Int randomPos = new Vector2Int(8, 8);

        Debug.LogFormat("new random pos: {0}, {1}", randomPos.x, randomPos.y);
        // set up queue and set
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        // visited is now a mapping of position -> prev
        Dictionary<Vector2Int, Vector2Int?> visited = new Dictionary<Vector2Int, Vector2Int?>();
        Vector2Int? finalMove = null;

        // enemyLocations should just return Vector2Ints, dw for now
        var currentPos = new Vector2Int(
            board.enemyLocations[this].Item1,
            board.enemyLocations[this].Item2
        );

        queue.Enqueue(currentPos);
        visited.Add(currentPos, null);

        // bfs
        while (queue.Count != 0)
        {

            var pop = queue.Dequeue();
            if (pop.Equals(randomPos))

            {
                Debug.Log("path found");
                finalMove = pop;
                break;
            }
            foreach (Vector2Int neighbor in GetNeighbours(pop))
            {
                if (CanMove(neighbor, board.occupiedBoard) && !visited.ContainsKey(neighbor))
                {
                    queue.Enqueue(neighbor);
                    visited.Add(neighbor, pop);
                }
            }
        }
        if (finalMove == null)
        {
            // Try and find out closest final move
            foreach (Vector2Int visitedPos in visited.Keys)
            {
                var newFinalMove = visited[visitedPos];
                // Can optimize this later if needed
                if (finalMove == null ||
                    ManhattanDistance(finalMove.Value, randomPos) <
                    ManhattanDistance(newFinalMove.Value, randomPos))
                    finalMove = newFinalMove;
            }
        }
        if (finalMove == null)
            return null; // No possible moves
        return ExtractFirstMove(visited, finalMove.Value);
    }

    private float ManhattanDistance(Vector2Int pos1, Vector2Int pos2)
    {
        return Mathf.Abs(pos1.x - pos2.x) + Mathf.Abs(pos1.y - pos2.y);
    }

    private Vector2Int ExtractFirstMove(Dictionary<Vector2Int, Vector2Int?> backpointers, Vector2Int dest)
    {
        if (!backpointers.ContainsKey(dest) || backpointers[dest] == null)
        {
            Debug.Log("Invalid path");
        }
        else
        {
            while (backpointers[dest] != null &&
                backpointers[backpointers[dest].Value] != null)
            {
                dest = backpointers[dest].Value;
            }
        }
        return dest;
    }

    private List<Vector2Int> GetNeighbours(Vector2Int current)
    {
        return new List<Vector2Int>{
            (current + Vector2Int.up),
            (current + Vector2Int.down),
            (current + Vector2Int.left),
            (current + Vector2Int.right),
        };
    }

    private bool CanMove(Vector2Int pos, bool[,] movementBoard)
    {
        int row = movementBoard.GetLength(0);
        int col = movementBoard.GetLength(1);
        return pos.x >= 0 && pos.x < col && pos.y >= 0 && pos.y < row && !movementBoard[pos.x, pos.y];
    }

    public IEnumerator MovePiece(Vector2 newPos)
    {
        Vector2 oldPos = this.transform.position;
        Debug.Log("moving: " + oldPos + " " + newPos);

        int numSteps = 10; // arbirtary, this is smooth though!
        float stepLength = GlobalManager.Instance.TIC_TIME / numSteps / 10; // time leng of step

        for (float i = 0; i < 1; i += (1f / numSteps))
        {
            // We can apply a sinuosoid to this as well!
            this.transform.position = Vector2.Lerp(oldPos, newPos, i);
            yield return new WaitForSeconds(stepLength);
        }
        this.transform.position = newPos;
    }

    public virtual void Tic()
    {


        Vector2Int? maybeNextMove = GetNextMove(GlobalManager.Instance.GameBoard);
        if (maybeNextMove == null)
            return;
        Vector2Int nextMove = maybeNextMove.Value;

        Debug.Log("piece tic");
        Tile tile = GlobalManager.Instance.GameBoard.GetTile((int)nextMove.x, (int)nextMove.y);
        Debug.Log(tile.transform.position);

        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MovePiece(tile.transform.position));

        //this.transform.position = tile.transform.position;
        GlobalManager.Instance.GameBoard.enemyLocations[this] = ((int)nextMove.x, (int)nextMove.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Tile t = collision.GetComponent<Tile>();
        if (t != null)
        {
            tilemap.Add(t);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Tile t = collision.GetComponent<Tile>();
        if (t != null)
        {
            tilemap.Remove(t);
        }
    }
}
