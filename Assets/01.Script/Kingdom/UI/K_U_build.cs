using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class K_U_build : MonoBehaviour
{
    [SerializeField] Image[] multi;
    [SerializeField] Image single;
    [SerializeField] Image loading;

    bool isSingle;

    public bool IsSINGLE
    {
        get { return isSingle; }
        set { isSingle = value; }
    }

    // Use this for initialization
    void Start()
    {
        loading.fillAmount = 0;
        isSingle = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (KingdomManager.isSelct.SelectObj[0].GetComponent<Structure>().PRODUCE._Count > 0)
        {
            Loadbar(KingdomManager.isSelct.SelectObj[0].GetComponent<Structure>().PdTime, 
                EnumHelper.StringToEnum<LowDataType>(KingdomManager.isSelct.SelectObj[0].GetComponent<Structure>().STRUCTSTATUS.PRODUCE));
        }
        else if (!KingdomManager.isSelct.SelectObj[0].GetComponent<Structure>().STRUCTSTATUS.COMPLETE)
        {
            Loadbar(KingdomManager.isSelct.SelectObj[0].GetComponent<Structure>().PdTime);
        }
    }

    /// <summary>
    ///  멀티 생산창 on 싱글 off
    /// </summary>
    public void product_m()
    {
        updateImg(KingdomManager.isSelct.SelectObj[0].GetComponent<Structure>().PRODUCE);

        for (int i = 0; i < multi.Length; i++)
            multi[i].gameObject.SetActive(true);

        single.gameObject.SetActive(false);
    }

    /// <summary>
    /// 멀티 생산창 off 싱글 on
    /// </summary>
    public void product_s()
    {
        for (int i = 0; i < multi.Length; i++)
            multi[i].gameObject.SetActive(false);
        single.gameObject.SetActive(true);
    }

    /// <summary>
    /// 생산 로드바
    /// </summary>
    /// <param name="num"></param>
    /// <param name="complete"> 지금 생산하는 건물이 완성됬나 </param>
    void Loadbar(float num, LowDataType lowData)
    {
        float delay = 0, nowPdct = 0;
        // 유닛생산 및 연구        
        switch (lowData)
        {
            case LowDataType.UNITDATA:
                nowPdct = (int)EnumHelper.StringToEnum<UNITDATA.type>(KingdomManager.isSelct.SelectObj[0].GetComponent<Structure>().PRODUCE.First());
                delay = float.Parse(DataMng.instance.Get(LowDataType.UNITDATA).NODE[nowPdct.ToString()]["DELAY"]);

                break;
            case LowDataType.UPGRADE:
                nowPdct = (int)EnumHelper.StringToEnum<UPGRADE.Name>(KingdomManager.isSelct.SelectObj[0].GetComponent<Structure>().PRODUCE.First());
                delay = float.Parse(KingdomManager.usingData[nowPdct.ToString()]["DELAY"]);
                break;
        }
    
        loading.fillAmount = num / delay;
    }

    /// <summary>
    /// 건물 건설 로드바
    /// </summary>
    /// <param name="num"></param>
    void Loadbar(float num)
    {
        float delay = 0;
         // 건물
        
        delay = float.Parse(DataMng.instance.Get(LowDataType.BUILD).NODE[KingdomManager.isSelct.SelectObj[0].GetComponent<Filter>().GetName()]["DELAY"]);
        
        loading.fillAmount = num / delay;
    }

    public void updateImg(Heap<string> str)
    {
        if(str._Count < 1)
        {
            gameObject.SetActive(false);
        }

        for (int i = 0; i < multi.Length; i++)
        {
            if (i < str._Count)
            {
                multi[i].sprite = PrefabPool.instance.Kingdom_sprite[str.GetT(i)];
                multi[i].color = Color.white;
            }
            else
            {
                multi[i].sprite = null;
                multi[i].color = Color.black;
            }            
        }
    }

    public void updateImg(string str)
    {
        single.sprite = PrefabPool.instance.Kingdom_sprite[str];
    }

    /// <summary>
    /// 생성되는 이미지를 눌러서 취소시키기
    /// </summary>
    /// <param name="i"></param>
    public void _remove(int i)
    {
        if (multi[i].sprite != null)
        {
            string str = KingdomManager.isSelct.SelectObj[0].GetComponent<Structure>().PRODUCE.GetT(i);

            KingdomManager.isSelct.SelectObj[0].GetComponent<Structure>().PRODUCE.Remove(i);
            updateImg(KingdomManager.isSelct.SelectObj[0].GetComponent<Structure>().PRODUCE);

            if (i == 0)
                KingdomManager.isSelct.SelectObj[0].GetComponent<Structure>().PdTime = 0f;

            int num = (int)EnumHelper.StringToEnum<UNITDATA.type>(str);
            KingdomManager.Mine += (int.Parse(DataMng.instance.Get(LowDataType.UNITDATA).NODE[num.ToString()]["COST"]));

            if (KingdomManager.isSelct.SelectObj[0].GetComponent<Structure>().PRODUCE._Count == 0)
                MngPack.instance.UIM.INFOUI.WhenSelect();
        }
    }
}
