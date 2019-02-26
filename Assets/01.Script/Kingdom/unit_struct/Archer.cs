using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : UNIT
{
    [SerializeField] Transform arrowPos;

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

    new public void Attack()
    {
        if (!FINDNEXTATT)
        {
            if (PERPOSE.whatTarget != null)
            {
                FINDNEXTATT = shotArrow(PERPOSE.whatTarget);
            }
            else
            {
                FINDNEXTATT = true;
            }
        }
        else
        {
            PERPOSE.TARGETs.RemoveAt(PERPOSE.whatTarget);

            if (PERPOSE.TARGETs._Count == 0)
            {
                SetState(prevState);
            }
            else
            {
                /*for (int i = 0; i < PERPOSE.TARGETs._Count; i++)
                {
                    if (PERPOSE.TARGETs[i] != null)
                    {
                        Debug.Log("새로찾기-아처");
                        FindEnemy(PERPOSE.TARGETs[i]);
                        break;
                    }
                    else
                    {
                        PERPOSE.TARGETs.RemoveAt(PERPOSE.TARGETs[i]);
                        i--;
                        continue;
                    }
                }*/
            }

            FINDNEXTATT = false;
        }        
    }

    bool shotArrow(GameObject go)
    {
        GameObject gfo = Instantiate(DataMng.instance.PREFAB_G["ARROW"], arrowPos.position, transform.rotation, transform);
        gfo.GetComponent<Arrow>().Settings(go, Arrow.Projectile.arrow, UNITSTATUS.STR(gameObject.tag == "KINGDOM"));

        // 애니메이션 변경
        return false;
    }
}
