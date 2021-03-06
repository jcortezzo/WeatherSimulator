﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class Piece : MonoBehaviour, Ticable
{
    public Arrow arrowPrefab;
    public ISet<Tile> tileMap;
    public static float EPSILON = 0.1f;
    protected Coroutine moveCoroutine;
    protected Vector2Int prevLocation;
    protected Vector2Int currLocation;
    private SpriteRenderer sr;

    protected Vector2Int? fieldNextMove;
    protected bool coroutineRunning;

    public bool cancelTic;
    protected GameObject arrow;

    protected virtual void Awake()
    {
        tileMap = new HashSet<Tile>();
    }

    protected virtual void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sortingLayerName = "Pieces";
        currLocation = GetLocation();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (GlobalManager.Instance.IsPaused())
        {
            return;
        }
        IList<Tile> shallowCopy = new List<Tile>(tileMap);
        for (int i = 0; i < shallowCopy.Count; i++)
        {
            ReceiveEffects(shallowCopy[i]);
        }
    }

    public abstract Vector2Int GetLocation();

    public Vector2Int? GetNextMove(Vector2Int currentPos, Vector2Int dest, bool[,] occupiedBoard)
    {
        //Debug.LogFormat("current: {0}, prev: {1}", currLocation, prevLocation);
        //prevLocation = currLocation;
        //currLocation = GetLocation();

        if (tileMap == null || tileMap.Count < 1 || tileMap.Count != 1)
        {
            Debug.Log($"ERR, currently standing on {tileMap.Count} tiles");
            return null;
        }
            
        
        var tile = new List<Tile>(tileMap)[0];
        var tileType = tile.DescribeTile().type;

        // Tile "Tic" effects
        //if (tileType == TileType.ICE)
        //{
        //    Vector2Int slideDir = currLocation - prevLocation;
        //    Vector2Int newDest = currLocation + slideDir;
        //    //Tile nextTile = GlobalManager.Instance.GameBoard.GetTile(newDest);
        //    //while(nextTile != null && nextTile.DescribeTile().type == TileType.ICE)
        //    //{
        //    //    newDest += slideDir;
        //    //    nextTile = GlobalManager.Instance.GameBoard.GetTile(newDest);
        //    //}
        //    if (CanMove(newDest, occupiedBoard) && slideDir != Vector2Int.zero)
        //        return newDest;
        //}
        //else 
        if (tileType == TileType.HOT)
        {
            if (currLocation != prevLocation)
                return currLocation; // Cancel every other move while hot
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

    protected GameObject GenerateArrow(Vector2Int curr, Vector2Int next, int arrowType)
    {
        var diff = next - curr;
        // Idk why... but need to flip b/c our X, Y are actually backwards
        var maybeAngle = Arrow.GetAngle(new Vector2Int(diff.y, diff.x));
        if (!maybeAngle.HasValue)
            return null;
        Arrow arrow = Instantiate(arrowPrefab.gameObject, Vector3.zero, maybeAngle.Value)
            .GetComponent<Arrow>();
        //arrow.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/" + arrowType + "Arrow_0");
        arrow.GetComponent<Animator>().SetInteger("id", arrowType);

        return arrow.gameObject;
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

    // given a value 0-1, smooths lerp via cos
    private float SmoothLerp(float lerpVal)
    {
        // take cos from -pi -> pi
        // add 1 to our cos and div by 2
        float currRad = (1 - lerpVal) * -Mathf.PI; // + (lerpVal) * 0
        return (Mathf.Cos(currRad) + 1) / 2;
    }

    public IEnumerator MovePiece(Vector2 newPos, bool isCancelTic, float duration = float.NegativeInfinity)
    {
        // Default param val
        //if (isCancelTic)
        //{
        //    cancelTic = true;
        //}
        coroutineRunning = true;
        if (duration == float.NegativeInfinity) duration = GlobalManager.Instance.TIC_TIME / 10;
        if (newPos.Equals(Vector2.negativeInfinity))
        {
            Debug.Log("negative infi");
            coroutineRunning = false;
            yield break;
        }
        Vector2 oldPos = this.transform.position;
        int numSteps = 20; // arbirtary, this is smooth though!
        float stepLength = duration / numSteps; // time leng of step

        for (float i = 0; i < 1; i += (1f / numSteps))
        {
            // We can apply a sinuosoid to this as well!
            this.transform.position = Vector2.Lerp(oldPos, newPos, SmoothLerp(i));
            yield return new WaitForSeconds(stepLength);
        }
        this.transform.position = newPos;
        //Debug.LogFormat("current: {0}, prev: {1}", currLocation, prevLocation);
        coroutineRunning = false;
        //if (isCancelTic)
        //{
        //    cancelTic = false;
        //}
    }

    public IEnumerator TornadoMove(Vector2Int tornadoPos, Vector2Int finalPos)
    {
        // Durations are arbitrary!
        yield return MovePiece(tornadoPos, false, GlobalManager.Instance.TIC_TIME / 30);
        yield return MovePiece(finalPos, false, GlobalManager.Instance.TIC_TIME / 10);
    }

    public virtual void Tic()
    {
        //Debug.Log(this.tileMap.Count);
        if(cancelTic)
        {
            fieldNextMove = null;
            cancelTic = false;
            return;
        }
    }

    // "Continuous" tile effects
    public virtual void ReceiveEffects(Tile t)
    {
        var info = t.DescribeTile();
        // Debug.Log(info);
        if (info.effect == TileEffect.ELECTRIC)
        {
            KillPiece(this.gameObject);
        }
        else if (info.effect == TileEffect.TORNADO)
        {
            //if (moveCoroutine != null)
            //{
            //    StopCoroutine(this.moveCoroutine);
            //}
            //moveCoroutine = StartCoroutine(
            //    TornadoMove(t.position, GetLocation() + t.tornadoDir)
            //);
            Vector2Int? nextMove = GlobalManager.Instance.GameBoard.GetCoordsFromTile(t) + 3 * t.tornadoDir;
            Vector3 newPos = GlobalManager.Instance.GetWorldPos(nextMove != null ? nextMove.Value : Vector2Int.zero);  // cursed af

            
            if (nextMove != null && !GlobalManager.Instance.GameBoard.IsValidTile(nextMove.Value))
            {
                KillPiece(this.gameObject, 1f);
            }

            Debug.Log(nextMove);
            if (!coroutineRunning)
            {
                StartCoroutine(MovePiece(newPos, true));
                if (arrow != null) Destroy(arrow.gameObject);
                UpdatePiecePosition(nextMove != null ? nextMove.Value : Vector2Int.zero);  // cursed af

            }
            cancelTic = true;
        }
        else if (info.effect == TileEffect.FIRE)
        {
            KillPiece(this.gameObject);
        }

        if (info.type == TileType.ICE)
        {
            Vector2 p1 = new Vector2(currLocation.x, currLocation.y);
            Vector2 p2 = new Vector2(prevLocation.x, prevLocation.y);
            Vector2 dir = (p1 - p2).normalized;
            Vector2Int slideDir = new Vector2Int((int)dir.x, (int)dir.y);
            Vector2Int newDest = currLocation;

            Tile nextTile = GlobalManager.Instance.GameBoard.GetTile(newDest);
            Tile prevTile = GlobalManager.Instance.GameBoard.GetTile(currLocation);
            bool standStill = false;

            while (nextTile != null && nextTile.DescribeTile().type == TileType.ICE && !coroutineRunning)
            {
                newDest += slideDir;
                if (newDest.Equals(newDest - slideDir))
                {
                    Debug.Log("standingstill");
                    standStill = true;
                    break;
                }
                prevTile = nextTile;
                nextTile = GlobalManager.Instance.GameBoard.GetTile(newDest);
            }
            if (nextTile == null)
            {
                Debug.Log("death by ice");
                KillPiece(this.gameObject);
                return;
            }
            //} else
            //{
            //    prevLocation = GlobalManager.Instance.GameBoard.GetCoordsFromTile(prevTile);   
            //}
            Vector2Int nextMove = standStill ? newDest + Vector2Int.left : newDest;
            Vector3 newPos = GlobalManager.Instance.GetWorldPos(nextMove);
            //Debug.Log(nextMove);
            if (!coroutineRunning)
            {
                Debug.Log("run coroutine");
                StartCoroutine(MovePiece(newPos, true));
                if (arrow != null) Destroy(arrow.gameObject);
                UpdatePiecePosition(nextMove);
                
            }else
            {
                Debug.Log("Coroutine still running");
            }
            cancelTic = true;
        }
    }



    public abstract void UpdatePiecePosition(Vector2Int newPos);

    void OnTriggerStay2D(Collider2D collision)
    {
        //Debug.Log("on something");
        Tile t = collision.GetComponent<Tile>();
        if (t != null)
        {
            //Debug.Log("fucucucuck");
            //Debug.Log("it's nguyend outside");
            this.tileMap.Add(t);
            //Debug.Log("omg wtf " + tileMap.Count);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        Tile t = collision.GetComponent<Tile>();
        if (t != null)
        {
            this.tileMap.Add(t);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        Tile t = collision.GetComponent<Tile>();
        if (t != null)
        {
            tileMap.Remove(t);
        }
    }

    public virtual void KillPiece(GameObject go, float afterSec = 0.0f)
    {
        StartCoroutine(Kill(go, afterSec));
    }

    private IEnumerator Kill(GameObject go, float afterSec)
    {
        yield return new WaitForSeconds(afterSec);
        //Jukebox.Instance.PlaySFX("Enemy Death", 0.5f, 1f);
        Destroy(go);
    }
}
