using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalManager : MonoBehaviour
{
    public float globalTimer;
    public const int GAME_SCALE = 2;
    [SerializeField] private float TIC_TIME = 2f;
    [SerializeField] private GameBoard gameBoard;
    [SerializeField] private Piece piece;

    public Camera cam;

    private void Awake()
    {
        globalTimer = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        gameBoard = Instantiate(gameBoard.transform).GetComponent<GameBoard>();
        for (int i = 0; i < 3; i++)
        {
            gameBoard.SpawnEnemy((i, 0), piece);  // TODO: give piece prefab from Nguyen
        }
        Vector3 centerPos = gameBoard.GetCenterTile().transform.position;
        cam.transform.position = new Vector3(centerPos.x, centerPos.y, cam.transform.position.z);
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
