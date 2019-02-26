using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragCheckBox : MonoBehaviour
{
    CtrlManager CTM;
    bool fst;

    void Awake()
    {
        fst = true;
        CTM = GetComponentInParent<CtrlManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "KINGDOM") // 땅과 기타 지형을 제외하고
        {
            CTM.nowSelect(other.gameObject, fst);            
            fst = false;
        }
    }
}
