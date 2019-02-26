using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerSetting : MonoBehaviour
{
    protected bool settingSuccess = false;

    public bool SUCCESS()
    {
        return settingSuccess; 
    }

    public virtual void settingManager()
    {
        settingSuccess = false;
    }
}
