using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    ISet<Tile> tilemap;

    // Start is called before the first frame update
    void Start()
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
    public Vector2 GetNextMove(GameBoard board)
    {
        int row = board.movementBoard.GetLength(0);
        int col = board.movementBoard.GetLength(1);
        Vector2 randomPos = new Vector2(Random.Range(0, row), Random.Range(0, col));

        // set up queue and set
        Queue<NextMove> queue = new Queue<NextMove>();
        ISet<Vector2> visited = new HashSet<Vector2>();

        (int, int) pos = board.enemyLocations[this];
        NextMove currentPos = new NextMove() { prev = null, nextMove = new Vector2(pos.Item1, pos.Item2) };
        
        bool pathFound = false;
        NextMove lastMove = null;

        queue.Enqueue(currentPos);
        visited.Add(currentPos.nextMove);

        // bfs
        while (queue.Count != 0)
        {
            NextMove pop = queue.Dequeue();

            if(pop.nextMove == randomPos)
            {
                pathFound = true;
                lastMove = pop;
                break;
            }
            List<Vector2> availableMove = GetNeighbour(currentPos.nextMove, board.movementBoard);
            foreach(Vector2 nextMove in availableMove)
            {
                if(!visited.Contains(nextMove)) {
                    NextMove newMove = new NextMove() { prev = pop, nextMove = nextMove };
                    queue.Enqueue(newMove);
                    visited.Add(newMove.nextMove);
                }
            }
        }

        Vector2 result = Vector2.zero; // this might be a problem
        if(pathFound)
        {
            //fence post
            board.movementBoard[(int)lastMove.nextMove.x, (int)lastMove.nextMove.y] = false;
            while (lastMove.prev != null) // stop one short
            {
                lastMove = lastMove.prev;
                board.movementBoard[(int)lastMove.nextMove.x, (int)lastMove.nextMove.y] = false;
            }
            result = lastMove.nextMove;
        }
        return result;
    }

    private List<Vector2> GetNeighbour(Vector2 current, bool[,] movementBoard)
    {
        List<Vector2> result = new List<Vector2>();
        result.Add( current + Vector2.up);
        result.Add (current + Vector2.down);
        result.Add (current + Vector2.left);
        result.Add (current + Vector2.right);
        for(int i = 0; i < result.Count; i++)
        {
            if(!CanMove(result[i], movementBoard))
            {
                result.Remove(result[i]);
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
        return pos.x >= 0 && pos.x < col && pos.y >= 0 && pos.y < row & movementBoard[(int)pos.x, (int)pos.y];
    }

    public IEnumerator MovePiece(Vector2 newPos)
    {
        return null;
    }

    public void Tick()
    {
        // DO sh1t
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
