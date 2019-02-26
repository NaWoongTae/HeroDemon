using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class K_U_UnitInfo : MonoBehaviour
{
    [SerializeField] GameObject _att;
    [SerializeField] Text att_t;
    [SerializeField] GameObject _def;
    [SerializeField] Text def_t;
    [SerializeField] GameObject leftTxt;
    [SerializeField] Text left_mineral;

    public void GetData(GameObject go)
    {
        _att.SetActive(false);
        _def.SetActive(false);
        leftTxt.SetActive(false);

        switch (go.GetComponent<Filter>().TYPE)
        {
            case Filter.UnitType.KS:
                SetData();
                break;
            case Filter.UnitType.KUC:
            case Filter.UnitType.KUH:
                SetData(go);
                break;
            case Filter.UnitType.MNR:
                SetResource(go);
                break;
        }
    }

    // 유닛
    void SetData(GameObject go)
    {
        if (go.tag == "DUMMY")
        {
            return;
        }

        _att.SetActive((go.GetComponent<Filter>().GetUnitStatus().STR(false) != 0) ? true : false);
        _def.SetActive(true);

        att_t.text = KingdomManager.isSelct.str_count.ToString();
        def_t.text = KingdomManager.isSelct.def_count.ToString();

        if (go.GetComponent<Filter>().containScript<Worker>())
            StartCoroutine(mine(go));
    }

    // 건물
    void SetData()
    {
    }

    // 자원
    void SetResource(GameObject go)
    {
        leftTxt.SetActive(true);
        left_mineral.text = go.GetComponent<mineral>().AMOUNT.ToString();
    }

    IEnumerator mine(GameObject go)
    {
        while (go.Equals(KingdomManager.isSelct.SelectObj[0]))
        {
            int num = go.GetComponent<Worker>().PERPOSE.GETMINERAL;
            if (num > 0)
            {
                leftTxt.SetActive(true);
                left_mineral.text = num.ToString();
            }
            else
            {
                leftTxt.SetActive(false);
            }
            yield return new WaitForFixedUpdate();
        }
    }
}
