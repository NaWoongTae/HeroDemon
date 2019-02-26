using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class K_U_Unit_Img : MonoBehaviour
{
    [SerializeField] Text _hp;
    [SerializeField] Text _mp;
    [SerializeField] Text Name;
    Image selectObject;

    void Awake()
    {
        selectObject = GetComponent<Image>();
    }

    public void GetData()
    {
        if (KingdomManager.isSelct.Count < 1)
        {
            _hp.enabled = false;
            _mp.enabled = false;
            Name.enabled = false;
            selectObject.sprite = null;
            return;
        }

        GameObject go = KingdomManager.isSelct.SelectObj[0];

        switch (go.GetComponent<Filter>().TYPE)
        {
            case Filter.UnitType.KS:
                SetData(go.GetComponent<Filter>().GetStructStatus());
                break;
            case Filter.UnitType.KUC:
            case Filter.UnitType.KUH:
                SetData(go.GetComponent<Filter>().GetUnitStatus());
                break;
            case Filter.UnitType.MNR:
                SetData();
                break;
            default:
                break;
        }
    }

    // 유닛
    void SetData(UnitStatus us)
    {
        selectObject.sprite = PrefabPool.instance.Kingdom_sprite[us.NAME];
        selectObject.color = new Color(1 - us.HP / us.MAXHP, us.HP / us.MAXHP, 0);
        _hp.text = us.HP.ToString() + " / " + us.MAXHP.ToString();
        _mp.text = (us.MP == 0) ? "" : us.MP.ToString() + " / " + us.MAXMP.ToString();
        Name.text = us.NAME;
    }

    // 건물
    void SetData(StructStatus ss)
    {
        selectObject.sprite = PrefabPool.instance.Kingdom_sprite[ss.NAME];
        if (KingdomManager.isSelct.selectMine())
        {
            selectObject.color = new Color(1 - ss.HP / ss.MAXHP, ss.HP / ss.MAXHP, 0);
        }
        else
        {
            selectObject.color = Color.white;
        }
        _hp.text = ((int)ss.HP).ToString() + " / " + ((int)ss.MAXHP).ToString();
        _mp.text = "";
        Name.text = ss.NAME;
    }

    // 자원
    void SetData()
    {
        selectObject.sprite = PrefabPool.instance.Kingdom_sprite["MINERAL"];// 미네랄 그림
        selectObject.color = new Color(0.8f,0.8f,1);
        _hp.text = "";
        _mp.text = "";
        Name.text = "MINERAL";
    }
}
