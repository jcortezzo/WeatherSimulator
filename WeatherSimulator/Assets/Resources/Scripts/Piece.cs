using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    ISet<Tile> tilemap;
    public static float EPSILON = 0.1f;

    protected Coroutine moveCoroutine;
    protected Vector2 finalPosition;

    // Start is called before the first frame update
    public void Start()
    {
        tilemap = new HashSet<Tile>();

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void GenerateFinalPosition(int row, int col)
    {
        finalPosition = new Vector2(Random.Range(0, row), Random.Range(0, col));
    }

    /// <summary>
    /// Get a mutable gameboard and generate a next available move
    /// </summary>
    /// <param name="board">Mutable GameBoard</param>
    /// <returns></returns>
    public Vector2 GetNextMove(GameBoard board, Vector2 finalPosition)
    {
        Debug.LogFormat("new random pos: {0}, {1}", finalPosition.x, finalPosition.y);
        // set up queue and set
        Queue<NextMove> queue = new Queue<NextMove>();
        ISet<Vector2> visited = new HashSet<Vector2>();
        (int, int) pos = (2,2);
        if (this.GetComponent<MainPiece>() != null)
        {
            pos = board.playerLocation;
        } else
        {
            pos = board.enemyLocations[this];
        }
        
        NextMove currentPos = new NextMove() { prev = null, nextMove = new Vector2(pos.Item1, pos.Item2) };
        
        bool pathFound = false;
        NextMove finalMove = null;

        queue.Enqueue(currentPos);
        visited.Add(currentPos.nextMove);

        // bfs
        while (queue.Count != 0)
        {
            NextMove pop = queue.Dequeue();
            //Debug.LogFormat("current move check: {0}", pop.nextMove);
            if(pop.nextMove.Equals(finalPosition))
            {
                Debug.Log("path found");
                pathFound = true;
                finalMove = pop;
                break;
            }
            List<Vector2> availableMove = GetNeighbour(pop.nextMove, board.occupiedBoard);
            //Debug.LogFormat("neighbour {0}", availableMove.Count);
            foreach (Vector2 nextMove in availableMove)
            {
                if(!visited.Contains(nextMove)) {
                    NextMove newMove = new NextMove() { prev = pop, nextMove = nextMove };
                    queue.Enqueue(newMove);
                    visited.Add(newMove.nextMove);
                }
            }
        }

        Vector2 result = Vector2.negativeInfinity;
        if(pathFound)
        {
            //fence post
            //board.occupiedBoard[(int)finalMove.nextMove.x, (int)finalMove.nextMove.y] = true;
            while (finalMove.prev != null && finalMove.prev.prev != null) // stop one short
            {
                finalMove = finalMove.prev;
                //board.occupiedBoard[(int)finalMove.nextMove.x, (int)finalMove.nextMove.y] = true;
            }
            result = finalMove.nextMove;
            //if(!finalMove.Equals(randomPos))
            //{
            //    board.occupiedBoard[(int)finalMove.nextMove.x, (int)finalMove.nextMove.y] = true;
            //    board.occupiedBoard[(int)currentPos.nextMove.x, (int)currentPos.nextMove.y] = false;
            //} else
            
            
        }
        return result;
    }

    private List<Vector2> GetNeighbour(Vector2 current, bool[,] movementBoard)
    {
        List<Vector2> result = new List<Vector2>();
        result.Add (current + Vector2.up);
        result.Add (current + Vector2.down);
        result.Add (current + Vector2.left);
        result.Add (current + Vector2.right);
        for(int i = 0; i < result.Count; i++)
        {
            if(!CanMove(result[i], movementBoard))
            {
                result.Remove(result[i]);
                i--;
            }
        }
        return result;
    }

    private class NextMove {
        public NextMove prev;
        public Vector2 nextMove;
    }

    private bool CanMove(Vector2 pos, bool[,] movementBoard)
    {
        int row = movementBoard.GetLength(0);
        int col = movementBoard.GetLength(1);
        //Debug.LogFormat("{0}, {1}, board {2}, {3}", pos.x, pos.y, row, col);
        return pos.x >= 0 && pos.x < col && pos.y >= 0 && pos.y < row && !movementBoard[(int)pos.x, (int)pos.y];
    }

    public IEnumerator MovePiece(Vector2 newPos)
    {
        while(Vector2.Distance(this.transform.position, newPos) > EPSILON)
        {
            Vector2 pos = Vector2.Lerp(this.transform.position, newPos, 0.05f);
            this.transform.position = pos;
            yield return null;
        }

        this.transform.position = newPos;
    }

    public virtual void Tic()
    {
        Vector2 nextMove = GetNextMove(GlobalManager.Instance.GameBoard, finalPosition);
        if (nextMove.Equals(Vector2.negativeInfinity)) { return; }

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
