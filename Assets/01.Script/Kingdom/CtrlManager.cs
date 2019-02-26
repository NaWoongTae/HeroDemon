using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;


public class CtrlManager : MonoBehaviour
{
    [SerializeField] GameObject selectbox2D;
    [SerializeField] GameObject moveSign;

    KingdomManager KDM;
    UIManager UIM;
    
    bool isOrder;

    public bool IsORDER
    {
        get { return isOrder; }
        set { isOrder = value; }
    }

    Vector3 _targetPos;

    public Vector3 TARGETPOS
    {
        get { return _targetPos; }
        set { _targetPos = value; }
    }

    Vector3 _inPos3D;    
    Vector3 _outPos3D;

    Vector3 _inPos2D;
    Vector3 _outPos2D;

    // Use this for initialization
    public void Init(KingdomManager kdm)
    {
        isOrder = false;
        MngPack.instance.CTM = this;
        KDM = kdm;
        UIM = GetComponent<UIManager>();

        StartCoroutine(mousePoint());
    }

    IEnumerator mousePoint()
    {
        while (true)
        {
            yield return null;

            if (UIM.MENUWIN)
            {
                continue;
            }

            if (isOrder)
            {
                OrderCheck();                
            }
            else
            {
                ScreenCheck();
            }
        }
    }

    /// <summary>
    /// 명령키 안누르고
    /// </summary>
    void ScreenCheck()
    {
        if (EventSystem.current.IsPointerOverGameObject() == false)
        {
            RaycastHit hit = GetHit();
            if (Input.GetButtonDown("Select")) // 좌클릭 누름 - 선택
            {
                // RaycastHit hit = GetHit();
                _inPos3D = hit.point;
                _inPos2D = Input.mousePosition;

                if (hit.collider.tag != "LAND")
                {
                    nowSelect(hit.collider.gameObject, true); // 새로 선택(단일)
                    UIM.ScreenInterface();
                }
            }
            else if (Input.GetButton("Select") && KingdomManager.isSelct.selectMine()) // 좌클릭 드래그
            {
                selectbox2D.SetActive(true);

                _outPos2D = Input.mousePosition;
                _outPos3D = GetHit().point;

                if (selectbox2D.activeSelf)
                {
                    selectbox2D.GetComponent<RectTransform>().anchoredPosition = (_inPos2D + _outPos2D) / 2;
                    selectbox2D.GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.Abs(_inPos2D.x - _outPos2D.x), Mathf.Abs(_inPos2D.y - _outPos2D.y));
                }
            }

            if (Input.GetButtonDown("Move") && KingdomManager.isSelct.selectMine()) // 우클릭 누름
            {
                // RaycastHit hit = GetHit();

                _targetPos = hit.point;

                if (hit.collider.tag == "LAND")
                {
                    makeMoveSign(_targetPos);

                    for (int i = 0; i < KingdomManager.isSelct.SelectObj.Count; i++)
                    {
                        KingdomManager.isSelct.SelectObj[i].GetComponent<Filter>().Adjustment(_targetPos);
                    }
                }
                else if (hit.collider.tag == "MINERAL")
                {
                    hit.collider.gameObject.GetComponent<Filter>().flick();
                    for (int i = 0; i < KingdomManager.isSelct.SelectObj.Count; i++)
                    {
                        KingdomManager.isSelct.SelectObj[i].GetComponent<Filter>().GetOrder(UNIT.Action.GATHER, hit);
                    }
                }
                else
                {
                    hit.collider.gameObject.GetComponent<Filter>().flick();
                    for (int i = 0; i < KingdomManager.isSelct.SelectObj.Count; i++)
                    {
                        KingdomManager.isSelct.SelectObj[i].GetComponent<Filter>().Adjustment(hit.collider.gameObject);
                    }
                }
            }
        }
        else
        {
            MngPack.instance.MCS.setMode(MouseCursor.Mode.IDLE);
        }

