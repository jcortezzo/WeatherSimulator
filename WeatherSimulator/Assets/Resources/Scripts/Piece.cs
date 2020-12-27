﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class Piece : MonoBehaviour, Ticable
{
    ISet<Tile> tilemap;
    public static float EPSILON = 0.1f;
    protected Coroutine moveCoroutine;
    private Vector2Int prevPosition;
    private SpriteRenderer sr;

    // Start is called before the first frame update
    public virtual void Start()
    {
        tilemap = new HashSet<Tile>();
        sr = GetComponent<SpriteRenderer>();
        sr.sortingLayerName = "Pieces";
        prevPosition = GetLocation();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(tilemap.Count);
        foreach (Tile t in tilemap)
        {
            ReceiveEffects(t);
        }
    }

    public abstract Vector2Int GetLocation();

    public Vector2Int? GetNextMove(Vector2Int currentPos, Vector2Int dest, bool[,] occupiedBoard)
    {
        Assert.IsTrue(tilemap.Count == 1);
        var tile = tilemap.GetEnumerator().Current;

        if (/* tile == ice*/ true)
        {
            return GetLocation() - prevPosition; // continue in the same dir
        }

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
        prevPosition = GetLocation();
    }

    private void OnDestroy()
    {
        GlobalManager.Instance.GameBoard.enemyLocations.Remove(this);
    }

    private void ReceiveEffects(Tile t)
    {
        var info = t.DescribeTile();
        Debug.Log(info);
        if (info.effect == TileEffect.ELECTRIC)
        {
            //Debug.Log(t.DescribeTile());
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Tile t = collision.gameObject.GetComponent<Tile>();
        if (t != null)
        {
            if (tilemap != null)
            {
                tilemap.Add(t);
            }
            //ReceiveEffects(t);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Tile t = collision.gameObject.GetComponent<Tile>();
        if (t != null)
        {
            if (tilemap != null)
            {
                tilemap.Add(t);
            }
            //ReceiveEffects(t);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Tile t = collision.gameObject.GetComponent<Tile>();
        if (t != null)
        {
            if (tilemap != null)
            {
                tilemap.Remove(t);
            }
            //ReceiveEffects(t);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log("on something");
        Tile t = collision.GetComponent<Tile>();
        if (t != null)
        {
            if (tilemap != null)
            {
                tilemap.Add(t);
            }
            //ReceiveEffects(t);
        }
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
            //ReceiveEffects(t);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Tile t = collision.GetComponent<Tile>();
        if (t != null)
        {
            if (tilemap != null)
            {
                //tilemap.Remove(t);
            }
        }
    }
}
