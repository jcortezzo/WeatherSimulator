using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Piece : MonoBehaviour, Ticable
{
    ISet<Tile> tilemap;
    public static float EPSILON = 0.1f;
    
    protected Coroutine moveCoroutine;
    protected Vector2Int finalDestination;

    // Start is called before the first frame update
    public void Start()
    {
        tilemap = new HashSet<Tile>();

    }

    // Update is called once per frame
    void Update()
    {

    }


    /// <summary>
    /// Get a mutable gameboard and generate a next available move
    /// </summary>
    /// <param name="board">Mutable GameBoard</param>
    /// <returns></returns>

    public Vector2Int? GetNextMove(GameBoard board, Vector2Int currentPos, Vector2Int dest, bool[,] occupiedBoard)
    {
        //var currPos = new Vector2Int(
        //    board.enemyLocations[this].Item1,
        //    board.enemyLocations[this].Item2
        //);

        // int row = board.occupiedBoard.GetLength(0);
        // int col = board.occupiedBoard.GetLength(1);

        //Vector2Int randomPos = new Vector2Int(8, 8); // our dest
        //Debug.LogFormat("new random pos: {0}, {1}", dest.x, dest.y);

        // This is a way we could genericize checking for movement instead... other ideas possible
        //Func<Vector2Int, bool> canMove = (Vector2Int pos) =>
        //{
        //    int row = board.occupiedBoard.GetLength(0);
        //    int col = board.occupiedBoard.GetLength(1);
        //    return pos.x >= 0 && pos.x < col && pos.y >= 0 && pos.y < row && !board.occupiedBoard[pos.x, pos.y];
        //};

        var bestPath = GetBestPath(currentPos, dest, occupiedBoard);
        if (bestPath.Count < 2)
            return null;
        return bestPath[1]; // return the NEXT move in our list
    }

    public List<Vector2Int> GetBestPath(Vector2Int currPos, Vector2Int dest, bool[,] occupiedBoard)
    {
        // set up queue and set
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        // visited is now a mapping of position -> prev
        Dictionary<Vector2Int, Vector2Int?> visited = new Dictionary<Vector2Int, Vector2Int?>();


        queue.Enqueue(currPos);
        visited[currPos] = null;

        // bfs
        while (queue.Count != 0)
        {
            var pop = queue.Dequeue();

            if (pop.Equals(dest))
            {
                return ExtractPath(visited, dest);
            }
            foreach (Vector2Int neighbor in GetNeighbours(pop))
            {
                if (CanMove(neighbor, occupiedBoard) && !visited.ContainsKey(neighbor))
                {
                    queue.Enqueue(neighbor);
                    visited[neighbor] = pop;
                }
            }
        }

        Vector2Int? nextBestDest = null;
        // if bfs didn't find a path, find a runner up dest
        foreach (Vector2Int visitedPos in visited.Keys)
        {
            if (nextBestDest == null ||
                ManhattanDistance(nextBestDest.Value, dest) >
                ManhattanDistance(visitedPos, dest))
                nextBestDest = visitedPos;
        }
        if (nextBestDest.HasValue)
            return ExtractPath(visited, nextBestDest.Value);
        return new List<Vector2Int>(); // No paths found
    }

    private float ManhattanDistance(Vector2Int pos1, Vector2Int pos2)
    {
        return Mathf.Abs(pos1.x - pos2.x) + Mathf.Abs(pos1.y - pos2.y);
    }

    private List<Vector2Int> ExtractPath(Dictionary<Vector2Int, Vector2Int?> backpointers, Vector2Int dest)
    {
        var path = new List<Vector2Int>();
        while (backpointers[dest] != null)
        {
            path.Insert(0, dest);
            dest = backpointers[dest].Value;
        }
        path.Insert(0, dest); // insert our origin into the path
        return path;
    }

    private Vector2Int? ExtractFirstMove(Dictionary<Vector2Int, Vector2Int?> backpointers, Vector2Int dest)
    {
        Vector2Int? nextVisit = null;
        while (backpointers[dest] != null)
        {
            nextVisit = dest;
            dest = backpointers[dest].Value;
        }
        return nextVisit;
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
        //Debug.Log("moving: " + oldPos + " " + newPos);

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

    }

    private void OnDestroy()
    {
        GlobalManager.Instance.GameBoard.enemyLocations.Remove(this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Tile t = collision.GetComponent<Tile>();
        if (t != null)
        {
            if (tilemap != null)
            {
                tilemap.Add(t);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Tile t = collision.GetComponent<Tile>();
        if (t != null)
        {
            if (tilemap != null)
            {
                tilemap.Remove(t);
            }
        }
    }
}
