using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSight : MonoBehaviour
{
    public enum tagType { DUMMY, KINGDOM, MINERAL}
    [SerializeField] tagType CheckTag;
    UNIT unit;
    bool mustFindMineral;

    public bool MUSTFIND_MINERAL
    {
        set { mustFindMineral = value; }
    }

    UNIT _unit
    {
        get
        {
            if(unit == null)
                unit = GetComponentInParent<UNIT>();
            return unit;
        }
    }

    bool combat;
    int nearest;

    void Awake()
    {
        combat = false;
        nearest = -1;
        mustFindMineral = false;
    }

    public bool COMBAT
    {
        set { combat = value; }
    }

    // Use this for initialization
    void Start ()
    {
        unit = GetComponentInParent<UNIT>();
    }

    // Update is called once per frame
    void Update()
    {        
        if (unit.PERPOSE.TARGETs._Count == 0)
        {
            nearest = -1;
            combat = false;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (_unit == null || unit.state == UNIT.Action.GATHER || other.tag == "LAND")
            return;

        // 내가 아닌 처음보는 적 == (가지고 있지 않고 // 태그가 맞고 // 스스로가 아닌)
        if (unit.GetComponent<Filter>().containScript<Worker>() && unit.state == UNIT.Action.TRACE)
        {
            OnMineral(other);
        }
        else if(unit.UNITSTATUS.NAME.Equals("HEALER") && other.tag == CheckTag.ToString() && other.GetComponent<Filter>().TYPE == Filter.UnitType.KS)
        {
            return;
        }
        else if (!combat && other.tag == CheckTag.ToString() && other.gameObject != unit.gameObject)// 전투중이 아니고 && 타겟이 적이며 && 스스로가 아닐때
        {
            unit.PERPOSE.TARGETs.Add(other.gameObject);
            combat = true;

            unit.FindEnemy(unit.PERPOSE.TARGETs.First());
        }
        else if(other.tag == CheckTag.ToString() && other.gameObject != unit.gameObject && !unit.PERPOSE.TARGETs.Contains(other.gameObject))
        {
            unit.PERPOSE.TARGETs.Add(other.gameObject);
        }

        /*if (unit.PERPOSE.TARGETs._Count > 0)
        {
            distanceCompare();

            if (unit.Attackable)
            {
                unit.FindEnemy(unit.PERPOSE.TARGETs.First());
            }
        }*/

        unit.FINDNEXTATT = false;
    }

    void OnMineral(Collider other)
    {
        if (other.tag == CheckTag.ToString()) // 태그 일치(미네랄)
        {
            unit.GetComponent<Worker>().PERPOSE.Added(other.GetComponent<mineral>());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (_unit.PERPOSE.TARGETs.Contains(other.gameObject))
        {
            bool result = _unit.PERPOSE.TARGETs.RemoveAt(other.gameObject);
            if (!result)
            {
                for (int i = 0; i < _unit.PERPOSE.TARGETs._Count; i++)
                {
                    if (_unit.PERPOSE.TARGETs[i] == null)
                    {
                        _unit.PERPOSE.TARGETs.Remove(i);
                        i--;
                    }
                }
            }
        }   
    }

    void distanceCompare()
    {
        for (int i = 0; i < unit.PERPOSE.TARGETs._Count; i++)
        {
            if (unit.PERPOSE.TARGETs.GetT(i) == null)
            {
                unit.PERPOSE.TARGETs.Remove(i);
                continue;
            }

            if (i == 0)
                nearest = 0;
            else 
            {
                if (Vector3.Distance(unit.transform.position, unit.PERPOSE.TARGETs.GetT(i).transform.position) <
                    Vector3.Distance(unit.transform.position, unit.PERPOSE.TARGETs.GetT(0).transform.position))
                {
                    unit.PERPOSE.TARGETs.Swap(0, i);
                }
            }
        }
    }

    public GameObject GetNearestEnemy()
    {
        if (unit.PERPOSE.TARGETs._Count >= 0)
            return unit.PERPOSE.TARGETs.GetT(0);
        else
            return null;
    }

    public void inGather()
    {
        CheckTag = tagType.MINERAL;
        GetComponent<SphereCollider>().radius = 12;
        MUSTFIND_MINERAL = true;
    }

    public void inNormal(UnitStatus us)
    {
        CheckTag = tagType.DUMMY;
        GetComponent<SphereCollider>().radius = us.NOTICE;
        MUSTFIND_MINERAL = false;
    }
}
