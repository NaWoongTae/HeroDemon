using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 나의 시트명 Enum
public enum LowDataType
{
	BUILD,
    UNITDATA,
    UPGRADE,
    IMAGE,
    UNITUI,
    RESOURCE,
    MAX
}

public enum Structype
{
    supplybase,
    barrack,
    quarters,
    laboratory,
    turret,
    MAX
}

public class DataMng : TSingleton< DataMng > 
{
    // LowDataType과 그 시트의 LowBase(내용?)
	Dictionary< LowDataType, LowBase > dataList = new Dictionary<LowDataType, LowBase>();
    Dictionary<string, GameObject> gameobjPrefab = new Dictionary<string, GameObject>();
    List<Sprite> unit_ui;    

    public Dictionary<string, GameObject> PREFAB_G
    {
        get { return gameobjPrefab; }
    }

    // TSingleton Init
    protected override void Init()
	{        
        base.Init ();
        unit_ui = new List<Sprite>();
    }
	
    // 
	public LowBase Load<T>( LowDataType type ) where T: LowBase, new()
	{
        // 이미 데이터가 존재하고 있다면, 함수를 종료한다.
        if (dataList.ContainsKey (type))
		{
			LowBase lowBase = dataList[ type ]; // 있는내용 넣어준다
			return dataList[ type ]; // 있는거 반환
		}
		
		TextAsset textAsset = Resources.Load ("Data/" + type.ToString ()) as TextAsset; // textAsset
        if ( textAsset != null )
		{
			T t = new T ();
			t.Load (textAsset.text); // textAsset.text은 이미 한번 변환된 데이터
            dataList.Add( type, t ); // 처리후 추가
		}
		
		return dataList [type];
	}

    // 로드
	public void LoadData()
	{
        Load<BUILD>(LowDataType.BUILD);
        Load<UNITDATA> (LowDataType.UNITDATA);
        Load<UPGRADE>(LowDataType.UPGRADE);
        Load<IMAGE>(LowDataType.IMAGE);
        Load<UNIT_UI>(LowDataType.UNITUI);
        Load<RESOURCE>(LowDataType.RESOURCE);

        GetResource();
    }
	
    // 있으면 가져오기
	public LowBase Get( LowDataType dataType ) 
	{
		if( dataList.ContainsKey ( dataType ) )
			return dataList[ dataType ];
		
		return null;
	}

    // clear
    public void Clear ()
	{
		dataList.Clear ();
	}

    public void GetResource()
    {
        for (int i = 0; i < Get(LowDataType.RESOURCE).NODE.Count; i++)
        {
            GameObject prefab = Resources.Load("Prefabs/" + Get(LowDataType.RESOURCE).NODE[i.ToString()][RESOURCE.Index.NAME.ToString()]) as GameObject;
            if (Get(LowDataType.RESOURCE).NODE[i.ToString()][RESOURCE.Index.TYPE.ToString()].CompareTo("GameObject") == 0)
                gameobjPrefab.Add(Get(LowDataType.RESOURCE).NODE[i.ToString()][RESOURCE.Index.NAME.ToString()], prefab);
        }
    }

}



