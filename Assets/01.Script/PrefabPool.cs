using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Managers
{
    BaseManager,
    LogoManager,
    LobbyManager,
    BattleManager,
    Max
}

public class PrefabPool : TSingleton<PrefabPool> {

    // 매니저
    Dictionary<Managers, GameObject> MngPrefabDic = new Dictionary<Managers, GameObject>();
    public Dictionary<Managers, GameObject> MngDic = new Dictionary<Managers, GameObject>();

    // 스프라이트
    Dictionary<string, Sprite> sprites;

    public Dictionary<string, Sprite> Kingdom_sprite
    {
        get { return sprites; }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    protected override void Init()
    {
        base.Init();        

        for (int i = (int)Managers.LogoManager; i < (int)Managers.Max; i++)
        {
            GameObject prefab = Resources.Load("Managers\\" + ((Managers)i).ToString()) as GameObject;
            MngPrefabDic.Add((Managers)i, prefab);
        }

        sprites = new Dictionary<string, Sprite>();

        var enumerator = DataMng.instance.Get(LowDataType.IMAGE).NODE.GetEnumerator();
        while (enumerator.MoveNext())
        {
            string str = enumerator.Current.Value[IMAGE.Index.IMAGE.ToString()];
            Sprite _sprite = Resources.Load<Sprite>("Sprite/" + str);
            if (_sprite == null)
                continue;

            sprites.Add(str, _sprite);
        }
    }

    public GameObject SetManager(Managers mng)
    {
        GameObject go = Instantiate(MngPrefabDic[mng]);

        if (MngDic.ContainsKey(mng))
        {
            MngDic[mng] = go;
        }
        else
        {
            MngDic.Add(mng, go);
        }

        return MngDic[mng];
    }

    public GameObject GetManager(Managers mng)
    {
        if (MngDic.ContainsKey(mng))
            return MngDic[mng];

        return SetManager(mng);
    }

    public void RemoveManager(Managers mng)
    {
        if (MngDic.ContainsKey(mng))
            MngDic.Remove(mng);
    }
}
