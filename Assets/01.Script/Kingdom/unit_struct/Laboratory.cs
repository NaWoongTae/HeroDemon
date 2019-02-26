using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laboratory : Structure
{
    // Update is called once per frame
    void Start ()
    {
        Init_Start();
        StartCoroutine(Init_Update());
    }
}
