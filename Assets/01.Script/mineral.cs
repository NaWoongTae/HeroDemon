using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mineral : usableObject
{
    [SerializeField] int head = 2;
    public Heap<GameObject> gathered;

    int[][] node;
    int amount;
    

    public int AMOUNT
    {
        get { return amount; }
        set { amount = value; }
    }

    public bool FULL
    {
        get { return gathered._Count >= head; }
    }

    // 내가 이미 포함되어있거나 자리가 남으면 false
    public bool includeFULL(GameObject go)
    {
        if (gathered.Contains(go))
        {
            return false;
        }
        else
            return gathered._Count >= head;
    }

    void Awake()
    {
        amount = Random.Range(0, 50) + 50;
        gathered = new Heap<GameObject>();
        node = new int[4][];
        init();
    }

    void Start()
    {
        MngPack.instance.MOB.Insert(transform, GetComponent<Filter>().TYPE);
    }

    private void Update()
    {
        _selectSign();

        if (gathered._Count > 2)
        {
            //gathered[2].GetComponent<Worker>().SetWait();
            gatherOut(gathered[2]);
        }
    }

    public int gather(GameObject go)
    {
        if (!gathered.Contains(go))
            gathered.Add(go);
        amount -= 1;

        return 1;
    }

    public void gatherOut(GameObject go)
    {
        gathered.RemoveAt(go);
    }

    public bool isEmpty()
    {
        if (amount <= 0)
        {
            for (int i = 0; i < 4; i++)
            {
                KingdomManager._Grid.GRID[node[i][0], node[i][1]].buildable = true;
            }
            if(KingdomManager.isSelct.SelectObj[0] == gameObject) {
                MngPack.instance.KDM.SelectRemove(gameObject);
            }
            Destroy(gameObject);
            return true;
        }

        return false;
    }

    /// <summary>
    /// 건설불가체크
    /// </summary>
    /// <param name="Opos"></param>
    public void manual_pos(Vector3 Opos)
    {
        for (int i = -1; i < 1; i++)
        {
            for (int j = -1; j < 1; j++)
            {
                node[2 * i + j + 3] = new int[] { (int)(Opos.x + i + 0.5f), (int)(Opos.z + j + 0.5f) };
                KingdomManager._Grid.GRID[(int)(Opos.x + i + 0.5f), (int)(Opos.z + j + 0.5f)].buildable = false;
            }
        }
    }
}
