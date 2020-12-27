using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Animator anim;
    private SpriteRenderer sr;

    public static Quaternion? GetAngle(Vector2Int direction)
    {
        if (direction == Vector2Int.left)
            return Quaternion.Euler(new Vector3(0, 0, 0));
        else if (direction == Vector2Int.down)
            return Quaternion.Euler(new Vector3(0, 0, 90));
        else if (direction == Vector2Int.right)
            return Quaternion.Euler(new Vector3(0, 0, 180));
        else if (direction == Vector2Int.up)
            return Quaternion.Euler(new Vector3(0, 0, 270));
        return null;
    }


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        //sr.sortingLayerName = "Lightning"; // TODO change this layer
        //sr.color = new Color(1f, 1f, 1f, .3f); // make transparent
        // anim.SetInteger("id", 0);
    }
}
