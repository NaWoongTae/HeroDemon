using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : UNIT
{
    [SerializeField] GameObject effect;
    [SerializeField] Transform effPos;

    private void Awake()
    {
        Awake_Init();
    }

    // Update is called once per frame
    void Update()
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
                _Idle();
                break;
            case Action.TRACE:
                _Trace();
                break;
            case Action.ATTACK:
                _Move();
                break;
            case Action.HEAL:
                _Heal(true);
                break;
            case Action.HOLD:
                _Hold();
                break;
            case Action.DIE:
                _Die();
                break;
        }
    }

    void _Heal(bool goTrace)
    {        
        if (PERPOSE.whatTarget == null && goTrace)
        {
            SetState(prevState);
            return;
        }

        bool isDamagedUnit = false;
        for (int i = 0; i < PERPOSE.TARGETs._Count; i++)
        {
            if (PERPOSE.TARGETs[i].GetComponent<Filter>().containScript<UNIT>() && PERPOSE.TARGETs[i].GetComponent<Filter>().GetDamagedHp())
            {
                PERPOSE.FLASHTARGET = PERPOSE.TARGETs[i];
                isDamagedUnit = true;
                break;
            }
        }
        Debug.Log(isDamagedUnit);
        if (isDamagedUnit)
        {
            Debug.Log(PERPOSE.whatTarget.name);
            Vector3 lookpos = new Vector3(PERPOSE.whatTarget.transform.position.x, transform.position.y, PERPOSE.whatTarget.transform.position.z);
            transform.LookAt(lookpos);
            ANI.SetBool("ATTACK", true);
            ANI.SetBool("MOVE", false);

            if (Vector3.Distance(transform.position, PERPOSE.whatTarget.transform.position) > UNITSTATUS.RANGE + PERPOSE.whatTarget.GetComponent<CapsuleCollider>().radius && goTrace)
            {
                SetState(Action.TRACE);
            }
        }
    }

    new public void Attack()
    {
        if (PERPOSE.whatTarget != null)
        {
            PERPOSE.whatTarget.GetComponent<UNIT>().GetHeal(UNITSTATUS.STR(true));
            GameObject go = Instantiate(effect, effPos.position, effPos.rotation);
            Destroy(go, 1f);
        }
    }
}
