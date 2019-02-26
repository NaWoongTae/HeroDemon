using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveEnemyUnit : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "DUMMY")
        {
            other.GetComponent<Filter>().goAttack(KingdomManager.Home[0].transform.position);
        }
    }
}
