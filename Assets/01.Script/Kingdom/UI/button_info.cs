using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class button_info : MonoBehaviour {

    [SerializeField] GameObject body;
    [SerializeField] Text info;
    [SerializeField] Image cost;
    [SerializeField] Image pop;
    [SerializeField] Image mana;

    private void Start()
    {
        body.SetActive(false);
    }

    private void Update()
    {
        if (body.activeSelf)
            GetComponent<RectTransform>().anchoredPosition = new Vector2(Input.mousePosition.x - 1680f, Input.mousePosition.y - 220f);
    }

    /// <summary>
    /// 버튼설명 보여주기
    /// </summary>
    /// <param name="b">버튼 이미지 타입</param>
    /// <param name="str">이름</param>
    public void show(bool b, string str = "")
    {
        if (body.activeSelf != b)
        {
            body.SetActive(b);
            if (b)
            {
                info.text = DataMng.instance.Get(LowDataType.IMAGE).NODE[str][IMAGE.Index.TRANS.ToString()];
                trans_info(DataMng.instance.Get(LowDataType.IMAGE).NODE[str][IMAGE.Index.TYPE.ToString()], str);
            }
        }
    }

    /// <summary>
    /// 버튼에 마우스 올렸을때의 설명창 크기 배치 만들기
    /// </summary>
    /// <param name="type">버튼 이미지 타입</param>
    /// <param name="str">이름</param>
    void trans_info(string type, string str)
    {
        Dictionary<string, Dictionary<string, string>> data;
        body.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 100);
        info.GetComponent<RectTransform>().anchoredPosition = new Vector3(0,21,0);
        
        switch (type)
        {            
            case "UNIT":
                data = DataMng.instance.Get(LowDataType.UNITDATA).NODE;
                cost.gameObject.SetActive(true);
                pop.gameObject.SetActive(true);
                mana.gameObject.SetActive(false);
                cost.GetComponentInChildren<Text>().text = data[((int)EnumHelper.StringToEnum<UNITDATA.type>(str)).ToString()][UNITDATA.Index.COST.ToString()];
                pop.GetComponentInChildren<Text>().text = data[((int)EnumHelper.StringToEnum<UNITDATA.type>(str)).ToString()][UNITDATA.Index.POPULATION.ToString()];
                break;
            case "STRUCT":
                data = DataMng.instance.Get(LowDataType.BUILD).NODE;
                cost.gameObject.SetActive(true);
                pop.gameObject.SetActive(false);
                mana.gameObject.SetActive(false);
                cost.GetComponentInChildren<Text>().text = data[str][BUILD.Index.PRICE.ToString()];
                break;
            case "UPGRADE":
                cost.gameObject.SetActive(true);
                pop.gameObject.SetActive(false);
                mana.gameObject.SetActive(false);
                string key = ((int)EnumHelper.StringToEnum<UPGRADE.Name>(str)).ToString();
                cost.GetComponentInChildren<Text>().text = KingdomManager.usingData[key][UPGRADE.Index.COST.ToString()];
                break;
            case "SKILL":
                cost.gameObject.SetActive(false);
                pop.gameObject.SetActive(false);
                mana.gameObject.SetActive(true);
                mana.GetComponentInChildren<Text>().text = 20.ToString();//data[str][UNITDATA.Index.COST.ToString()];
                break;
            case "CT":
                body.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 50);
                info.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
                cost.gameObject.SetActive(false);
                pop.gameObject.SetActive(false);
                mana.gameObject.SetActive(false);
                break;
        }
    }
}
