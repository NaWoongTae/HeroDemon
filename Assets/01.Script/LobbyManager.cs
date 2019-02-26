using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : ManagerSetting
{
    enum shade { flag, outline }
    [SerializeField] SkinnedMeshRenderer smr;
    [SerializeField] Material[] mr;

    BaseManager _BSM;
    float degree;

	// Use this for initialization
	public bool Lobby_Init ()
    {
        degree = 0;
        _BSM = PrefabPool.instance.GetManager(Managers.BaseManager).GetComponent<BaseManager>();
        StartCoroutine(UpdateCover());

        return true;
    }

    // Update is called once per frame
    public override void settingManager()
    {
        bool result = Lobby_Init();
        if (!result)
        {
            settingSuccess = false;
        }

        settingSuccess = result;
    }

    IEnumerator UpdateCover()
    {
        RaycastHit hit;
        while (true)
        {
            SkyRoll();

            Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(r, out hit, 50))
            {
                if (hit.collider.tag.Equals("BUTTON"))
                {
                    smr.material = mr[(int)shade.outline];

                    if (Input.GetButtonUp("Select"))
                        _BSM._loadScene(Managers.BattleManager, "LobbyScene");
                }
            }
            else
                smr.material = mr[(int)shade.flag];

            yield return null;
        }
    }

    void SkyRoll()
    {
        degree += Time.deltaTime;
        if (degree >= 360)
            degree = 0;

        RenderSettings.skybox.SetFloat("_Rotation", degree);
    }    

    RaycastHit GetHit()
    {
        Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Physics.Raycast(r, out hit, 50, 1 << 13);

        return hit;
    }
}
