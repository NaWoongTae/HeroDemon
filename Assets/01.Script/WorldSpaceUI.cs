using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSpaceUI : MonoBehaviour
{
    KingdomManager KDM;
    Camera _mainCamera;    
    Hpbar[] _hpbar;

    // 현재 UI를 target의 UI로 설정함
    public void Init(KingdomManager kdm)
    {
        KDM = kdm;
        _mainCamera = Camera.main;
        _hpbar = GetComponentsInChildren<Hpbar>();
        for (int i = 0; i < _hpbar.Length; i++)
        {
            _hpbar[i].gameObject.SetActive(false);
        }
    }

    // hp바 정면보게
    void LateUpdate()
    {
        for (int i = 0; i < KingdomManager.isSelct.SelectObj.Count; i++)
        {
            _hpbar[i].transform.rotation = Quaternion.LookRotation(_mainCamera.transform.forward);
            _hpbar[i].transform.position = KingdomManager.isSelct.SelectObj[i].GetComponent<Filter>().GethpPos();
            if (!_hpbar[i].SettingHpBar(KingdomManager.isSelct.SelectObj[i]))
            {
                KDM.SelectRemove(KingdomManager.isSelct.SelectObj[i]);
            }
        }   
    }

    // hp바 껏다켯다
    public void SettingHP()
    {
        for (int i = 0; i < _hpbar.Length; i++)
        {
            if (i < KingdomManager.isSelct.SelectObj.Count)
            {
                _hpbar[i].gameObject.SetActive(true);
            }
            else
            {
                _hpbar[i].gameObject.SetActive(false);
            }
        }
    }
}
