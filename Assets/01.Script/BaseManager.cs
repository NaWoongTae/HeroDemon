using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BaseManager : MonoBehaviour
{
    [SerializeField] GameObject loadwin;
    [SerializeField] Image loadbar;

	// Use this for initialization
	void Start ()
    {
        DataMng.instance.LoadData();
        PrefabPool.instance.MngDic.Add(Managers.BaseManager, gameObject);
        loadbar.fillAmount = 0;
        StartCoroutine(LogoScene());
	}

    IEnumerator LoadingScene(Managers load, string remove = "")
    {
        loadwin.SetActive(true);
        loadbar.fillAmount = 0;

        AsyncOperation AO;
        if (!remove.Equals(string.Empty))
        {
            AO = SceneManager.UnloadSceneAsync(remove);

            while (!AO.isDone)
            {
                loadbar.fillAmount = AO.progress / 3;
                yield return new WaitForSeconds(0.5f);
            }
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneAt(0));
        loadbar.fillAmount = 0.5f;
        yield return new WaitForSeconds(0.5f);
        AO = SceneManager.LoadSceneAsync((int)load, LoadSceneMode.Additive);
        while (!AO.isDone)
        {
            yield return new WaitForSeconds(0.5f);
            loadbar.fillAmount = 0.5f + AO.progress/3;
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneAt(1));
        loadbar.fillAmount = 1f;

        yield return new WaitForSeconds(0.1f);

        GameObject go = PrefabPool.instance.SetManager(load);
        go.GetComponentInChildren<ManagerSetting>().settingManager();

        yield return new WaitUntil(go.GetComponentInChildren<ManagerSetting>().SUCCESS);

        yield return new WaitForSeconds(0.1f);

        loadwin.SetActive(false);
    }

    IEnumerator LogoScene()
    {
        AsyncOperation AO = SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
        while (!AO.isDone)
        {
            yield return new WaitForSeconds(0.3f);
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneAt(1));
        PrefabPool.instance.GetManager(Managers.LogoManager);
    }

    public void _loadScene(Managers mng, string remove = "")
    {
        StartCoroutine(LoadingScene(mng, remove));
    }
}
