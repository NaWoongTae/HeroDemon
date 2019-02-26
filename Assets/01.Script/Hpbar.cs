using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hpbar : MonoBehaviour
{    
    Image _hpBarProgress;
    // GameObject nowSelect;

    void Awake()
    {
        // nowSelect = null;
        _hpBarProgress = GetComponent<Image>();
    }

    public bool SettingHpBar(GameObject go)
    {
        // nowSelect = go;
        _hpBarProgress.fillAmount = go.GetComponent<Filter>().GetHp();
        if (_hpBarProgress.fillAmount <= 0)
            return false;

        return true;            
    }
}