        if (Input.GetButtonUp("Select")) // 좌클릭 뗌
        {
            if (selectbox2D.activeSelf)
            {
                selectbox2D.SetActive(false);

                GameObject checkBox = new GameObject();
                checkBox.transform.parent = this.transform;

                checkBox.transform.position = (_outPos3D + _inPos3D) / 2;
                checkBox.transform.localScale = new Vector3(Mathf.Abs(_outPos3D.x - _inPos3D.x), 10, Mathf.Abs(_outPos3D.z - _inPos3D.z));

                checkBox.AddComponent<DragCheckBox>();
                BoxCollider checkBoxCol = checkBox.AddComponent<BoxCollider>();                
                checkBoxCol.isTrigger = true;

                Destroy(checkBox, 0.05f);
            }
            _inPos3D = _outPos3D = Vector3.zero;
            _inPos2D = _outPos2D = Vector3.zero;            
        }
    }

    /// <summary>
    /// 명령이 있을때 선택되는 오브젝트
    /// </summary>
    void OrderCheck()
    {
        if (EventSystem.current.IsPointerOverGameObject() == false)
        {
            if (Input.GetButtonDown("Select")) // 좌클릭 누름 - 선택
            {
                RaycastHit hit = GetHit();

                if (hit.collider.tag == "LAND")
                {
                    GameObject go = Instantiate(moveSign);
                    go.transform.position = hit.point + new Vector3(0, 0.2f, 0);
                }
                else
                    hit.collider.gameObject.GetComponent<Filter>().flick();

                for (int i = 0; i < KingdomManager.isSelct.SelectObj.Count; i++)
                {
                    KingdomManager.isSelct.SelectObj[i].GetComponent<Filter>().middleOrder(hit);
                }
                GetComponent<UIManager>().ScreenInterface();
            }
            else if (Input.GetButtonDown("Move")) // 우클릭 누름 // 취소
            {
                //RaycastHit hit = GetHit();

                for (int i = 0; i < KingdomManager.isSelct.SelectObj.Count; i++)
                {
                    KingdomManager.isSelct.SelectObj[i].GetComponent<Filter>().resetReady();

                }
                UIM.isButtonSelect(UIManager.buttonType.normalview);
                GetComponent<UIManager>().ScreenInterface();
            }
            if (Input.GetButtonUp("Select") || Input.GetButtonUp("Move"))
            {
                isOrder = false;
            }
        }
        else
        {
            MngPack.instance.MCS.setMode(MouseCursor.Mode.IDLE);
        }
    }

    public void nowSelect(GameObject other, bool fst)
    {
        KDM.nowSelect(other, fst);
        UIM.SetNormal();
    }

    /// <summary>
    /// 명령 목적 선택을 위한 isOrder의 상태변경 
    /// </summary>
    public void itsOrder()
    {
        isOrder = true;
    }

    void NewSelect(GameObject gameobject)
    {
        if (KingdomManager.isSelct.SelectObj.Count > 0)
        {
            for (int i = 0; i < KingdomManager.isSelct.SelectObj.Count; i++)
                KingdomManager.isSelct.SelectObj[i].GetComponent<UNIT>().ImSelect(false);

            KingdomManager.isSelct.SelectObj.Clear();
        }
        AddSelectObj(gameobject);
    }

    RaycastHit GetHit()
    {
        Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Physics.Raycast(r, out hit, 50, 1 << 9 | 1 << 11 | 1 << 12);

        if (hit.collider.tag == "KINGDOM")
        {
            MngPack.instance.MCS.setMode(MouseCursor.Mode.CK_MY);
        }
        else if (hit.collider.tag == "DUMMY" || hit.collider.tag == "ENEMY")
            MngPack.instance.MCS.setMode(MouseCursor.Mode.CK_EN);
        else
            MngPack.instance.MCS.setMode(MouseCursor.Mode.IDLE);

        return hit;
    }

    // 
    void AddSelectObj(GameObject go)
    {
        go.GetComponent<UNIT>().ImSelect(true);
        KingdomManager.isSelct.SelectObj.Add(go);        
    }

    public void makeMoveSign(Vector3 vec)
    {
        GameObject go = Instantiate(moveSign);
        go.transform.position = vec + new Vector3(0, 0.2f, 0);
    }
}
