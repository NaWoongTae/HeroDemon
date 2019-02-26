using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingCastle : Structure
{
    // Use this for initialization
    void Start()
    {        
        KingdomManager.Home.Add(this.gameObject);
        MngPack.instance.MOB.Insert(transform, GetComponent<Filter>().TYPE);

        Init_Start();
        StartCoroutine(Init_Update());
    }
}
