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

        Queue<NextMove> queue = new Queue<NextMove>();
        NextMove currentPos = new NextMove() { prev = Vector2.zero };

        queue.Enqueue(currentPos);
        while (queue.Count != 0)
        {
            
        }

        return Vector2.zero;
    }

    private class NextMove {
        public Vector2 prev;
    }

    private bool CanMove(Vector2 pos, int row, int col)
    {
        return pos.x >= 0 && pos.x < col && pos.y >= 0 && pos.y < row;
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
