using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure : usableObject
{

    [SerializeField] Transform hpbarPos;
    StructStatus _structstatus;
    public bool Crushed;
    
    public StructStatus STRUCTSTATUS
    {
        get { return _structstatus; }
        set { _structstatus = value; }
    }

    Heap<string> produce; // 생산라인
    float produceTime;
    bool isAssem;
    protected Vector3 AssemPos;
    GameObject AssemGo;

    UNITDATA.type tp;
    UPGRADE.Name nm;

    UIManager.buttonType nowState;
    public UIManager.buttonType NOWSTATE
    {
        get { return nowState; }
        set { nowState = value; }
    }

    public Heap<string> PRODUCE
    {
        get { return produce; }        
    }
    public float PdTime
    {
        get { return produceTime; }
        set { produceTime = value; }
    }

    public Transform HPBARPOS
    {
        get { return hpbarPos; }
    }

    void Awake()
    {
        nowState = UIManager.buttonType.normalstate;
        _structstatus = new StructStatus();
        produce = new Heap<string>();
        produceTime = 0;
        Crushed = false;
        isAssem = false;
        init();
    }

    public void Init(StructStatus strt, Vector3 rallyPos)
    {        
        STRUCTSTATUS = strt;
        MngPack.instance.MOB.Insert(transform, GetComponent<Filter>().TYPE);
        if (rallyPos != Vector3.zero)
        {
            SetAssem(rallyPos);
        }
    }

    protected void Init_Start()
    {
        if (KingdomManager.isSelct.SelectObj.Contains(gameObject))
        {
            MngPack.instance.UIM.ScreenInterface();
        }

        STRUCTSTATUS.DELAY = -1;

        for (int i = 0; i < STRUCTSTATUS.SIZE * STRUCTSTATUS.SIZE; i++)
        {
            KingdomManager._Grid.GRID[(int)(STRUCTSTATUS.NODE[i][0]), STRUCTSTATUS.NODE[i][1]].buildable = false;
        }
    }

    // Update is called once per frame
    protected IEnumerator Init_Update ()
    {
        yield return new WaitForEndOfFrame();
        while (true)
        {
            if (!_structstatus.PRODUCE.Equals("NULL"))
                Making();
            _selectSign();

            yield return null;
        }
    }

    /// <summary>
    /// 선택시
    /// </summary>
    public override void WhenSelected()
    {
        MngPack.instance.KDM.SetRally(isAssem, (AssemGo == null) ? AssemPos : AssemGo.transform.position);

        if (KingdomManager.isSelct.isContain(gameObject) && produce._Count > 0)
            MngPack.instance.UIM.UI_Update(produce);
    }

    /// <summary>
    /// 선택 해제시
    /// </summary>
    public override void WhenOff()
    {
        MngPack.instance.KDM.SetRally(false, Vector3.zero);
    }

    public void Product(string reserve)
    {
        bool result = true;

        if (produce._Count < 5)
        {
            if (STRUCTSTATUS.PRODUCE.Equals("UNITDATA"))
            {
                int num = (int)EnumHelper.StringToEnum<UNITDATA.type>(reserve);
                result = MngPack.instance.UIM.use_mine(int.Parse(DataMng.instance.Get(LowDataType.UNITDATA).NODE[num.ToString()]["COST"]));
            }
            else
            {
                nm = EnumHelper.StringToEnum<UPGRADE.Name>(reserve);

                int num = (int)EnumHelper.StringToEnum<UPGRADE.Name>(reserve);
                result = MngPack.instance.UIM.use_mine(int.Parse(KingdomManager.usingData[num.ToString()]["COST"]));

                if (result)
                {
                    KingdomManager.isSelct.upgrade_enable[((int)nm)] = true;
                }
            }

            if (result)
            {
                produce.Add(reserve);

                nowState = UIManager.buttonType.produce;
                MngPack.instance.UIM.ScreenInterface();
                MngPack.instance.UIM.UI_Update(produce);
            }
        }
        else
            StartCoroutine(MngPack.instance.KDM.MsgDebug("대기열이 가득찼습니다."));
        
    }

    /// <summary>
    /// 유닛 생성 루틴
    /// </summary>
    void Making() 
    {
        if (produce._Count > 0)
        {
            produceTime += Time.deltaTime;

            if (STRUCTSTATUS.PRODUCE.Equals("UNITDATA"))
            {
                tp = EnumHelper.StringToEnum<UNITDATA.type>(produce.First());
                STRUCTSTATUS.DELAY = int.Parse(DataMng.instance.Get(LowDataType.UNITDATA).NODE[((int)tp).ToString()]["DELAY"]);
            }
            else
            {
                nm = EnumHelper.StringToEnum<UPGRADE.Name>(produce.First());
                STRUCTSTATUS.DELAY = int.Parse(KingdomManager.usingData[((int)nm).ToString()]["DELAY"]);
            }

            if (produceTime > STRUCTSTATUS.DELAY)
            {
                produce.RemoveFirst();
                produceTime = 0;

                if (KingdomManager.isSelct.isContain(gameObject))
                    MngPack.instance.UIM.UI_Update(produce);

                if (STRUCTSTATUS.PRODUCE.Equals("UNITDATA"))
                {
                    GameObject go = Instantiate(DataMng.instance.PREFAB_G[tp.ToString()],
                    _makingPosMaker(), DataMng.instance.PREFAB_G[tp.ToString()].transform.rotation);
                    go.GetComponent<UNIT>()._Init(tp.ToString(), true);

                    if (isAssem)
                    {
                        if (AssemGo != null)
                        {
                            go.GetComponent<UNIT>()._MoveSet(AssemGo);
                        }
                        else
                        {
                            go.GetComponent<UNIT>()._MoveSet(AssemPos);
                        }
                    }
                }
                else
                {
                    switch (nm)
                    {
                        case UPGRADE.Name.HERO_LEARN_SKILL_1:
                            KingdomManager.isSelct.skill[0] = true;
                            DataMng.instance.Get(LowDataType.UNITUI).NODE["LABORATORY"][nm.ToString()] = "BLOCK";
                            break;
                        case UPGRADE.Name.HERO_LEARN_SKILL_2:
                            KingdomManager.isSelct.skill[1] = true;
                            DataMng.instance.Get(LowDataType.UNITUI).NODE["LABORATORY"][nm.ToString()] = "BLOCK";
                            break;
                        case UPGRADE.Name.HERO_LEARN_SKILL_3:
                            KingdomManager.isSelct.skill[2] = true;
                            DataMng.instance.Get(LowDataType.UNITUI).NODE["LABORATORY"][nm.ToString()] = "BLOCK";
                            break;
                        case UPGRADE.Name.RANGE_UP:
                            KingdomManager.isSelct.rangeUp = true;
                            DataMng.instance.Get(LowDataType.UNITUI).NODE["LABORATORY"][nm.ToString()] = "BLOCK";
                            break;
                        case UPGRADE.Name.STR_UP:
                            KingdomManager.isSelct.str_count += 1;

                            KingdomManager.usingData[((int)nm).ToString()]["COST"] = 
                                (int.Parse(KingdomManager.usingData[((int)nm).ToString()]["COST"]) + int.Parse(KingdomManager.usingData[((int)nm).ToString()][UPGRADE.Index.INCREASE.ToString()])).ToString();

                            if (KingdomManager.isSelct.str_count >= 3)
                                DataMng.instance.Get(LowDataType.UNITUI).NODE["LABORATORY"][nm.ToString()] = "BLOCK";
                            else
                                KingdomManager.isSelct.upgrade_enable[(int)nm] = false;

                            break;
                        case UPGRADE.Name.DEF_UP:
                            KingdomManager.isSelct.def_count += 1;

                            KingdomManager.usingData[((int)nm).ToString()]["COST"] =
                                (int.Parse(KingdomManager.usingData[((int)nm).ToString()]["COST"]) + int.Parse(KingdomManager.usingData[((int)nm).ToString()][UPGRADE.Index.INCREASE.ToString()])).ToString();

                            if (KingdomManager.isSelct.def_count >= 3)
                                DataMng.instance.Get(LowDataType.UNITUI).NODE["LABORATORY"][nm.ToString()] = "BLOCK";
                            else
                                KingdomManager.isSelct.upgrade_enable[(int)nm] = false;

                            break;                        
                    }
                }
            }
            if (produce._Count == 0 && KingdomManager.isSelct.SelectObj[0] == gameObject)
            {
                nowState = UIManager.buttonType.normalstate;
                MngPack.instance.UIM.ScreenInterface();
                MngPack.instance.UIM.isButtonSelect(UIManager.buttonType.normalstate);
            }
        }
    }

    void Making_struct()
    {
        if (!STRUCTSTATUS.COMPLETE)
        {
            produceTime += Time.deltaTime;

            if (produceTime > float.Parse(DataMng.instance.Get(LowDataType.UNITDATA).NODE[STRUCTSTATUS.NAME]["DELAY"]))
            {
                produceTime = 0;
            }
        }
    }

    public void RemoveFirst()
    {
        produce.RemoveFirst();
    }

    /// <summary>
    /// 생성위치 설정
    /// </summary>
    /// <returns></returns>
    Vector3 _makingPosMaker()
    {
        Vector3 vec = transform.position;
        RaycastHit hit;

        Vector3[] way = new Vector3[4] { Vector3.left, Vector3.forward, Vector3.right, Vector3.back };

        if (STRUCTSTATUS.SIZE % 2 == 1)
            vec += new Vector3((STRUCTSTATUS.SIZE + 1) / 2, 0, -(STRUCTSTATUS.SIZE + 1) / 2);        
        else
            vec += new Vector3(STRUCTSTATUS.SIZE / 2 + 0.5f, 0, -(STRUCTSTATUS.SIZE / 2 + 0.5f));

        int start = STRUCTSTATUS.SIZE + 1;
        if (!Physics.Raycast(vec + new Vector3(0, 0.5f, 0), Vector3.up, out hit, 20))
        {
            for (int k = 0; k < 3; k++)
            {
                for (int j = 0; j < 4; j++)
                {
                    for (int i = 0; i < start; i++)
                    {
                        vec += way[j];

                        if (!Physics.Raycast(vec + new Vector3(0, -0.5f, 0), Vector3.up, out hit, 2, 1 << 11))
                        {
                            return vec;
                        }
                    }
                }
                start += 2;
                vec += new Vector3(1, 0, -1);
            }
        }

        return vec; // null
    }

    public void structDestroy(float time = 0.0f)
    {
        GetComponent<CapsuleCollider>().isTrigger = true;
        Circle.SetActive(false);
        MngPack.instance.KDM.SelectRemove(this.gameObject);
        MngPack.instance.KDM.removeNodeinGrid(STRUCTSTATUS.NODE);
        Destroy(this.gameObject, time);
    }

    public bool GetDamage(float Damage)
    {
        STRUCTSTATUS.HP -= Damage;
        if (STRUCTSTATUS.HP <= 0)
        {
            Crushed = true;
            GetComponent<CapsuleCollider>().enabled = false;
            structDestroy(1.5f);
            MngPack.instance.KDM.SelectRemove(gameObject);
            return true;
        }

        return false;
    }

    public void SaveMine(int m)
    {
        KingdomManager.Mine += m;
    }

    public void SetAssem(Vector3 vec)
    {
        if (!STRUCTSTATUS.RALLY)
            return;

        isAssem = true;
        AssemPos = vec;
        MngPack.instance.KDM.SetRally(isAssem, (AssemGo == null) ? AssemPos : AssemGo.transform.position);
    }


    public void SetAssem(GameObject go)
    {
        if (!STRUCTSTATUS.RALLY)
            return;

        isAssem = true;
        AssemGo = go;
        if (go.Equals(gameObject))
        {
            isAssem = false;
            AssemGo = null;
            AssemPos = Vector3.zero;
        }
        else
            MngPack.instance.KDM.SetRally(isAssem, (AssemGo == null) ? AssemPos : AssemGo.transform.position);
    }

    protected void GetStatus(string str)
    {
        Dictionary<string, Dictionary<string, string>> data = DataMng.instance.Get(LowDataType.BUILD).NODE;

        STRUCTSTATUS = new StructStatus(str, false,
            int.Parse(data[str][BUILD.Index.SIZE.ToString()]),
            float.Parse(data[str][BUILD.Index.HP.ToString()]),
            data[str][BUILD.Index.PRODUCE.ToString()],
            data[str][BUILD.Index.CONNECT.ToString()],
            int.Parse(data[str][BUILD.Index.VALUE.ToString()]),
            data[str][BUILD.Index.PRODUCE.ToString()],
            int.Parse(data[str][BUILD.Index.DELAY.ToString()]),
            int.Parse(data[str][BUILD.Index.RALLY.ToString()])
            );
    }

    public void manual_pos()
    {
        Vector3 Opos = transform.position;

        if (STRUCTSTATUS.SIZE % 2 == 1)
        {
            int wing = (STRUCTSTATUS.SIZE - 1) / 2;

            for (int i = -wing; i < wing + 1; i++)
            {
                for (int j = -wing; j < wing + 1; j++)
                {
                    STRUCTSTATUS.NODE[(wing + i) * STRUCTSTATUS.SIZE + wing + j] = new int[]{ (int)(Opos.x + i), (int)(Opos.z + j)};
                    KingdomManager._Grid.GRID[(int)(Opos.x + i), (int)(Opos.z + j)].buildable = false;
                }
            }
        }
        else if (STRUCTSTATUS.SIZE % 2 == 0)
        {
            int wing = STRUCTSTATUS.SIZE / 2;
            for (int i = -wing; i < wing; i++)
            {
                for (int j = -wing; j < wing; j++)
                {
                    STRUCTSTATUS.NODE[(wing + i) * STRUCTSTATUS.SIZE + wing + j] = new int[] { (int)(Opos.x + i + 0.5f), (int)(Opos.z + j + 0.5f) };
                    KingdomManager._Grid.GRID[(int)(Opos.x + i + 0.5f), (int)(Opos.z + j + 0.5f)].buildable = false;
                }
            }
        }
    }
}
