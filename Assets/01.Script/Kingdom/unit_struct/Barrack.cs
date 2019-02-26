using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrack : Structure
{
    void Start()
    {
        Init_Start();
        StartCoroutine(Init_Update());
    }
}
