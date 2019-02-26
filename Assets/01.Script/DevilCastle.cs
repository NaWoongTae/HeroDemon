using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevilCastle : Structure
{
    float instTime = 45;
    float instGap = 0.8f;
    int instCount = 3;
    Vector3[] vector3s;

    float t = 20f;
    bool sally = false;

    // Use this for initialization
    void Start()
    {
        vector3s = new Vector3[3];
        RaycastHit hit;
        Physics.Raycast(transform.position + new Vector3(-3, 0, 0), Vector3.down, out hit, 10f);
        vector3s[0] = hit.point;
        Physics.Raycast(transform.position + new Vector3(-3, 0, -3), Vector3.down, out hit, 10f);
        vector3s[1] = hit.point;
        Physics.Raycast(transform.position + new Vector3(0, 0, -3), Vector3.down, out hit, 10f);
        vector3s[2] = hit.point;

        Init_Start();
    }

    void Update()
    {
        if (!sally)
        {
            t += Time.deltaTime;

            if (t > instTime)
            {
                StartCoroutine(sallyCorps());
                t = 0f;
                sally = true;
            }
        }

        _selectSign();
    }

    IEnumerator sallyCorps()
    {
        GameObject go;
        for (int i = 0; i < instCount; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                go = Instantiate(DataMng.instance.PREFAB_G["E_WARRIOR"], vector3s[j], transform.rotation);
                go.GetComponent<UNIT>()._Init("WARRIOR", false);

                if (j == 0)
                {
                    go.GetComponent<Filter>().goAttack(new Vector3(8, 0, 119));
                }
                else if (j == 2)
                {
                    go.GetComponent<Filter>().goAttack(new Vector3(119, 0, 8));
                }
                else
                {
                    go.GetComponent<Filter>().goAttack(KingdomManager.Home[0].transform.position);
                }
            }
            yield return new WaitForSeconds(instGap);
        }

        for (int i = 0; i < instCount; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                go = Instantiate(DataMng.instance.PREFAB_G["E_ARCHER"], vector3s[j], transform.rotation);
                go.GetComponent<UNIT>()._Init("ARCHER", false);

                if (j == 0)
                {
                    go.GetComponent<Filter>().goAttack(new Vector3(8, 0, 119));
                }
                else if (j == 2)
                {
                    go.GetComponent<Filter>().goAttack(new Vector3(119, 0, 8));
                }
                else
                {
                    go.GetComponent<Filter>().goAttack(KingdomManager.Home[0].transform.position);
                }
            }

            yield return new WaitForSeconds(instGap);
        }

        sally = false;
    }
}
