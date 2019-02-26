using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warrior : UNIT
{
    private void Awake()
    {
        Awake_Init();
    }

    // Update is called once per frame
    void Update ()
    {
        _selectSign();
        CheckState();
        Follow();
        extraRotation();
    }

    void CheckState()
    {
        switch (state)
        {
            case Action.MOVE:
            case Action.PATROL:
            case Action.ATTACKMOVE:
                _Move();                
                break;
            case Action.CANCEL:
            //case Action.ARRIVAL:
                _Idle();
                break;
            case Action.TRACE:
                _Trace();
                break;
            case Action.ATTACK:
                _Attack(true);
                break;
            case Action.HOLD:
                _Hold();
                break;
            case Action.DIE:
                _Die();
                break;
        }
    }
}
