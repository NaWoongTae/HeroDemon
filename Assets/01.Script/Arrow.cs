using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public enum Projectile { arrow, shell }
    Projectile type;
    [SerializeField] float speed;
    SphereCollider sphere;
    GameObject target;
    int Damage;
    bool settingSuccess = false;

    private void Start()
    {
        sphere = GetComponentInChildren<SphereCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (settingSuccess)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
            
            if (Vector3.Distance(target.transform.position, transform.position) < 0.2f)
            {
                target.GetComponent<Filter>().GetDamage(Damage);
                destroyEff(gameObject);
            }
        }
    }

    public void Settings(GameObject go, Projectile t, int damage)
    {
        target = go;
        Damage = damage;
        type = t;
        settingSuccess = true;
    }

    void destroyEff(GameObject go)
    {
        switch (type)
        {
            case Projectile.shell:
                GameObject gfo = Instantiate(DataMng.instance.PREFAB_G["EXP"], transform.position, transform.rotation);
                Destroy(gfo, 1.5f);
                break;
            default:
                break;
        }

        Destroy(go);
    }
}
