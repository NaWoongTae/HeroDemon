using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMapMaker : MonoBehaviour {

    Vector3 Home;
    [SerializeField] Transform E_Home;
    [SerializeField] Transform Mineral;
    [SerializeField] int mineralCount;
    Vector3 unit;

    /// <summary>
    /// 게임 실행시 기본 건물/유닛 배치
    /// </summary>
    public void init()
    {
        StartCoroutine(mapMaking());
    }

    IEnumerator mapMaking()
    {
        yield return new WaitUntil(() => KingdomManager.setting == true);

        GameObject go;

        Dictionary<string, Dictionary<string, string>> data = DataMng.instance.Get(LowDataType.BUILD).NODE;

        //==================================================================================================== 미네랄

        for (int i = 0; i < mineralCount; i++)
        {
            go = Instantiate(DataMng.instance.PREFAB_G["MINERAL"]);
            Vector3 thisTimePos = Mineral.position + new Vector3(i / 2 * 2, 0, -(i + 1) / 2 * 2);
            go.GetComponent<mineral>().manual_pos(thisTimePos);
            go.transform.position = thisTimePos;
        }

        Home = new Vector3(Mineral.position.x + mineralCount + 1, Mineral.position.y, Mineral.position.z + 2);

        //==================================================================================================== 홈 커맨드

        go = Instantiate(DataMng.instance.PREFAB_G["KINGCASTLE"]);

        StructStatus STRUCTSTATUS = new StructStatus("KINGCASTLE", false,
            int.Parse(data["KINGCASTLE"][BUILD.Index.SIZE.ToString()]),
            float.Parse(data["KINGCASTLE"][BUILD.Index.HP.ToString()]),
            data["KINGCASTLE"][BUILD.Index.PRODUCE.ToString()],
            data["KINGCASTLE"][BUILD.Index.CONNECT.ToString()],
            int.Parse(data["KINGCASTLE"][BUILD.Index.VALUE.ToString()]),
            data["KINGCASTLE"][BUILD.Index.PRECEDE.ToString()],
            int.Parse(data["KINGCASTLE"][BUILD.Index.DELAY.ToString()]),
            int.Parse(data["KINGCASTLE"][BUILD.Index.RALLY.ToString()])
            );
        STRUCTSTATUS.COMPLETE = true;

        go.GetComponent<Structure>().Init(STRUCTSTATUS, Vector3.zero);
        go.transform.position = Home;
        go.GetComponent<Structure>().manual_pos();

        unit = Home + new Vector3(1.5f, 0, -2.5f);

        //==================================================================================================== 적 커맨드

        go = Instantiate(DataMng.instance.PREFAB_G["DEVIL_CASTLE"]);

        StructStatus D_STRUCTSTATUS = new StructStatus("DEVIL_CASTLE", false, 4, 10000, "NULL", "NULL", 0, "NULL", 0, 0);
        D_STRUCTSTATUS.COMPLETE = true;

        go.GetComponent<Structure>().Init(D_STRUCTSTATUS, Vector3.zero);
        go.transform.position = E_Home.position;
        go.GetComponent<Structure>().manual_pos();

        //================================================================== 일꾼

        for (int i = 0; i < 3; i++)
        {
            go = Instantiate(DataMng.instance.PREFAB_G["WORKER"]);
            go.GetComponent<UNIT>()._Init("WORKER", true);
            go.transform.position = unit + new Vector3(-1 * i, 0, 0);
        }

        //==================================================================

    }
}
