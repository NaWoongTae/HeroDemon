using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class UNIT_UI : LowBase
{
    public string mainKey = "INDEX";

    public override void Load(string strJson)
    {
        // 이 node는 이미 한번 변환된 데이터
        JSONNode node = JSONNode.Parse(strJson);

        // column의 개수만큼
        for (int i = 0; i < 9; i++)
        {            
            if (string.Compare(mainKey, i.ToString()) != 0) // subkey가 index가 아닐때
            {
                // 시트별로 분할하기 때문에 node[x]는 항상 x == 0
                for (int j = 0; j < node[0].AsArray.Count; j++) // node[0].AsArray.Count 줄의 개수?
                    Add(node[0][j][mainKey],                    // 키
                        i.ToString(),                      // 서브 키
                        node[0][j][i.ToString()].Value);   // 값
            }
        }
    }
}

