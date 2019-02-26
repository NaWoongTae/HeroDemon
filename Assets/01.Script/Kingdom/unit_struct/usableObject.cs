using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class usableObject : MonoBehaviour {

    [SerializeField] GameObject SelectSign;
    bool play;

    protected GameObject Circle
    {
        get { return SelectSign; }
    }

    // Use this for initialization
    protected void init()
    {
        play = false;
    }

    public virtual void WhenSelected()
    { }

    public virtual void WhenOff()
    { }

    protected void _selectSign()
    {
        SelectSign.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.green * 0.5f);
        if (SelectSign.activeSelf)
            SelectSign.transform.Rotate(Vector3.forward * 60 * Time.deltaTime);
    }

    public void flicker()
    {
        if (play)
        {
            play = false;
            StopCoroutine("flick");
        }

        StartCoroutine("flick");
    }

    IEnumerator flick()
    {
        play = true;
        int t = 0;

        while (t < 21)
        {
            t += 1;
            if (t % 3 == 0)
                SelectSign.SetActive(!SelectSign.activeSelf);

            yield return new WaitForSeconds(0.1f);
        }

        SelectSign.SetActive(KingdomManager.isSelct.isContain(gameObject));
        play = false;
    }

    public void ImSelect(bool isSelect)
    {
        SelectSign.SetActive(isSelect);
    }
}
