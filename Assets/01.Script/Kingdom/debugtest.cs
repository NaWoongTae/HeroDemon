using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class debugtest : MonoBehaviour
{
    [SerializeField] Text test;

	// Use this for initialization
	void Start ()
    {
        MngPack.instance._DT = this;
        test.text = "-";
    }

    private void Update()
    {
        
    }

    public void testDebug(string str)
    {
        ////test.text = str;
        //if (KingdomManager.isSelct.Count > 0 && KingdomManager.isSelct.SelectObj[0].GetComponent<Filter>().containScript<UNIT>() && KingdomManager.isSelct.SelectObj[0].GetComponent<UNIT>().PERPOSE.TARGETs._Count > 0)
        //{
        //    test.text = KingdomManager.isSelct.SelectObj[0].GetComponent<UNIT>().state.ToString();
        //}
        //else
        //{
        //    test.text = "-";
        //}
    }
}
