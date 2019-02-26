using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Filter : MonoBehaviour {

    public enum UnitType
    {
        KUH,
        KUC,
        KS,
        MNR,
        nothing
    }

    UNIT unit;
    Structure structure;
    mineral mnr;

    public UNIT Unit
    {
        get
        {
            if (unit == null)
            {
                unit = GetComponent<UNIT>();
            }
            return unit;
        }
    }

    public Structure STRUCTURE
    {
        get
        {
            if (structure == null)
            {
                structure = GetComponent<Structure>();
            }
            return structure;
        }
    }

    [SerializeField] UnitType type;

    public UnitType TYPE
    {
        get { return type; }
        set { type = value; }
    }

    void Start()
    {
        if (type == UnitType.KS)
            structure = GetComponent<Structure>();
        else if (type == UnitType.KUC)
            unit = GetComponent<UNIT>();
        else if (type == UnitType.MNR)
            mnr = GetComponent<mineral>();
    }
    
    public void WhenSelected()
    {
        if (type == UnitType.KUC || type == UnitType.KUH)
        {
            unit.WhenSelected();
        }
        if (type == UnitType.KS)
        {
            structure.WhenSelected();
        }
    }

    public void WhenOff()
    {
        if (type == UnitType.KUC || type == UnitType.KUH)
        {
            unit.WhenOff();
        }
        if (type == UnitType.KS)
        {
            structure.WhenOff();
        }
    }

    public void Adjustment(Vector3 targetPos)
    {
        if (type == UnitType.KUC || type == UnitType.KUH)
        {
            unit._MoveSet(targetPos);
        }
        if (type == UnitType.KS)
        {
            structure.SetAssem(targetPos);
        }
    }

    public void Adjustment(GameObject targetGam)
    {
        if (type == UnitType.KUC || type == UnitType.KUH)
        {
            unit._MoveSet(targetGam);
        }
        if (type == UnitType.KS)
        {
            structure.SetAssem(targetGam);
        }
    }

    public void middleOrder(RaycastHit hit)
    {
        if (type == UnitType.KUC || type == UnitType.KUH)
        {
            if (unit.StandBy != UNIT.StandByAction.BUILD) // 건설이 아니면
            {
                unit.GetOrder(hit);
            }
            else
            {
                if (GetComponent<Worker>().BUILD_OK)
                {
                    unit.GetOrder(hit);
                }
                else
                {
                    StartCoroutine(MngPack.instance.KDM.MsgDebug("이 지역에는 건설할 수 없습니다."));
                    GetComponent<Worker>().build_ready_Cancel();
                }
            }
        }
    }

    public void witchImage(string str)
    {
        if (type == UnitType.KUC || type == UnitType.KUH)
        {       
            unit.standbyOrder(EnumHelper.StringToEnum<UNIT.StandByAction>(str));
            Debug.Log(str);
        }
        else if(type == UnitType.KS)
        {
            STRUCTURE.Product(str);
        }
    }

    public void _selectSign(bool bl)
    {
        if (type == UnitType.KUC || type == UnitType.KUH)
        {
            unit.ImSelect(bl);
        }
        else if(type == UnitType.KS)
        {
            STRUCTURE.ImSelect(bl);
        }
        else if (type == UnitType.MNR)
        {
            GetComponent<mineral>().ImSelect(bl);
        }
    }

    public string GetName()
    {
        if (type == UnitType.KUC || type == UnitType.KUH)
        {
            return unit.UNITSTATUS.NAME;
        }
        else if (type == UnitType.KS)
        {
            return STRUCTURE.STRUCTSTATUS.NAME;
        }
        else
            return null;
    }

    public float GetHp()
    {
        if (type == UnitType.KUC || type == UnitType.KUH)
        {
            return unit.UNITSTATUS.PERCENT_HP;
        }
        else if (type == UnitType.KS)
        {
            return STRUCTURE.STRUCTSTATUS.PERCENT_HP;
        }
        else
            return 1;
    }

    public bool GetDamagedHp()
    {
        if (type == UnitType.KUC || type == UnitType.KUH)
        {
            return unit.UNITSTATUS.HP < unit.UNITSTATUS.MAXHP;
        }

        return false;
    }

    public UnitStatus GetUnitStatus()
    {
        return unit.UNITSTATUS;
    }

    public StructStatus GetStructStatus()
    {
        return STRUCTURE.STRUCTSTATUS;
    }

    public Vector3 GethpPos()
    {
        if (type == UnitType.KUC || type == UnitType.KUH)
        {
            return unit.HPBARPOS.position;
        }
        else if (type == UnitType.KS)
        {
            return STRUCTURE.HPBARPOS.position;
        }
        else
            return Vector3.zero;
    }

    public void Init(StructStatus strt, Vector3 rallyPos)
    {
        STRUCTURE.Init(strt, rallyPos);
    }

    public string Getstate()
    {
        if (type == UnitType.KUC || type == UnitType.KUH)
        {
            return unit.StandBy.ToString();
        }

        return "null";
    }

    public void GetOrder(UNIT.Action act, RaycastHit hit = new RaycastHit())
    {
        if (type == UnitType.KUC || type == UnitType.KUH)
        {
            switch (act)
            {
                case UNIT.Action.CANCEL:
                    unit.SetState(UNIT.Action.CANCEL);
                    unit.StandBy = UNIT.StandByAction.NOTHING;
                    break;
                case UNIT.Action.GATHER:
                    if (containScript<Worker>())
                    {
                        // unit.PERPOSE.AllClear();
                        unit.standbyOrder(UNIT.StandByAction.GATHER);
                        unit.PERPOSE.MoveTarget = null;
                        unit.GetOrder(hit);
                    }
                    unit.StandBy = UNIT.StandByAction.NOTHING;
                    break;
                default :
                    break;
            }            
        }
    }

    public bool containScript<T>()
    {
        if (gameObject.GetComponent<T>() == null)
            return false;

        return true;
    }

    public bool GetDamage(float dmg)
    {
        if (type == UnitType.KUC || type == UnitType.KUH)
        {
            return unit.GetDamage(dmg);
        }
        else if (type == UnitType.KS)
        {
            return structure.GetDamage(dmg);
        }

        return false;
    }

    public void YetSave(int money)
    {
        if (containScript<Worker>())
            unit.GetComponent<Worker>().Yetpaid += money;
        /*else
            StartCoroutine(MngPack.instance.KDM.MsgDebug("error : worker가 아닙니다리~"));*/
    }

    public void flick()
    {
        if (type == UnitType.KUC || type == UnitType.KUH)
        {
            unit.flicker();
        }
        else if (type == UnitType.KS)
        {
            structure.flicker();
        }
        else if (type == UnitType.MNR)
        {
            mnr.flicker();
        }
    }

    public void resetReady()
    {
        if (type == UnitType.KUC || type == UnitType.KUH)
        {
            unit.resetReady();
        }
    }

    public void goAttack(Vector3 vec)
    {
        Unit.PERPOSE.MoveGroundPos = vec;
        Unit.PERPOSE.MOVESPOT = Unit.PERPOSE.MoveGroundPos;
        Unit.SetState(UNIT.Action.ATTACKMOVE);
    }

}
