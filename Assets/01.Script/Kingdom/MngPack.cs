using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MngPack : TSingleton<MngPack>
{
    KingdomManager _KDM;
    CtrlManager _CTM;
    UIManager _UIM;
    MouseCursor _MCS;
    MapObject _MOB;

    public debugtest _DT;

    public KingdomManager KDM
    {
        get { return _KDM; }
        set { _KDM = value; }
    }

    public CtrlManager CTM
    {
        get { return _CTM; }
        set { _CTM = value; }
    }

    public UIManager UIM
    {
        get { return _UIM; }
        set { _UIM = value; }
    }

    public MouseCursor MCS
    {
        get { return _MCS; }
        set { _MCS = value; }
    }

    public MapObject MOB
    {
        get { return _MOB; }
        set { _MOB = value; }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // TSingleton Init
    protected override void Init()
    {
        base.Init();
    }
}
