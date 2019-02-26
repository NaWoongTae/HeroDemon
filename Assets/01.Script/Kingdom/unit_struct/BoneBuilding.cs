using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneBuilding : Structure {

    [SerializeField] GameObject buildprocess_1;
    [SerializeField] GameObject buildprocess_2;
    int Yetpaid;

    public bool build(string buildname, int money = 0)
    {
        GetStatus(buildname);

        Yetpaid += money;

        StartCoroutine(startBuild(buildname));
        MngPack.instance.MOB.Insert(transform, GetComponent<Filter>().TYPE);        

        return true;
    }

    void Update()
    {
        _selectSign();
    }

    IEnumerator startBuild(string buildname)
    {
        PdTime = 0;
        STRUCTSTATUS.HP = 0;
        // STRUCTSTATUS.DELAY = 5; // 생산속도 잠시 5초
        while (true)
        {
            PdTime += Time.deltaTime;

            if (PdTime > STRUCTSTATUS.DELAY)
            {
                STRUCTSTATUS.COMPLETE = true;
                STRUCTSTATUS.HP += (STRUCTSTATUS.MAXHP / STRUCTSTATUS.DELAY * Time.deltaTime);
                STRUCTSTATUS.HP = (STRUCTSTATUS.HP > STRUCTSTATUS.MAXHP) ? STRUCTSTATUS.MAXHP : STRUCTSTATUS.HP;
                GameObject go = buildup(transform.position, buildname, buildname);
                go.GetComponent<Filter>().Init(STRUCTSTATUS, AssemPos);

                structbuildComplete(go);
                break;
            }
            else if (PdTime > STRUCTSTATUS.DELAY / 2)
            {
                buildprocess_1.SetActive(false);
                buildprocess_2.SetActive(true);
            }

            STRUCTSTATUS.HP += (STRUCTSTATUS.MAXHP / STRUCTSTATUS.DELAY * Time.deltaTime);

            yield return null;
        }
    }

    GameObject buildup(Vector3 pos, string stc, string stc_normal = "STRUCTBONE")
    {
        GameObject st_bp = Instantiate(DataMng.instance.PREFAB_G[stc_normal]);
        if (stc_normal.CompareTo("STRUCTBONE") == 0)
            st_bp.GetComponent<BoneBuilding>().build(stc);
        st_bp.transform.position = MngPack.instance.KDM.setPos(pos);

        return st_bp;
    }

    public void structbuildComplete(GameObject go)
    {
        GetComponent<CapsuleCollider>().isTrigger = true;
        Circle.SetActive(false);

        if (KingdomManager.isSelct.SelectObj.Contains(gameObject))
        {
            KingdomManager.isSelct.Change(go);
            go.GetComponent<Structure>().ImSelect(true);

            MngPack.instance.UIM.ScreenInterface();
        }        
        Destroy(gameObject);
    }

    void Cancel()
    {
        KingdomManager.Mine += Yetpaid;
        Destroy(gameObject);
    }
}
