using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move_Info {

    Vector3 _MoveGroundPos;
    Vector3 _PatrolstartPos;
    GameObject _MoveTarget;

    public Vector3 MoveGroundPos
    {
        get { return _MoveGroundPos; }
        set
        {
            _MoveTarget = null;
            _MoveGroundPos = value;
        }
    }

    public Vector3 PatrolStartPos
    {
        get { return _PatrolstartPos; }
        set { _PatrolstartPos = value; }
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

    public Move_Info()
    {
        _MoveGroundPos = Vector3.zero;
        _MoveTarget = null;
    }
}
