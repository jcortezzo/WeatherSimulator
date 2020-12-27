using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyEnemy : Piece
{
    // Update is called once per frame
    void Update()
    {
        finalPosition = new Vector2(GlobalManager.Instance.GameBoard.playerLocation.Item1, 
                                    GlobalManager.Instance.GameBoard.playerLocation.Item2);
    }
}
