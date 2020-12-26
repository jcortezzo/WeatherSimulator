using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalManager : MonoBehaviour
{
    public float globalTimer;
    [SerializeField] private float TIC_TIME = 2f;
    [SerializeField] private GameBoard gameBoard;

    private void Awake()
    {
        globalTimer = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        gameBoard = Instantiate(gameBoard.transform).GetComponent<GameBoard>();
        for (int i = 0; i < 3; i++)
        {
            gameBoard.SpawnEnemy((i, 0));
        }
    }

    // Update is called once per frame
    void Update()
    {
        globalTimer += Time.deltaTime;
        if (globalTimer >= TIC_TIME)
        {
            globalTimer = 0f;
            gameBoard.Tic();
        }
    }
}
