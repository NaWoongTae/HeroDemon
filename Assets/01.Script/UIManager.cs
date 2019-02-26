using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public enum buttonType
    {
        normalview,
        normalstate,
        cancel,
        build,
        produce
    }

    [SerializeField] OrderButton[] _Button;
    [SerializeField] Text mine;
    [SerializeField] Text popular;
    [SerializeField] GameObject img;
    [SerializeField] GameObject MenuWin;

    buttonType _buttonT;
    Info_UI _infoUI;

    public bool MENUWIN
    {
        get { return MenuWin.activeSelf; }
    }

    public buttonType BUTTONTYPE
    {
        get { return _buttonT; }
        set { _buttonT = value; }
    }

    public Info_UI INFOUI
    {
        get { return _infoUI; }
    }

    private void Awake()
    {
        _buttonT = buttonType.normalstate;
    }

    IEnumerator InterfaceCheck()
    {
        while (true)
        {
            _infoUI.UiRefresh();
            mine.text = KingdomManager.Mine.ToString();
            yield return null;
        }
    }

    // Use this for initialization
    public void Init(KingdomManager kdm)
    {
        for (int i = 0; i < 9; i++)
            _Button[i].Init();

        popular.text = KingdomManager.popular_part + "/" + KingdomManager.popular_all;
        MngPack.instance.UIM = this;

        _infoUI = GetComponentInChildren<Info_UI>();

        StartCoroutine(SelectKey());
        StartCoroutine(InterfaceCheck());
    }

    /// <summary>
    /// 단축키 조작
    /// </summary>
    IEnumerator SelectKey()
    {
        Dictionary<string, Dictionary<string, string>> data = DataMng.instance.Get(LowDataType.IMAGE).NODE;
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.F5) && MENUWIN == false)
            {
                openMenuWin();
            }
            else if (Input.anyKeyDown)
            {
                string str = (Input.GetKeyDown(KeyCode.Escape))? "ESC" : Input.inputString.ToUpper();

                if (!str.Equals(""))
                {
                    for (int i = 0; i < 9; i++)
                    {
                        if (_Button[i].order.Equals("BLOCK"))
                        {
                        }
                        else if (data[_Button[i].order]["KEY"].Equals(str))
                        {
                            _Button[i].ButtonOrder();
                            break;
                        }                        
                    }
                }
            }
            yield return null;
        }
        yield return null;
    }

    public void UI_Update(Heap<string> str)
    {
        popular.text = KingdomManager.popular_part + "/" + KingdomManager.popular_all;
        _infoUI.UI_Update(str);
    }

    /// <summary>
    /// (버튼)명령창
    /// </summary>
    public void ScreenInterface()
    {
        _infoUI.WhenSelect();

        List<GameObject> gameObj = new List<GameObject>();
        gameObj = KingdomManager.isSelct.SelectObj;

        bool being = (KingdomManager.isSelct.SelectObj.Count == 0) ? true : false;
        bool _Same = (KingdomManager.isSelct.SelectObj.Count == 1) ? true : false;

        for (int i = 1; i < gameObj.Count; i++)
        {
            if (gameObj[0].GetComponent<UNIT>().UNITSTATUS.NAME == gameObj[i].GetComponent<UNIT>().UNITSTATUS.NAME)
                _Same = true;
            else
                _Same = false;
        }

        if (!being)
        {            
            if (_Same) // 같거나 1개
            {
                if (KingdomManager.isSelct.SelectType == Filter.UnitType.KS && !KingdomManager.isSelct.SelectObj[0].GetComponent<Filter>().GetStructStatus().COMPLETE) // 건물이고 짓는중
                {
                    settingButton("STRUCTBONE");
                }
                else if (KingdomManager.isSelct.SelectType == Filter.UnitType.KUC && KingdomManager.isSelct.SelectObj[0].GetComponent<UNIT>().StandBy == UNIT.StandByAction.BUILD) // 유닛이고 건물짓는중
                {
                    settingButton("SELECT");
                }
                else if (KingdomManager.isSelct.SelectType == Filter.UnitType.MNR) // 미네랄
                {
                    for (int i = 0; i < _Button.Length; i++)
                    {
                        _Button[i].setOff();
                    }
                }
                else if (KingdomManager.isSelct.SelectObj[0].tag == "DUMMY") // 적이면
                {
                    for (int i = 0; i < _Button.Length; i++)
                    {
                        _Button[i].setOff();
                    }
                }
                else
                {
                    settingButton(gameObj[0].GetComponent<Filter>().GetName());
                }

                if (KingdomManager.isSelct.SelectType == Filter.UnitType.KS && KingdomManager.isSelct.SelectObj[0].GetComponent<Structure>().PRODUCE._Count > 0)
                {
                    _Button[8].inputImg("CANCEL");
                }
            }
            else // 여러개의 중복되는 버튼만 표시
            {
                for (int i = 0; i < _Button.Length; i++)
                {
                    for (int j = 0; j < KingdomManager.isSelct.SelectObj.Count; j++)
                    {
                        if (DataMng.instance.Get(LowDataType.UNITUI).NODE[gameObj[j].GetComponent<Filter>().GetName()][i.ToString()] == "ATTACK") // 어택은 있으면 무조건 넣어줌
                        {
                            _Button[i].inputImg("ATTACK");
                            break;
                        }
                        else if (DataMng.instance.Get(LowDataType.UNITUI).NODE[gameObj[0].GetComponent<Filter>().GetName()][i.ToString()] ==
                        DataMng.instance.Get(LowDataType.UNITUI).NODE[gameObj[j].GetComponent<Filter>().GetName()][i.ToString()]) // 모두가 있는버튼
                        {
                            _Button[i].inputImg(DataMng.instance.Get(LowDataType.UNITUI).NODE[gameObj[0].GetComponent<Filter>().GetName()][i.ToString()]);
                        }
                        else // 나머지는 다 끔
                        {
                            _Button[i].setOff();
                            break;
                        }
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < _Button.Length; i++)
            {
                _Button[i].setOff();
            }            
        }
        
        closedButton();

        // 건물이고 & 지금상태가 생산일때
        if (KingdomManager.isSelct.SelectType == Filter.UnitType.KS && KingdomManager.isSelct.SelectObj[0].GetComponent<Structure>().NOWSTATE == buttonType.produce)
        {
            isButtonSelect(KingdomManager.isSelct.SelectObj[0].GetComponent<Structure>().NOWSTATE);
        }
    }
    
    // 상태에 따라 버튼 모양 변경
    public void isButtonSelect(buttonType bt)
    {
        switch (bt)
        {
            case buttonType.normalview:
                ScreenInterface();
                bt = buttonType.normalstate;
                if (KingdomManager.isSelct.IsSolo)
                    if (KingdomManager.isSelct.SelectObj[0].GetComponent<Filter>().containScript<Worker>())
                        KingdomManager.isSelct.SelectObj[0].GetComponent<Worker>().build_ready_Cancel();
                break;
            case buttonType.normalstate:
                ScreenInterface();
                if (KingdomManager.isSelct.IsSolo)
                {
                    if (KingdomManager.isSelct.SelectObj[0].GetComponent<Filter>().containScript<Worker>())
                    {
                        KingdomManager.isSelct.SelectObj[0].GetComponent<Worker>().build_ready_Cancel();
                    }
                    KingdomManager.isSelct.SelectObj[0].GetComponent<Filter>().GetOrder(UNIT.Action.CANCEL);
                }
                else
                {
                    for (int i = 0; i < KingdomManager.isSelct.SelectObj.Count; i++)
                    {
                        KingdomManager.isSelct.SelectObj[i].GetComponent<Filter>().GetOrder(UNIT.Action.CANCEL);
                    }
                }
                break;
            case buttonType.cancel:
                settingButton("SELECT");

                GetComponent<CtrlManager>().itsOrder();
                closedButton();
                break;
            case buttonType.build:
                settingButton("BUILD");

                closedButton();
                break;
            case buttonType.produce:
                _infoUI.build_Multi();
                if (KingdomManager.isSelct.SelectObj[0].GetComponent<Structure>().PRODUCE._Count > 0)
                    _Button[8].inputImg("CANCEL");
                else
                    _Button[8].setOff();
                closedButton();
                KingdomManager.isSelct.SelectObj[0].GetComponent<Structure>().NOWSTATE = bt;
                break;
        }
        _buttonT = bt;
    }

    public void SetNormal()
    {
        BUTTONTYPE = buttonType.normalstate;
    }

    public void buildstruct()
    {
        _infoUI.build_Single();
    }

    // 버튼이 block이면 버튼 닫기
    void closedButton()
    {
        for (int i = 0; i < _Button.Length; i++)
        {
            if (_Button[i].order.Equals("BLOCK"))
            {
                if (KingdomManager.isSelct.SelectType == Filter.UnitType.KUC)
                    _Button[i].GetAct();
            }
            else
            {
                if (KingdomManager.isSelct.SelectType == Filter.UnitType.KUC)
                {
                    _Button[i].GetAct();
                }
            }
        }
    }

    void settingButton(string str)
    {
        if (str.Equals("LABORATORY"))
        {
            for (int i = 0; i < _Button.Length; i++)
            {
                if (KingdomManager.isSelct.upgrade_enable[i])
                    _Button[i].setOff();
                else
                {
                    string s1 = DataMng.instance.Get(LowDataType.UNITUI).NODE[str][i.ToString()];
                    if (!s1.Equals("OFF"))
                        _Button[i].inputImg(DataMng.instance.Get(LowDataType.UNITUI).NODE[str][i.ToString()]);
                    else
                        _Button[i].setOff();
                }
            }
        }
        else
        {
            for (int i = 0; i < _Button.Length; i++)
            {
                string s1 = DataMng.instance.Get(LowDataType.UNITUI).NODE[str][i.ToString()];
                if (!s1.Equals("OFF"))
                    _Button[i].inputImg(DataMng.instance.Get(LowDataType.UNITUI).NODE[str][i.ToString()]);
                else
                    _Button[i].setOff();
            }
        }
    }

    // 인구수 실질적 업데이트
    public void update_popul()
    {
        popular.text = KingdomManager.popular_part.ToString() + "/" + KingdomManager.popular_all;
    }

    public bool use_mine(int m)
    {
        if (KingdomManager.Mine - m < 0)
        {
            StartCoroutine(MngPack.instance.KDM.MsgDebug("자원이 부족합니다."));
            return false;
        }
        else
        {
            KingdomManager.Mine -= m;
            KingdomManager.isSelct.SelectObj[0].GetComponent<Filter>().YetSave(m);
            return true;
        }
        
        // mine.text = KingdomManager.Mine.ToString();
    }

    /// <summary>
    /// button_info로 전달
    /// </summary>
    public void img_info(bool b, string str = "")
    {
        img.GetComponent<button_info>().show(b, str);
    }

    // 인게임 메뉴창 열기버튼
    public void openMenuWin()
    {
        MenuWin.SetActive(true);
        MenuWin.GetComponent<IngameOption>().SetOpen();
    }
}
