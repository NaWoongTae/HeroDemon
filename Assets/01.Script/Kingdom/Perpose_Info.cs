using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Perpose_Info
{
    Vector3 _MoveGroundPos;
    Vector3 _PatrolStartPos;
    GameObject _MoveTarget;
    Vector3 _MoveSpot;
    Vector3 _builpPos;
    Heap<GameObject> _targets;

    GameObject AttackTarget;
    GameObject flash_AttackTarget;

    bool isGather;
    int GetMineral;
    float stopRange;
    GameObject GatherTarget;
    List<mineral> Mineral;
    GameObject HomePos;
    
    public Vector3 MoveGroundPos
    {
        get { return _MoveGroundPos; }
        set
        {
            _MoveTarget = null;
            _MoveGroundPos = value;
        }
    }

    public GameObject MoveTarget
    {
        get { return _MoveTarget; }
        set
        {
            _MoveGroundPos = Vector3.zero;
            _MoveTarget = value;
        }
    }    

    public bool isGround
    {
        get { return _MoveGroundPos != Vector3.zero; }
    }

    public bool isTarget
    {
        get { return _MoveTarget != null; }
    }

    public Vector3 PatrolStartPos
    {
        get { return _PatrolStartPos; }
        set { _PatrolStartPos = value; }
    }

    //------------------------------------------------
    public Vector3 MOVESPOT
    {
        get { return _MoveSpot; }
        set { _MoveSpot = value; }
    }

    public Vector3 BUILDPOS
    {
        get { return  _builpPos; }
        set
        {
            if(value == Vector3.zero || _builpPos == Vector3.zero)
                _builpPos = value;
        }
    }

    public Vector3 BUILDorMOVE
    {
        get
        {
            if (BUILDPOS != Vector3.zero)
                return BUILDPOS;
            else if (MOVESPOT != Vector3.zero)
                return MOVESPOT;

            return Vector3.zero;
        }
    }
    //------------------------------------------------
    public GameObject ATTACKTARGET
    {
        get { return AttackTarget; }
        set
        {
            flash_AttackTarget = null;
            AttackTarget = value;
        }
    }

    public GameObject FLASHTARGET
    {
        get { return flash_AttackTarget; }
        set { flash_AttackTarget = value; }
    }

    public GameObject whatTarget
    {
        get
        {
            if (AttackTarget != null) // 지정한 적일때
            {
                if (AttackTarget.GetComponent<Filter>().containScript<UNIT>())
                {
                    if (AttackTarget.GetComponent<UNIT>().state != UNIT.Action.DIE)
                        return AttackTarget;
                    else
                    {
                        if (flash_AttackTarget != null)
                        {
                            if (flash_AttackTarget.GetComponent<UNIT>().state != UNIT.Action.DIE)
                                return flash_AttackTarget;
                        }

                        return null;
                    }
                }
                else if (AttackTarget.GetComponent<Filter>().containScript<Structure>())
                {
                    if (!AttackTarget.GetComponent<Structure>().Crushed)
                        return AttackTarget;
                    else
                    {
                        if (flash_AttackTarget != null)
                        {
                            if (flash_AttackTarget.GetComponent<Structure>().Crushed)
                                return flash_AttackTarget;
                        }
                        return null;
                    }
                }
                return null;
            }
            else if (flash_AttackTarget != null) // 플래시하게 설정된 적일때
            {
                if (flash_AttackTarget.GetComponent<Filter>().containScript<UNIT>())
                {
                    if (flash_AttackTarget.GetComponent<UNIT>().state != UNIT.Action.DIE)
                        return flash_AttackTarget;
                    
                    return null;
                }
                else if (flash_AttackTarget.GetComponent<Filter>().containScript<Structure>())
                {
                    if (!flash_AttackTarget.GetComponent<Structure>().Crushed)
                    {
                        return flash_AttackTarget;
                    }

                    return null;
                }
                else
                {
                    return null;
                }
            }
            return null;
        }
    }

    public Heap<GameObject> TARGETs
    {
        get { return _targets; }
        set { _targets = value; }
    }
    //------------------------------------------------
    public bool ISGATHER
    {
        get { return isGather; }
        set { isGather = value; }
    }

    public GameObject GatherTARGET
    {
        get { return GatherTarget; }
        set { GatherTarget = value; }
    }

    public List<mineral> MnR
    {
        get { return Mineral; }
        set { Mineral = value; }
    }
    
    public int GETMINERAL
    {
        get { return GetMineral; }
        set { GetMineral = value; }
    }

    public bool HAVEMINERAL
    {
        get { return (GetMineral > 0); }
    }

    public GameObject HOMEPOS
    {
        get { return HomePos; }
        set { HomePos = value; }
    }

    public float STOP_RANGE { get; set; }

    public void Added(mineral m)
    {
        if (MnR.Contains(m))
            return;
        else
            MnR.Add(m);
    }

    /// <summary>
    /// 다캔거 빼기
    /// </summary>
    public void refresh_mnr()
    {
        for (int i = 0; i < MnR.Count; i++)
        {
            if (MnR[i] == null)
            {
                MnR.RemoveAt(i);
                i--;
            }
        }
    }
    //------------------------------------------------
    public Perpose_Info()
    {
        _MoveGroundPos = Vector3.zero;
        _PatrolStartPos = Vector3.zero;
        _MoveTarget = null;
        AttackTarget = null;
        flash_AttackTarget = null;
        _targets = new Heap<GameObject>();
        MnR = new List<mineral>();
    }

    public void AllClear()
    {
        TARGETs.Clear();
        _MoveGroundPos = Vector3.zero;//
        _PatrolStartPos = Vector3.zero;//
        _MoveTarget = null;
        _MoveSpot = Vector3.zero;
        _builpPos = Vector3.zero;//

        AttackTarget = null;//
        flash_AttackTarget = null;//
    }
}
