﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selection : MonoBehaviour
{
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // sync animations. Not really sure how this works.
        // https://forum.unity.com/threads/how-to-sync-three-animations.332271/
        anim.Play(0, -1, Time.time);
    }
}
