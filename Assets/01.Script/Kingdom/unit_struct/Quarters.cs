using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quarters : Structure
{

    void Start()
    {
        KingdomManager.popular_all += STRUCTSTATUS.VALUE;
        Debug.Log("추가인구수 : " + STRUCTSTATUS.VALUE);
        MngPack.instance.UIM.update_popul();
        Init_Start();
        StartCoroutine(Init_Update());
    }
}
