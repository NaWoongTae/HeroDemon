using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderButton : MonoBehaviour
{
    Image img;

    Sprite btn;
    Sprite block;

    bool open;

    public string order
    {
        get { return img.sprite.name; }
    }

    public bool isComplete
    {
        get
        {
            if (KingdomManager.isSelct.SelectType == Filter.UnitType.KS)
                return KingdomManager.isSelct.SelectObj[0].GetComponent<Filter>().GetStructStatus().COMPLETE;
            else
                return false;
        }
    }
    UIManager UIM;
    bool isSelect;

    Dictionary<string, Dictionary<string, string>> data;

    public void Init()
    {
        isSelect = false;
        img = GetComponent<Image>();
        UIM = GetComponentInParent<UIManager>();

        data = DataMng.instance.Get(LowDataType.BUILD).NODE;

        open = false;
        string str = DataMng.instance.Get(LowDataType.IMAGE).NODE["BLOCK"]["IMAGE"];
        block = PrefabPool.instance.Kingdom_sprite[str];
        btn = block;
        img.sprite = block;

        StartCoroutine(Co_Update());
    }

    IEnumerator Co_Update()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();

            if (btn.name.Equals("RETURN") && KingdomManager.isSelct.SelectObj[0].GetComponent<Filter>().containScript<UNIT>())
            {
                open = (KingdomManager.isSelct.SelectObj[0].GetComponent<UNIT>().PERPOSE.HAVEMINERAL) ? true : false;
                img.sprite = (open) ? btn : block;
            }

            img.color = (order.CompareTo("HOLD") == 0 && isSelect) ? Color.yellow : Color.white;
        }
    }

    // 이미지 설정
    public void inputImg(string _sprite)
    {
        if (_sprite.Equals("OFF"))
        {
            setOff();
            return;
        }

        btn = PrefabPool.instance.Kingdom_sprite[_sprite];
        img.sprite = btn;

        // 미네랄 반납 버튼 디폴트는 off지만 미네랄을 가지고 있으면 바로 보여줘야 해서
        if (_sprite.Equals("RETURN") && !KingdomManager.isSelct.SelectObj[0].GetComponent<UNIT>().PERPOSE.HAVEMINERAL)
        {
            setOff(false);
            return;
        }
    }

    // 이미지 블록 설정
    public void setOff(bool reset = true)
    {
        if (reset)
            btn = block;

        img.sprite = block;
    }

    public void ButtonOrder()
    {
        if (img.sprite.name != "BLOCK")
        {
            if (KingdomManager.isSelct.SelectType == Filter.UnitType.KUC ||
                KingdomManager.isSelct.SelectType == Filter.UnitType.KUH)
                SelectType_Unit();
            else if (KingdomManager.isSelct.SelectType == Filter.UnitType.KS)
                SelectType_Struct();

            MngPack.instance.UIM.img_info(false);
        }
    }

    void SelectType_Unit()
    {
        if (order.Equals("CANCEL"))
            UIM.isButtonSelect(UIManager.buttonType.normalview);
        else if (order.Equals("STOP"))
            UIM.isButtonSelect(UIManager.buttonType.normalstate);
        else if (UIM.BUTTONTYPE == UIManager.buttonType.normalstate)
        {
            if (order.Equals("BUILD"))
            {
                UIM.isButtonSelect(UIManager.buttonType.build);
            }
            else
            {
                for (int i = 0; i < KingdomManager.isSelct.SelectObj.Count; i++)
                {
                    KingdomManager.isSelct.SelectObj[i].GetComponent<Filter>().witchImage(img.sprite.name);
                }
                if (!order.Equals("HOLD"))
                {
                    isSelect = true;
                    UIM.isButtonSelect(UIManager.buttonType.cancel);
                }
            }
        }
        else if (UIM.BUTTONTYPE == UIManager.buttonType.cancel)
        {
            UIM.isButtonSelect(UIManager.buttonType.normalstate);
        }
        else if (UIM.BUTTONTYPE == UIManager.buttonType.build)
        {
            if (MngPack.instance.UIM.use_mine(int.Parse(data[img.sprite.name][BUILD.Index.PRICE.ToString()])))
            {
                MngPack.instance.KDM.Build_Ready(img.sprite.name);
                UIM.isButtonSelect(UIManager.buttonType.cancel);
            }
        }
    }

    void SelectType_Struct()
    {
        if (order.Equals("CANCEL"))
        {
            if (!isComplete)
            {
                KingdomManager.isSelct.SelectObj[0].GetComponent<Structure>().structDestroy();
                return;
            }
            else if (KingdomManager.isSelct.SelectObj[0].GetComponent<Structure>().PRODUCE._Count > 0)
                KingdomManager.isSelct.SelectObj[0].GetComponent<Structure>().RemoveFirst();
        }
        else if (UIM.BUTTONTYPE == UIManager.buttonType.normalstate)
        {
            KingdomManager.isSelct.SelectObj[0].GetComponent<Structure>().Product(img.sprite.name);
            UIM.isButtonSelect(UIManager.buttonType.produce);
        }
        else if (UIM.BUTTONTYPE == UIManager.buttonType.produce)
        {
            KingdomManager.isSelct.SelectObj[0].GetComponent<Structure>().Product(img.sprite.name);
        }

        if (KingdomManager.isSelct.SelectObj[0].GetComponent<Structure>().PRODUCE._Count == 0)
        {
            UIM.isButtonSelect(UIManager.buttonType.normalstate);
        }
    }

    public void GetAct()
    {
        isSelect = false;
    }

    /// <summary>
    /// 버튼에 마우스가 올라갔을때
    /// 버튼이 올라가면 이미지타입과 bool값 반환
    /// </summary>
    public void Enterbutton()
    {
        if (img.sprite.name != "BLOCK")
            MngPack.instance.UIM.img_info(true, img.sprite.name);
    }

    /// <summary>
    /// 마우스가 버튼에서 빠져나갈때 false
    /// </summary>
    public void Exitbutton()
    {
        MngPack.instance.UIM.img_info(false);
    }

    public void buttonDown()
    {
        if (img.sprite.name != "BLOCK")
            img.color = Color.yellow;
    }
}
