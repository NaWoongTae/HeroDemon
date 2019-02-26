using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectObject
{
    public List<GameObject> SelectObj;

    public int popular;
    public int MaxPopular;

    public bool[] upgrade_enable;
    public bool[] skill;
    public int str_count;
    public int def_count;
    public bool rangeUp;

    public Filter.UnitType SelectType
    {
        get
        {
            if (SelectObj.Count > 0)
                return SelectObj[0].GetComponent<Filter>().TYPE;
            else
                return Filter.UnitType.nothing;
        }
    }

    public int Count
    {
        get { return SelectObj.Count; }
    }

    public bool IsSolo
    {
        get { return SelectObj.Count == 1; }
    }

    public SelectObject()
    {
        SelectObj = new List<GameObject>();
        popular = 3;
        MaxPopular = 100;
        upgrade_enable = new bool[9] { false, false, false, false, false, false, false, false, false };
        skill = new bool[3] { false, false, false };
        str_count = 0;
        def_count = 0;
        rangeUp = false;
    }

    public void SignView(bool view)
    {
        for (int i = 0; i < SelectObj.Count; i++)
            SelectObj[i].GetComponent<Filter>()._selectSign(view);
    }

    public void SelectOff()
    {
        SignView(false);
        for (int i = 0; i < Count; i++)
            SelectObj[i].GetComponent<Filter>().WhenOff();
        SelectObj.Clear();        
    }

    public void Change(GameObject go)
    {
        SelectObj[0] = go;
    }

    public bool isEmpty
    {
        get
        {
            if (SelectObj.Count > 0)
                return false;

            return true;
        }
    }

    public bool Building
    {
        get
        {
            if (!isEmpty)
            {
                if (SelectObj[0].GetComponent<Filter>().containScript<Worker>())
                    return SelectObj[0].GetComponent<Worker>().BUILDRD;
                else
                    return false;
            }
            else
                return false;
        }
    }

    public bool isContain(GameObject go)
    {
        for (int i = 0; i < Count; i++)
        {
            if (SelectObj[i] == go)
                return true;
        }
        return false;
    }

    public bool findDelete(GameObject go)
    {
        if (isContain(go))
        {
            SelectObj.Remove(go);
            return true;
        }

        return false;
    }

    public bool selectMine()
    {
        if (SelectObj.Count > 0 && SelectObj[0].tag == "DUMMY")
        {
            return false;
        }

        return true;
    }
}
