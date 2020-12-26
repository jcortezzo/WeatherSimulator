using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    public Tile[,] board;
    public bool[,] movementBoard;

    [SerializeField] private int BOARD_SIZE;

    private void Awake()
    {
        board = new Tile[BOARD_SIZE, BOARD_SIZE];
        movementBoard = new bool[BOARD_SIZE, BOARD_SIZE];
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DisplayBoard()
    {
        Debug.Log(board);
    }
}
