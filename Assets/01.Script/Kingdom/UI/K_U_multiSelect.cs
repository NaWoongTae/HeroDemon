using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class K_U_multiSelect : MonoBehaviour
{
    [SerializeField] Image[] img;

    // Use this for initialization
    void Start ()
    {
    }

    public void _Setting()
    {
        for (int i = 0; i < img.Length; i++)
        {
            bool open = (i < KingdomManager.isSelct.SelectObj.Count) ? true : false;
            img[i].gameObject.SetActive(open);

            if (open)
            {
                img[i].sprite = PrefabPool.instance.Kingdom_sprite[KingdomManager.isSelct.SelectObj[i].GetComponent<UNIT>().UNITSTATUS.NAME];
                float hp = KingdomManager.isSelct.SelectObj[i].GetComponent<Filter>().GetHp();
                img[i].color = new Color(1 - hp, hp, 0);
            }
        }
    }

    public void _select(int i)
    {
        Debug.Log(i);

        GameObject go = KingdomManager.isSelct.SelectObj[i];
        MngPack.instance.KDM.nowSelect(go, true);
    }
}
