using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSign : MonoBehaviour {

    float t = 0;

    // Update is called once per frame
    void Update ()
    {
        t += Time.deltaTime;
        float s1 = 1f * (0.5f - t), s2 = 1f * (0.5f - t);

        if (s1 > 0)
        {
            transform.localScale = new Vector3(s1, s2, 1f);
        }
        if (t > 1)
            Destroy(gameObject);
	}
}
