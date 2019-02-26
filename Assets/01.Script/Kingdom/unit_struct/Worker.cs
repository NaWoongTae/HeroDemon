using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Worker : UNIT
{
    GameObject build_bp;
    List<GameObject> blueprint;
    bool buildingRd;
    bool PosSelect;
    bool buildOK;
    public int Yetpaid;
    string build_n;

    public bool BUILDRD
    {
        get { return buildingRd; }
        set { buildingRd = value; }
    }

    public bool BUILD_OK
    {
        get { return buildOK; }
        set { buildOK = value; }
    }

    public GameObject BUILD_BP
    {
        get { return build_bp; }
        set { build_bp = value; }
    }

    public List<GameObject> BLUE_P
    {
        get { return blueprint; }
        set { blueprint = value; }
    }

    public string BUILD_N
    {
        get { return build_n; }
        set { build_n = value; }
    }

    public int Blue_PCount
    {
        get
        {
            if (blueprint == null)
                return 0;
            else
                return blueprint.Count;
        }
    }

    public bool POSS
    {
        get { return PosSelect; }
    }

    // Use this for initialization
    void Awake()
    {
        Awake_Init();

        BUILD_OK = true;
        buildingRd = false;
        build_bp = null;
        PosSelect = false;
        blueprint = new List<GameObject>();
        Yetpaid = 0;
    }

    private void Start()
    {
        StartCoroutine(_Update());
        StartCoroutine(Late_Update());
    }

    // Update is called once per frame
    IEnumerator _Update()
    {
        while (true)
        {
            yield return null;

            _selectSign();
            CheckState();
            Follow();

            if (!BUILDRD && Blue_PCount > 0)
            {
                for (int i = 0; i < blueprint.Count; i++)
                {
                    Destroy(blueprint[i]);
                }
                blueprint = new List<GameObject>();
            }
            
            checkBuildable();
            extraRotation();
        }
    }

    IEnumerator Late_Update()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();

            if (KingdomManager.Home.Count > 0)
            {
                GameObject nearest = KingdomManager.Home[0];
                float compareDist = Vector3.Distance(KingdomManager.Home[0].transform.position, transform.position);
                for (int i = 1; i < KingdomManager.Home.Count; i++)
                {
                    float dist = Vector3.Distance(KingdomManager.Home[i].transform.position, transform.position);
                    if (compareDist > dist)
                    {
                        compareDist = dist;
                        nearest = KingdomManager.Home[i];
                    }
                }
                PERPOSE.HOMEPOS = nearest;
            }
        }
    }

    // 빌드할때 바닥 초록색/빨간색 보여주는거
    void checkBuildable()
    {
        if (blueprint == null)
            return;

        BUILD_OK = true;
        // 건물을 지을땐 ray에 체크되는 레이어를 ground만으로 변경
        for (int i = 0; i < blueprint.Count; i++)
        {
            if (BLUE_P[i].transform.position.x < 0 || BLUE_P[i].transform.position.z < 0)
            {
                blueprint[i].GetComponent<mapQuad>().setRed();
            }
            else if (KingdomManager._Grid.GRID[(int)(BLUE_P[i].transform.position.x), (int)(BLUE_P[i].transform.position.z)].buildable)
            {
                blueprint[i].GetComponent<mapQuad>().setGreen();
            }
            else
            {
                blueprint[i].GetComponent<mapQuad>().setRed();
                BUILD_OK = false;
            }
        }
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
            case Action.GATHER:
                _Gather();
                break;
            case Action.RETURN:
                _Return();
                break;
            case Action.BUILD:
                if(_NAVA.enabled)
                    _BuildMove();
                break;
            case Action.DIE:
                build_ready_Cancel();
                _Die();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 매프레임 반복 채굴
    /// </summary>
    void _Gather()
    {
        PERPOSE.ISGATHER = true;
        bool Wait = false;

        if (!PERPOSE.GatherTARGET.Equals(null))
        {
            Wait = PERPOSE.GatherTARGET.GetComponent<mineral>().includeFULL(gameObject);
        }
                
        if (PERPOSE.GatherTARGET.Equals(null) || Wait)
        {
            if (!checkMineral())
            {
                motionWait();

                if (PERPOSE.MnR.Count == 0)
                {
                    SetState(Action.CANCEL);
                }                
            }
        }
        else if (!PERPOSE.GatherTARGET.Equals(null) && !Wait)
        {
            Vector3 lookrotation = PERPOSE.GatherTARGET.transform.position - transform.position;
            if (lookrotation != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookrotation), 20f * Time.deltaTime);

            ANI.SetBool("ATTACK", true);
            ANI.SetBool("MOVE", false);
        }
    }

    void _Return()
    {
        GameObject _target = PERPOSE.HOMEPOS;
        
        if (_NAVA.enabled && Vector3.Distance(transform.position, _target.transform.position) > UNITSTATUS.RANGE + _target.GetComponent<CapsuleCollider>().radius)
        {
            _NAVA.destination = _target.transform.position;
        }
        else
        {
            PERPOSE.HOMEPOS.GetComponent<Structure>().SaveMine(PERPOSE.GETMINERAL);
            PERPOSE.GETMINERAL = 0;

            PERPOSE.refresh_mnr();
            if (PERPOSE.MnR.Count == 0)
            {
                SetState(Action.CANCEL);
            }
            else
            {
                _nextAction = Action.CANCEL;
                SetState(Action.TRACE);
            }
        }

        ANI.SetBool("ATTACK", false);
        ANI.SetBool("MOVE", true);
    }

    /// <summary>
    /// 여러 빌드 취소
    /// </summary>
    /// <returns></returns>
    public bool build_ready_Cancel()
    {
        if (StandBy == StandByAction.BUILD)
        {
            BUILDRD = false;
            _posSelect(false);
            blueprint = null;

            PERPOSE.BUILDPOS = Vector3.zero;

            StandBy = StandByAction.NOTHING;
            MngPack.instance.UIM.ScreenInterface();

            SetState(Action.CANCEL);

            if (!KingdomManager.isSelct.isEmpty)
            {
                Destroy(BUILD_BP);
                KingdomManager.Mine += Yetpaid;
                Yetpaid = 0;
                return true;
            }
        }
        return false;
    }

    // 여기서 빌드
    public bool Set_Build(Vector3 pos)
    {
        _posSelect(false);
        PosSelect = false;

        Destroy(BUILD_BP);

        buildup(pos, BUILD_N);

        BUILD_BP = null;
        buildingRd = false;
        MngPack.instance.KDM.buildinit();
        build_n = "";
        blueprint = new List<GameObject>();
        if (KingdomManager.isSelct.isContain(gameObject))
        {
            MngPack.instance.UIM.isButtonSelect(UIManager.buttonType.normalstate);
        }

        return true;
    }

    public GameObject buildup(Vector3 pos, string stc, string stc_normal = "STRUCTBONE")
    {
        GameObject st_bp = Instantiate(DataMng.instance.PREFAB_G[stc_normal]);
        if (stc_normal.CompareTo("STRUCTBONE") == 0)
        {
            st_bp.GetComponent<BoneBuilding>().build(stc, Yetpaid);
            Yetpaid = 0;
        }
        st_bp.transform.position = MngPack.instance.KDM.setPos(pos);
        st_bp.GetComponent<Structure>().manual_pos();
        return st_bp;
    }

    /// <summary>
    /// 워커용 idle = _posSelect때문에 만듬
    /// </summary>
    new public void _Idle()
    {
        PERPOSE.TARGETs.Clear();
        _posSelect(false);
        ANI.SetBool("MOVE", false);
        ANI.SetBool("ATTACK", false);
    }

    /// <summary>
    /// 건물 지을 자리가 이미 정해졌는지 안정해졌는지 bool값으로 저장
    /// </summary>
    /// <param name="bl"></param>
    public void _posSelect(bool bl)
    {
        PosSelect = bl;
    }

    /// <summary>
    /// 건물 지으러 이동
    /// </summary>
    protected void _BuildMove()
    {
        PERPOSE.STOP_RANGE = _NAVA.stoppingDistance;
        _NAVA.destination = PERPOSE.BUILDPOS;
                
        _posSelect(true);

        moveCheck();

        ANI.SetBool("MOVE", true);
        ANI.SetBool("ATTACK", false);
    }

    /// <summary>
    /// 채굴
    /// </summary>
    public void Havior()
    {
        if (state == Action.ATTACK)
        {
            Attack();
        }
        else if (state == Action.GATHER && !PERPOSE.GatherTARGET.Equals(null))
        {
            if (PERPOSE.GETMINERAL < 6)
            {
                PERPOSE.GETMINERAL += PERPOSE.GatherTARGET.GetComponent<mineral>().gather(gameObject);
                if (PERPOSE.GatherTARGET.GetComponent<mineral>().isEmpty())
                {
                    checkMineral();
                }
            }
            else if (PERPOSE.GETMINERAL >= 6)
            {
                PERPOSE.GatherTARGET.GetComponent<mineral>().gatherOut(gameObject);
                _nextAction = Action.RETURN;
            }
        }
    }

    /// <summary>
    /// 캘 미네랄이 있는지 확인
    /// </summary>
    /// <returns></returns>
    bool checkMineral()
    {
        PERPOSE.refresh_mnr();

        for (int i = 0; i < PERPOSE.MnR.Count; i++)
        {
            if (!PERPOSE.MnR[i].FULL)
            {
                PERPOSE.GatherTARGET = PERPOSE.MnR[i].gameObject;
                SetState(Action.TRACE);
                return true;
            }
        }

        return false;
    }

    void motionWait()
    {
        ANI.SetBool("MOVE", false);
        ANI.SetBool("ATTACK", false);
    }
}
