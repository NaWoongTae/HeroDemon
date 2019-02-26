using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Info_UI : MonoBehaviour
{
    public enum ScreenType
    {
        blank,
        select_s,
        select_e,
        select_m,
        build_s,
        build_m
    }

    [SerializeField] GameObject select;
    [SerializeField] GameObject unit_img;
    [SerializeField] GameObject unit_info;
    [SerializeField] GameObject build;

    K_U_Unit_Img _img;
    K_U_UnitInfo _info;
    K_U_build _build;
    K_U_multiSelect _select;

    ScreenType _screenType;

    // Use this for initialization
    void Start ()
    {
        _screenType = ScreenType.blank;
        _img = unit_img.GetComponent<K_U_Unit_Img>();
        _info = unit_info.GetComponent<K_U_UnitInfo>();
        _build = build.GetComponent<K_U_build>();
        _select = select.GetComponent<K_U_multiSelect>();

        SetScreenType(ScreenType.blank);
    }
    

    void SetScreenType(ScreenType st)
    {
        select.SetActive(false);
        unit_img.SetActive(false);
        unit_info.SetActive(false);
        build.SetActive(false);

        switch (st)
        {
            case ScreenType.select_s:
                unit_img.SetActive(true);
                unit_info.SetActive(true);
                break;
            case ScreenType.select_e:
                unit_img.SetActive(true);
                break;
            case ScreenType.select_m:
                select.SetActive(true);
                break;
            case ScreenType.build_s:
                unit_img.SetActive(true);
                build.SetActive(true);
                _build.product_s();
                break;
            case ScreenType.build_m:
                unit_img.SetActive(true);
                build.SetActive(true);
                _build.product_m();
                break;
        }
        _screenType = st;
    }

    /// <summary>
    /// 선택된 유닛/건물의 개수 상태에 따라 정보화면 설정
    /// </summary>
    public void WhenSelect()
    {
        if (KingdomManager.isSelct.SelectObj.Count == 1) // 단일선택
        {
            if (KingdomManager.isSelct.SelectType == Filter.UnitType.KS)
            {
                if (KingdomManager.isSelct.SelectObj[0].tag == "DUMMY")
                {
                    SetScreenType(ScreenType.select_e);
                    return;
                }
                else if (!KingdomManager.isSelct.SelectObj[0].GetComponent<Structure>().STRUCTSTATUS.COMPLETE)
                {
                    SetScreenType(ScreenType.build_s);
                    build_Single();
                    return;
                }
            }
            SetScreenType(ScreenType.select_s);
            Select_Single();            
        }
        else if (KingdomManager.isSelct.SelectObj.Count > 1) // 복수 선택
        {
            SetScreenType(ScreenType.select_m);
            Select_Multi();
        }
        else
            SetScreenType(ScreenType.blank);
    }

    public void UiRefresh()
    {
        if (_screenType == ScreenType.select_m)
        {
            _select._Setting();
        }
        else if(_screenType != ScreenType.blank)
        {
            _img.GetData();
            if (KingdomManager.isSelct.SelectType == Filter.UnitType.MNR)
            {
                _info.GetData(KingdomManager.isSelct.SelectObj[0]);
            }
        }
    }

    void Select_Single()
    {        
        _info.GetData(KingdomManager.isSelct.SelectObj[0]);        
    }

    void Select_Multi()
    {
        _select._Setting();
    }

    public void build_Single()
    {
        _build.updateImg(KingdomManager.isSelct.SelectObj[0].GetComponent<Filter>().GetName());
        SetScreenType(ScreenType.build_s);
    }

    public void build_Multi()
    {
        SetScreenType(ScreenType.build_m);
    }

    public void UI_Update(Heap<string> str)
    {
        _build.updateImg(str);
    }

    public void Screenblank()
    {
        SetScreenType(ScreenType.blank);
    }
}
