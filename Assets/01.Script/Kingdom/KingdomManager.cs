using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class KingdomManager : ManagerSetting
{
    public static SelectObject isSelct;
    public static List<GameObject> Home;
    public static int Mine;
    public static int popular_all;
    public static int popular_part;
    public static Grid _Grid;
    public static bool setting = false;
    public static Dictionary<string, Dictionary<string, string>> usingData;

    WorldSpaceUI WSU;
    [SerializeField] Text msg;
    [SerializeField] GameObject RallyPoint;
    int textWait = 0;

    public bool isHave
    {
        get { return isSelct.SelectObj.Count > 0; }
    }

    public int size = 1;
    public GameObject tile;
    public GameObject panel;
    string buildname;

    public string buildN
    {
        get { return buildname; }
        set { buildname = value; }
    }

    Vector3 pos;    

    CtrlManager CTM;
    UIManager UIM;
    MoveCamera MC;

    private void Awake()
    {
        usingData = ValueCopy(DataMng.instance.Get(LowDataType.UPGRADE).NODE);

        Mine = 2000;
        popular_all = 10;
        popular_part = 0;
        isSelct = new SelectObject();
        Home = new List<GameObject>();
        MngPack.instance.KDM = this;        
    }

    bool _setting()
    {
        _Grid = GetComponent<Grid>();
        if (!_Grid._Init())
            return false;

        return true;
    }

    public override void settingManager()
    {
        bool result = true;
        result = Kingdom_Init();
        if (!result)
        {
            settingSuccess = false;
        }

        result = MC.Init();
        if (!result)
        {
            settingSuccess = false;
        }

        settingSuccess = result;
    }

    // Use this for initialization
    bool Kingdom_Init()
    { 
        CTM = GetComponentInChildren<CtrlManager>();
        UIM = GetComponentInChildren<UIManager>();
        WSU = GameObject.Find("WorldUICanvas").GetComponent<WorldSpaceUI>();
        MC = GetComponentInChildren<MoveCamera>();
        GetComponentInChildren<MapObject>().Init();
        CTM.Init(this);
        UIM.Init(this);
        WSU.Init(this);

        setting = _setting();

        Cursor.lockState = CursorLockMode.Confined;
        GetComponentInChildren<StartMapMaker>().init();
        StartCoroutine(MngPack.instance.MOB.mapObject());

        UIM.ScreenInterface();

        return true;
    }

    // Update is called once per frame
    void Update()
    {
        posCheck();
    }
    
    // 건물지을 자리 사각형으로 보여주는거같음
    void posCheck()
    {
        if (EventSystem.current.IsPointerOverGameObject() == false)
        {
            if (isSelct.Building)
            {
                List<GameObject> blueprint = isSelct.SelectObj[0].GetComponent<Worker>().BLUE_P;
                Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(r, out hit, 100, 512))
                {
                    if (!isSelct.SelectObj[0].GetComponent<Worker>().POSS)
                    {
                        pos = setPos(hit.point);
                        isSelct.SelectObj[0].GetComponent<Worker>().BUILD_BP.transform.position = pos;
                        isSelct.SelectObj[0].GetComponent<Worker>().BUILD_N = buildN;
                    }
                }
            }
        }
    }

    public Vector3 setPos(Vector3 vec)
    {
        float w = 0, h = 0, l = 0;

        if (vec.x < size / 2)
            vec.x = size / 2;
        else if (vec.x > _Grid._MapSizeW - size / 2)
            vec.x = _Grid._MapSizeW - size / 2;

        if (vec.z < size / 2)
            vec.z = size / 2;
        else if (vec.z > _Grid._MapSizeH - size / 2)
            vec.z = _Grid._MapSizeH - size / 2;

        if (size % 2 == 1)
        {
            w = (int)vec.x + 0.5f;
            l = (int)vec.z + 0.5f;
        }
        else if (size % 2 == 0)
        {
            w = (vec.x - (int)vec.x >= 0.5f) ? (int)vec.x + 1 : (int)vec.x;
            l = (vec.z - (int)vec.z >= 0.5f) ? (int)vec.z + 1 : (int)vec.z;
        }

        h = vec.y;

        return new Vector3(w, h, l);
    }

    void showBlueprint(List<GameObject> blueprint)
    {
        Vector3 Opos = isSelct.SelectObj[0].GetComponent<Worker>().BUILD_BP.transform.position;
        if (size % 2 == 1)
        {
            int wing = (size - 1) / 2;

            for (int i = -wing; i < wing + 1; i++)
            {
                for (int j = -wing; j < wing + 1; j++)
                {
                    GameObject go = Instantiate(tile, isSelct.SelectObj[0].GetComponent<Worker>().BUILD_BP.transform);
                    go.transform.position = new Vector3(Opos.x + i, Opos.y + 0.2f, Opos.z + j);
                    blueprint.Add(go);
                }
            }
        }
        else if (size % 2 == 0)
        {
            int wing = size / 2;
            for (int i = -wing; i < wing; i++)
            {
                for (int j = -wing; j < wing; j++)
                {
                    GameObject go = Instantiate(tile, isSelct.SelectObj[0].GetComponent<Worker>().BUILD_BP.transform);
                    go.transform.position = new Vector3(Opos.x + i + 0.5f, Opos.y + 0.2f, Opos.z + j + 0.5f);
                    blueprint.Add(go);
                }
            }
        }
    }

    public void removeNodeinGrid(int[][] nd)
    {
        for (int i = 0; i < nd.Length; i++)
        {
            _Grid.GRID[nd[i][0], nd[i][1]].buildable = true;
        }
    }

    public void Build_Ready(string str)
    {
        buildname = str;
        size = int.Parse(DataMng.instance.Get(LowDataType.BUILD).NODE[buildname][BUILD.Index.SIZE.ToString()]);
        isSelct.SelectObj[0].GetComponent<Worker>().BUILD_BP = Instantiate(DataMng.instance.PREFAB_G[buildname + "_BP"], panel.transform);
        isSelct.SelectObj[0].GetComponent<UNIT>().standbyOrder(UNIT.StandByAction.BUILD);
        isSelct.SelectObj[0].GetComponent<Worker>().BUILDRD = true;
        if (isSelct.SelectObj[0].GetComponent<Worker>().BLUE_P == null ||
            isSelct.SelectObj[0].GetComponent<Worker>().BLUE_P.Count == 0)
        {
            isSelct.SelectObj[0].GetComponent<Worker>().BLUE_P = new List<GameObject>();
            showBlueprint(isSelct.SelectObj[0].GetComponent<Worker>().BLUE_P);
        }
    }

    //===========================================================================

    void SelectAdd(GameObject go)
    {
        if (isSelct.SelectObj.Count < 12)
            isSelct.SelectObj.Add(go);
    }

    public void SelectRemove(GameObject go)
    {
        if (isSelct.SelectObj.Contains(go))
            isSelct.SelectObj.Remove(go);
        reflesh();
    }

    public void nowSelect(GameObject other, bool fst)
    {
        if (fst) // 처음이라면 그전에 남아있던 선택된 오브젝트를 해제
        {
            isSelct.SelectOff();// 이전 선택 해제

            SelectAdd(other.gameObject); // 새로운 선택(첫) 추가
        }
        else
        {
            if (other.GetComponent<Filter>().TYPE != Filter.UnitType.KS && KingdomManager.isSelct.selectMine()) // 새로들어온게 유닛이고 내유닛일때
            {
                if (isSelct.SelectType == Filter.UnitType.KS || isSelct.SelectType == Filter.UnitType.MNR) // 원래있던게 건물이거나 자원이면
                {
                    isSelct.SelectOff();

                    SelectAdd(other.gameObject); // 다시추가
                }
                else if (isSelct.SelectType == Filter.UnitType.KUC) // 원래잇던게 유닛이면
                {
                    SelectAdd(other.gameObject); // 추가
                }
            }
        }

        isSelct.SignView(true);

        UIM.ScreenInterface();
        WSU.SettingHP();
        // reflesh();

        for(int i = 0; i < isSelct.Count; i++)
        {
            isSelct.SelectObj[i].GetComponent<Filter>().WhenSelected();
        }
    }

    void reflesh()
    {
        UIM.ScreenInterface();
        UIM.isButtonSelect(UIManager.buttonType.normalstate);
        WSU.SettingHP();
    }

    public void buildinit()
    {
        buildN = "";
    }

    public IEnumerator MsgDebug(string str)
    {
        textWait++;
        msg.text = str;

        yield return new WaitForSeconds(3f);

        if (textWait == 1)
        {
            msg.text = "";
            textWait--;
        }
    }

    public void SetRally(bool isRally, Vector3 RallyPos)
    {
        RallyPoint.SetActive(isRally);
        if (isRally)
        {
            RallyPoint.transform.position = RallyPos;
        }
    }

    Dictionary<string, Dictionary<string, string>> ValueCopy(Dictionary<string, Dictionary<string, string>> dic)
    {
        Dictionary<string, Dictionary<string, string>> cpy = new Dictionary<string, Dictionary<string, string>>();

        foreach (string str in dic.Keys)
        {
            Dictionary<string, string> d = new Dictionary<string, string>();

            foreach (string inKey in dic[str].Keys)
            {
                d.Add(inKey, dic[str][inKey]);
            }

            cpy.Add(str, d);
        }

        return cpy;
    }
}
