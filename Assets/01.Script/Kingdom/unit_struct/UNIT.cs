using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UNIT : usableObject
{
    
    public enum Action
    {
        NOTHING,
        MOVE,
        CANCEL,
        TRACE,
        ATTACK,
        ATTACKMOVE,
        HOLD,
        PATROL,
        GATHER,
        BUILD,
        RETURN,
        HEAL,
        DIE
    }

    public enum StandByAction
    {
        NOTHING,
        CANCEL,
        MOVE,
        ATTACK,
        HOLD,
        PATROL,
        GATHER,
        BUILD,
        RETURN,
        HEAL
    }

    [SerializeField] Transform hpbarPos;
    bool SetPatrol;
    bool SetBuild;
    bool findNextatt;
    bool arrive;

    Perpose_Info _Perpose;
    GameObject _unitSight;
    UnitStatus _unitStatus;

    public Perpose_Info PERPOSE
    {
        get { return _Perpose; }
        set { _Perpose = value; }
    }

    public UnitStatus UNITSTATUS
    {
        get { return _unitStatus; }
        set { _unitStatus = value; }
    }

    public GameObject UNITSIGHT
    {
        get
        {
            if (_unitSight == null)
                return _unitSight = GetComponentInChildren<UnitSight>().gameObject;
            else
                return _unitSight;
        }
        set { _unitSight = value; }
    }

    public Transform HPBARPOS
    {
        get { return hpbarPos; }
    }

    public bool FINDNEXTATT
    {
        get { return findNextatt; }
        set { findNextatt = value; }
    }

    public Action state;
    protected Action _nextAction;
    protected Action prevState;

    StandByAction _standby;     

    public bool Attackable
    {
        get { return ((state == Action.CANCEL) || (state == Action.HOLD)); }
    }

    public StandByAction StandBy
    {
        get { return _standby; }
        set { _standby = value; }
    }

    NavMeshAgent _navA;
    NavMeshObstacle _navM;
    NavMeshPath path;
    Animator _animator;    

    protected NavMeshObstacle _NAVM
    {
        get
        {
            if (_navM == null)
                return _navM = GetComponent<NavMeshObstacle>();
            else
                return _navM;
        }
    }

    protected NavMeshAgent _NAVA
    {
        get
        {
            if (_navA == null)
                return _navA = GetComponent<NavMeshAgent>();
            else
                return _navA;
        }
    }

    protected NavMeshPath _NAVP
    {
        get { return path; }
    }

    protected Animator ANI
    {
        get
        {
            if (_animator == null)
                return _animator = GetComponent<Animator>();
            else
                return _animator;
        }
    }

    protected void Awake_Init()
    {
        SetPatrol = false;
        SetBuild = false;
        findNextatt = false;

        _Perpose = new Perpose_Info();
        _unitStatus = new UnitStatus();
        path = new NavMeshPath();
                
        _nextAction = Action.CANCEL;
        prevState = Action.CANCEL;        

        init();
    }

    // ==============================================================================================================

    public void _Init(string name, bool itsMine)
    {        
        UNITSTATUS = GetUnitData(EnumHelper.StringToEnum<UNITDATA.type>(name));

        MngPack.instance.MOB.Insert(transform, GetComponent<Filter>().TYPE);

        if (itsMine)
        {
            KingdomManager.popular_part += UNITSTATUS.POPULATION;
        }
        MngPack.instance.UIM.update_popul();
        arrive = true;
        UNITSIGHT.GetComponent<SphereCollider>().radius = _unitStatus.RANGE;

        SetState(Action.CANCEL);
    }

    /// <summary>
    /// NaavMeshAgent가 대상을 바라보며 회전하는 속도가 한계가 있어 추가
    /// NaavMeshAgent가 움직이는 방향으로 빠르게 회전한다.
    /// arrive의 값이 먼저 정해져야 하기때문에 moveCheck()이후 동작되어야한다.
    /// </summary>
    protected void extraRotation()
    {
        if (_NAVA.enabled && arrive)
        {
            Vector3 lookrotation = _NAVA.steeringTarget - transform.position;
            if (lookrotation != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookrotation), 20f * Time.deltaTime);
        }

        goAble();
    }

    public void moveCheck()
    {        
        if (Vector3.Distance(transform.position, PERPOSE.BUILDorMOVE) <= PERPOSE.STOP_RANGE)
        {
            arrive = false;
            if (state == Action.PATROL)
            {
                if (Vector3.Distance(transform.position, PERPOSE.MoveGroundPos) <= 1f)
                {
                    PERPOSE.MOVESPOT = PERPOSE.PatrolStartPos;
                }
                else if (Vector3.Distance(transform.position, PERPOSE.PatrolStartPos) <= 1f)
                {
                    PERPOSE.MOVESPOT = PERPOSE.MoveGroundPos;
                }
            }
            else if (state == Action.BUILD)
            {
                if (GetComponent<Worker>().Set_Build(PERPOSE.BUILDPOS))
                {
                    SetState(Action.CANCEL);
                    StandBy = StandByAction.NOTHING;
                }
                MngPack.instance.UIM.ScreenInterface();
                _Perpose.BUILDPOS = Vector3.zero;
                SetBuild = false;
            }
            else
            {
                SetState(Action.CANCEL);
            }
        }
        else
        {
            arrive = true;
        }
    }

    //==============================================================================================================

    // state를 바꾸면서 상태 초기화
    public bool SetState(Action type)
    {        
        if (type == state)
            return true;

        //====================================== 셋팅 초기화 부분
        // 적체크 X
        UNITSIGHT.SetActive(false);
        // 네비게이션 X
        if (_NAVM.enabled)
            _NAVM.carving = false;
        _NAVM.enabled = false;
        _NAVA.enabled = false;
        if (!PERPOSE.ISGATHER && UNITSTATUS.NAME.Equals("WORKER"))
        {
            UNITSIGHT.GetComponent<UnitSight>().inGather();
        }
        //================================================
        
        switch (type)
        {
            case Action.CANCEL:
                if (arrive)
                    _Idle_allStop();
                else
                    _Idle();
                // 적체크 O
                UNITSIGHT.SetActive(true);
                UNITSIGHT.GetComponent<SphereCollider>().radius = (type == Action.HOLD) ? _unitStatus.RANGE : _unitStatus.NOTICE;

                // 추적 O
                StartCoroutine(EnableUnitMovementCoroutine(type, true));
                break;
            case Action.ATTACK:
            case Action.HEAL:
            case Action.HOLD:
                // 적체크 O
                UNITSIGHT.SetActive(true);
                UNITSIGHT.GetComponent<SphereCollider>().radius = (type == Action.HOLD) ? _unitStatus.RANGE : _unitStatus.NOTICE;
                StartCoroutine(EnableUnitMovementCoroutine(type, false));
                break;
            case Action.MOVE:
            case Action.BUILD:
                PERPOSE.STOP_RANGE = _NAVA.stoppingDistance;
                StartCoroutine(EnableUnitMovementCoroutine(type, true));
                state = type;
                break;
            case Action.GATHER:
                UNITSIGHT.SetActive(true);
                UNITSIGHT.GetComponentInChildren<SphereCollider>().radius = 10;
                StartCoroutine(EnableUnitMovementCoroutine(type,true));
                state = type;
                break;
            case Action.RETURN:
                UNITSIGHT.SetActive(true);
                PERPOSE.ISGATHER = true;
                prevState = state;
                StartCoroutine(EnableUnitMovementCoroutine(type, true));
                break;
            case Action.TRACE:
                UNITSIGHT.SetActive(true);
                if (state != Action.ATTACK)
                {
                    prevState = state;
                }
                //if (PERPOSE.ISGATHER)
                //{
                //    UNITSIGHT.SetActive(true);
                //}
                StartCoroutine(EnableUnitMovementCoroutine(type,true));
                break;

            case Action.ATTACKMOVE:
            case Action.PATROL:
                UNITSIGHT.SetActive(true);
                PERPOSE.STOP_RANGE = _NAVA.stoppingDistance;
                StartCoroutine(EnableUnitMovementCoroutine(type,true));
                break;

            case Action.DIE:
                _Die();
                state = type;
                break;
        }

        return true;
    }

    //==============================================================================================================

    // 정지
    public void _Idle()
    {
        PERPOSE.TARGETs.Clear();
        ANI.SetBool("MOVE", false);
        ANI.SetBool("ATTACK", false);
    }

    public void _Idle_allStop()
    {
        if (!KingdomManager.isSelct.isEmpty)
            if (GetComponent<Filter>().containScript<Worker>())
                GetComponent<Worker>().build_ready_Cancel();
        PERPOSE.AllClear();
        StandBy = StandByAction.NOTHING;

        if (UNITSTATUS.NAME.Equals("WORKER"))
        {
            UNITSIGHT.GetComponent<UnitSight>().inNormal(UNITSTATUS);
        }

        SetBuild = false;
        ANI.SetBool("MOVE", false);
        ANI.SetBool("ATTACK", false);
    }

    public void resetReady()
    {
        if (GetComponent<Filter>().containScript<Worker>())
            GetComponent<Worker>().build_ready_Cancel();

        StandBy = StandByAction.NOTHING;
        SetBuild = false;
    }

    // 이동 타겟 설정 - 1
    public void _MoveSet(Vector3 target)
    {
        PERPOSE.ISGATHER = false;
        _Perpose.MoveGroundPos = target;
        PERPOSE.MOVESPOT = _Perpose.MoveGroundPos; // 목표위치 수정/ 원래는 함수내 최하단에 존재했으나 위치를 잡고 state를 move로 해야하여 상단으로 옮김

        if (ANI.GetBool("ATTACK")) // 공격중 / 채굴중
        {
            if (SetPatrol)
                _nextAction = Action.PATROL;
            else
                _nextAction = Action.MOVE;
        }
        else
        {
            if (SetPatrol)
            {
                PERPOSE.PatrolStartPos = transform.position;
                SetState(Action.PATROL);
            }
            else if (SetBuild)
            {
                SetState(Action.BUILD);
                _Perpose.BUILDPOS = target;
                MngPack.instance.UIM.ScreenInterface();
            }
            else
            {
                PERPOSE.STOP_RANGE = _NAVA.stoppingDistance;
                SetState(Action.MOVE);
            }
        }        
    }

    // 이동 타겟 설정 - 2
    public void _MoveSet(GameObject target)
    {
        PERPOSE.ISGATHER = false;
        if (target.Equals(gameObject))
            return;

        if (SetPatrol)
        {
            _MoveSet(target.transform.position);
        }
        else
        {
            if (ANI.GetBool("ATTACK"))
                _nextAction = Action.MOVE;
            else
                SetState(Action.MOVE);

            PERPOSE.STOP_RANGE = _NAVA.stoppingDistance + target.GetComponent<CapsuleCollider>().radius;

            _Perpose.MoveTarget = target;
        }
    }

    // 이동
    protected void _Move()
    {
        if (_NAVA.enabled)
        {
            if (_Perpose.isGround) // 땅
            {
                _NAVA.destination = PERPOSE.MOVESPOT;
            }
            else if (_Perpose.isTarget) // 유닛
            {
                PERPOSE.MOVESPOT = (_Perpose.MoveTarget.transform.position);
            }

            _NAVA.destination = PERPOSE.MOVESPOT;


            moveCheck();

            
            ANI.SetBool("MOVE", true);
            ANI.SetBool("ATTACK", false);
        }
    }
    
    // 이동 ver.따라가기
    protected void Follow()
    {
        if (_Perpose.isTarget)
        {
            if (Vector3.Distance(_Perpose.MoveTarget.transform.position, transform.position) >= 3.5f)
            {
                _NAVA.enabled = true;
                _NAVA.destination = (_Perpose.MoveTarget.transform.position);
                ANI.SetBool("MOVE", true);
            }
        }
    }

    // -- 여기서 공격 시작
    // (충돌)시야내 체크시 발견된 적 받아옴 
    public void FindEnemy(GameObject Enemy)
    {
        _Perpose.FLASHTARGET = Enemy;

        switch (state)
        {
            case Action.NOTHING:
            case Action.CANCEL:
            case Action.PATROL:
            case Action.ATTACKMOVE:
                SetState(Action.TRACE);
                break;

            case Action.HOLD:
                SetState(Action.HOLD);
                break;
        }
    }

    // 추적
    protected void _Trace()
    {
        GameObject _target = (!PERPOSE.ISGATHER)? PERPOSE.whatTarget : (PERPOSE.GETMINERAL == 6)? PERPOSE.HOMEPOS : PERPOSE.GatherTARGET;

        if (_target == null)
        {
            PERPOSE.refresh_mnr();
            if (PERPOSE.ISGATHER && PERPOSE.MnR.Count > 0) // 채굴중 + 미네랄 남음
            {
                PERPOSE.GatherTARGET = PERPOSE.MnR[0].gameObject;
            }
            else if (!PERPOSE.ISGATHER) // 채굴중 아님
            {
                SetState(prevState);
            }
            else
            {
                SetState(Action.CANCEL);
            }

            return;
        }

        if (_NAVA.enabled && Vector3.Distance(transform.position, _target.transform.position) > _unitStatus.RANGE + _target.GetComponent<CapsuleCollider>().radius)
        {
            _NAVA.destination = _target.transform.position;
        }
        else if (PERPOSE.ISGATHER)
        {
            if (PERPOSE.GETMINERAL == 6)
            {
                PERPOSE.HOMEPOS.GetComponent<Structure>().SaveMine(PERPOSE.GETMINERAL);
                PERPOSE.GETMINERAL = 0;
            }
            else
            {
                _nextAction = Action.GATHER;
                SetState(Action.GATHER);
            }
        }
        else if (GetComponent<Filter>().containScript<Healer>())
        {
            _nextAction = Action.HEAL;
            SetState(Action.HEAL);
        }
        else
        {
            _nextAction = Action.ATTACK;
            SetState(Action.ATTACK);
        }

        ANI.SetBool("ATTACK", false);
        ANI.SetBool("MOVE", true);
    }

    // 공격
    protected void _Attack(bool goTrace)
    {
        if (PERPOSE.whatTarget == null && goTrace)
        {
            UNITSIGHT.GetComponent<UnitSight>().COMBAT = false;

            if (PERPOSE.TARGETs._Count > 0)
            {
                for (int i = 0; i < PERPOSE.TARGETs._Count; i++)
                {
                    if (PERPOSE.TARGETs[i] != null && PERPOSE.TARGETs[i].GetComponent<Filter>().GetHp() > 0)
                    {
                        FindEnemy(PERPOSE.TARGETs[i]);
                        break;
                    }
                    else
                    {
                        PERPOSE.TARGETs.RemoveAt(PERPOSE.TARGETs[i]);
                        i--;
                        continue;
                    }
                }
            }
            else
            {
                if (tag == "DUMMY")
                {
                    SetState(Action.ATTACKMOVE);
                }
                else if (prevState == Action.NOTHING)
                {
                    SetState(Action.CANCEL);
                }
                else
                {
                    SetState(prevState);
                }
                return;
            }
        }

        if (PERPOSE.whatTarget != null)
        {
            Vector3 lookpos = new Vector3(PERPOSE.whatTarget.transform.position.x, transform.position.y, PERPOSE.whatTarget.transform.position.z);
            transform.LookAt(lookpos);

            ANI.SetBool("ATTACK", true);
            ANI.SetBool("MOVE", false);

            if (Vector3.Distance(transform.position, PERPOSE.whatTarget.transform.position) > _unitStatus.RANGE + PERPOSE.whatTarget.GetComponent<CapsuleCollider>().radius && goTrace)
            {
                SetState(Action.TRACE);
            }
        }
        
    }

    // 홀드
    protected void _Hold()
    {
        if (PERPOSE.whatTarget == null || 
            Vector3.Distance(transform.position, PERPOSE.whatTarget.transform.position) > _unitStatus.RANGE + PERPOSE.whatTarget.GetComponent<CapsuleCollider>().radius)
        {
            ANI.SetBool("ATTACK", false);
        }
        else
            _Attack(false);
    }

    // 패트롤
    protected void _setPatrol()
    {
        SetPatrol = true;
    }

    protected void _setBuild()
    {
        SetBuild = true;
    }

    // 다이
    protected void _Die()
    {
        ANI.SetBool("DIE", true);
        GetComponent<CapsuleCollider>().enabled = false;
        if (gameObject.tag == "KINGDOM")
        {
            tag = "Untagged";
            KingdomManager.popular_part -= UNITSTATUS.POPULATION;
        }
        Circle.SetActive(false);
        Destroy(gameObject, 2.5f);
    }

    public void cancel()
    {
        _Idle();
    }

    //==============================================================================================================

    public void GetOrder(RaycastHit order)
    {
        PERPOSE.ISGATHER = false;
        switch (StandBy)
        {
            case StandByAction.MOVE:
                if (order.collider.tag == "KINGDOM")
                {
                    _MoveSet(order.collider.gameObject);                    
                }
                else
                {
                    _MoveSet(order.point);
                }
                break;
            case StandByAction.ATTACK:
                if (order.collider.tag == "KINGDOM" || order.collider.tag == "DUMMY")
                {
                    PERPOSE.ATTACKTARGET = order.collider.gameObject;
                    SetState(Action.TRACE);
                }
                else
                {
                    if (state != Action.ATTACK && state != Action.ATTACKMOVE)
                    {
                        PERPOSE.MoveGroundPos = order.point;
                        PERPOSE.MOVESPOT = PERPOSE.MoveGroundPos;
                        SetState(Action.ATTACKMOVE);
                    }
                }
                break;
            case StandByAction.PATROL:
                SetPatrol = true;
                _MoveSet(order.point);
                break;
            case StandByAction.GATHER:
                UNITSIGHT.GetComponent<UnitSight>().inGather();
                _gather(order);
                break;
            case StandByAction.HEAL:
                if (order.collider.tag == "UNIT")
                {
                    PERPOSE.ATTACKTARGET = order.collider.gameObject;
                    SetState(Action.TRACE);
                }
                break;
            case StandByAction.BUILD:
                SetBuild = true;
                _MoveSet(order.point);

                break;
            default:
                break;
        }
    }

    public void NextAction()
    {
        if (_nextAction == Action.MOVE)
        {
            SetState(Action.MOVE);
        }
        if (_nextAction == Action.RETURN)
        {
            SetState(Action.RETURN);
        }
    }

    //==============================================================================================================

    public void standbyOrder(StandByAction act)
    {
        if (act == StandByAction.CANCEL)
            SetState(Action.CANCEL);
        else if (act == StandByAction.HOLD)
            SetState(Action.HOLD);
        else if (act == StandByAction.RETURN)
            SetState(Action.RETURN);
        else
        {
            StandBy = act;
        }
    }

    public bool GetDamage(float Damage)
    {
        UNITSTATUS.HP -= (Damage - UNITSTATUS.DEF(tag == "KINGDOM"));
        if (UNITSTATUS.HP <= 0)
        {
            GetComponent<CapsuleCollider>().enabled = false;
            SetState(Action.DIE);
            if (KingdomManager.isSelct.isContain(gameObject))
            {
                MngPack.instance.KDM.SelectRemove(gameObject);
            }
            return true;
        }

        return false;            
    }

    public void GetHeal(float Heal)
    {
        UNITSTATUS.HP += Heal;

        if (UNITSTATUS.HP > UNITSTATUS.MAXHP)
        {
            UNITSTATUS.HP = UNITSTATUS.MAXHP;
        }
    }

    public void Attack()
    {
        if (!findNextatt)
        {
            if (_Perpose.whatTarget != null)
            {
                findNextatt = _Perpose.whatTarget.GetComponent<Filter>().GetDamage(UNITSTATUS.STR(gameObject.tag == "KINGDOM"));
            }
            else
            {
                findNextatt = true;
            }
        }
        else
        {
            PERPOSE.TARGETs.RemoveAt(_Perpose.whatTarget);

            if (PERPOSE.TARGETs._Count == 0)
            {
                SetState(prevState);
            }

            findNextatt = false;
        }
    }

    IEnumerator EnableUnitMovementCoroutine(Action type, bool navA)
    {
        if (_NAVM != null && _NAVM.enabled)
        {
            _NAVM.enabled = !navA;

            _NAVM.carving = true;
        }
        yield return new WaitForEndOfFrame();
        if (_NAVA != null && !_NAVA.enabled)
        {
            _NAVA.enabled = navA;
        }
        yield return new WaitForEndOfFrame();

        state = type;
    }

    void _gather(RaycastHit hit)
    {
        GameObject go = hit.collider.gameObject;
        if (go.tag != "LAND")
        {
            if (go.GetComponent<Filter>().TYPE == Filter.UnitType.MNR)
            {
                PERPOSE.GatherTARGET = go;
                PERPOSE.ISGATHER = true;
                UNITSIGHT.GetComponent<UnitSight>().inGather();
            }
            
            SetState(Action.TRACE);
        }
        else
        {
            StandBy = StandByAction.MOVE;
            _MoveSet(hit.point);
        }
    }

    // 데이터 -> 오브젝트 스테이터스에 삽입위한 정리
    protected UnitStatus GetUnitData(UNITDATA.type name)
    {
        Dictionary<string, Dictionary<string, string>> data = DataMng.instance.Get(LowDataType.UNITDATA).NODE;
        int num = (int)name;

        UnitStatus us = new UnitStatus(name.ToString(), data[num.ToString()]["TYPE"], int.Parse(data[num.ToString()]["COST"]),
            float.Parse(data[num.ToString()]["HP"]), int.Parse(data[num.ToString()]["STR"]), int.Parse(data[num.ToString()]["DEF"]),
            float.Parse(data[num.ToString()]["ATTSPEED"]), float.Parse(data[num.ToString()]["CRITICAL"]), int.Parse(data[num.ToString()]["MP"]),
            float.Parse(data[num.ToString()]["RANGE"]), float.Parse(data[num.ToString()]["NOTICE"]), float.Parse(data[num.ToString()]["SIGHT"]),
            int.Parse(data[num.ToString()]["SPEED"]), data[num.ToString()]["MANUFACTURER"], int.Parse(data[num.ToString()]["POPULATION"]));

        return us;
    }

    /// <summary>
    /// 빈 미네랄 받아오기
    /// </summary>
    /// <returns></returns>
    //protected mineral GetMnR()
    //{
    //    PERPOSE.refresh_mnr();

    //    for(int i = 0; i < PERPOSE.MnR.Count; i++)
    //    {
    //        if (!PERPOSE.MnR[i].FULL)
    //            return PERPOSE.MnR[i];
    //    }

    //    return null;
    //}

    /// <summary>
    /// 해당위치 접근가능?
    /// </summary>
    void goAble()
    {
        if (_NAVA.enabled)
        {
            _NAVA.CalculatePath(_NAVA.destination, _NAVP);

            if (_NAVP.status != NavMeshPathStatus.PathComplete)
            {
                SetState(Action.CANCEL);
                StartCoroutine(MngPack.instance.KDM.MsgDebug("이동 불가능한 장소입니다."));
            }
        }
    }
}