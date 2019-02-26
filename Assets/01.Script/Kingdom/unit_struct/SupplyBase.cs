using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupplyBase : Structure
{
    private void Start()
    {
        KingdomManager.Home.Add(this.gameObject);
        Init_Start();
        StartCoroutine(Init_Update());
    }
}
