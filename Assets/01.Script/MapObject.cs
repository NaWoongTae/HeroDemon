using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapObject : MonoBehaviour
{
    // 미니맵 유닛 표시

    [SerializeField] GameObject square;

    struct Img
    {
        public GameObject images;
        Transform trans;

        public Img(GameObject go, Transform tr,int size)
        {
            images = go;
            trans = tr;
            images.GetComponent<RectTransform>().sizeDelta = new Vector2(size, size);
            images.GetComponent<RectTransform>().anchoredPosition = new Vector2(trans.position.x / 128f * 400f, trans.position.z / 128f * 400f);
        }

        public void refresh()
        {
            images.GetComponent<RectTransform>().anchoredPosition = new Vector2(trans.position.x / 128f * 400f, trans.position.z / 128f * 400f);
        }

        public bool isit()
        {
            if (trans == null)
                return false;

            return true;
        }
    }

    List<Img> imgs;

    void Awake()
    {
        imgs = new List<Img>();
    }

    // Use this for initialization
    public void Init ()
    {        
        MngPack.instance.MOB = this;
    }

    /// <summary>
    /// 미니맵
    /// </summary>
    /// <returns></returns>
    public IEnumerator mapObject()
    {
        while (true)
        {
            for (int i = 0; i < imgs.Count; i++)
            {
                if (!imgs[i].isit())
                {
                    Destroy(imgs[i].images);
                    imgs.Remove(imgs[i]);                    
                    break;
                }
                imgs[i].refresh();
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public void Insert(Transform tr, Filter.UnitType unitType)
    {
        int size = 0;
        GameObject go = Instantiate(square, gameObject.transform);
        switch (unitType)
        {
            case Filter.UnitType.KS:
                size = 6;
                break;
            case Filter.UnitType.KUC:
            case Filter.UnitType.KUH:
                size = 4;
                break;
            case Filter.UnitType.MNR:
                size = 5;
                break;
        }

        if (tr.tag == "KINGDOM")
        {
            go.GetComponent<Image>().color = Color.green;
        }
        else if (tr.tag == "DUMMY")
        {
            go.GetComponent<Image>().color = Color.red;
        }
        else if(tr.tag == "MINERAL")
        {
            go.GetComponent<Image>().color = new Color(0, 1, 1);
        }

        Img img = new Img(go, tr,size);
        imgs.Add(img);
    }
}
