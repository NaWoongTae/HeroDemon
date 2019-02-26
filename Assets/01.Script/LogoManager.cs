using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogoManager : ManagerSetting
{
    [SerializeField] Animation ani;
    BaseManager _BSM;

	// Use this for initialization
	void Start ()
    {
        _BSM = PrefabPool.instance.GetManager(Managers.BaseManager).GetComponent<BaseManager>();
    }

    private void Update()
    {
        if (!ani.isPlaying)
            nextScene();
    }

    public override void settingManager()
    {
        settingSuccess = true;
    }

    // Update is called once per frame
    public void nextScene()
    {
        _BSM._loadScene(Managers.LobbyManager, "LogoScene");
    }
}
