using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : Structure {

    enum eTurretState { IDLE, ATTACK }
    eTurretState state;

    enum tagType { DUMMY, KINGDOM, MINERAL }
    [SerializeField] tagType CheckTag;

    [SerializeField] GameObject body;
    [SerializeField] Transform shotPosR;
    [SerializeField] Transform shotPosL;

    UnitSight unitSight;
    GameObject target;
    Quaternion originQ;
    int STR = 500;
    bool combat;

    // Update is called once per frame
    void Start()
    {
        state = eTurretState.IDLE;
        combat = false;
        Init_Start();
        unitSight = GetComponentInChildren<UnitSight>();
        originQ = body.transform.rotation;
    }

    private void Update()
    {
        CheckState();
        _selectSign();
    }

    void CheckState()
    {
        switch (state)
        {
            case eTurretState.IDLE:
                body.transform.Rotate(Vector3.up * 30 * Time.deltaTime);
                break;
            case eTurretState.ATTACK:
                if (target != null)
                {
                    Vector3 lookrotation = body.transform.position - new Vector3(target.transform.position.x, body.transform.position.y, target.transform.position.z);
                    if (lookrotation != Vector3.zero)
                    {
                        body.transform.rotation = Quaternion.Slerp(body.transform.rotation, Quaternion.LookRotation(lookrotation), 20f * Time.deltaTime);
                    }
                }
                break;
        }
    }

    IEnumerator Attack()
    {        
        int turn = 0;

        while (true)
        {
            if (target == null || target.GetComponent<Filter>().GetHp() <= 0)
            {
                target = null;
                state = eTurretState.IDLE;
                combat = false;
                break;
            }
            else
            {
                GameObject gfo = Instantiate(DataMng.instance.PREFAB_G["SHELL"], (turn % 2 == 0) ? shotPosR.position : shotPosL.position, transform.rotation, transform);
                gfo.GetComponent<Arrow>().Settings(target, Arrow.Projectile.shell, STR);
            }

            turn++;
            yield return new WaitForSeconds(2f);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag != CheckTag.ToString())
            return;

        if (!combat &&  other.gameObject != gameObject)// 전투중이 아니고 && 타겟이 적이며 && 스스로가 아닐때
        {
            target = other.gameObject;
            combat = true;
            state = eTurretState.ATTACK;
            StartCoroutine(Attack());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == target)
        {
            StopCoroutine(Attack());
            target = null;
        }
    }
}
