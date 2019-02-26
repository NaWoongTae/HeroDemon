using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
        
    Vector3 mousePosition;
    Vector3 keyboard;
    float FOV;
    float toZero;
    float toOut;

    Vector3 willMove
    {
        get { return transform.position + keyboard; }
    }

    public bool Init()
    {
        FOV = GetComponentInChildren<Camera>().fieldOfView;
        toZero = (FOV / 2) - 7.5f;
        toOut = 128 - toZero;

        return true;
    }

	// Use this for initialization
	void Start ()
    {
        StartCoroutine(MoveCam());
	}

    IEnumerator MoveCam()
    {
        while (true)
        {
            yield return null;

            if (MngPack.instance.UIM.MENUWIN)
            {
                continue;
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                keyboard = new Vector3(Time.deltaTime * 20, 0, 0);
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                keyboard = new Vector3(-Time.deltaTime * 20, 0, 0);
            }
            else if (Input.GetKey(KeyCode.UpArrow))
            {
                keyboard = new Vector3(0, 0, Time.deltaTime * 20);
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                keyboard = new Vector3(0, 0, -Time.deltaTime * 20);
            }
            else
            {
                keyboard = Vector3.zero;
            }

            if (willMove.x + willMove.z >= 0 || willMove.x <= 1919f || willMove.z <= 1079f)
            {
                transform.position += keyboard;
            }

            resetPosition(Input.mousePosition);            
        }
    }

    void resetPosition(Vector3 vec)
    {
        MngPack.instance._DT.testDebug("(" + vec.x.ToString() + ", " + vec.y.ToString() + ")");
        int x = 0, y = 0;
        if (vec.x <= 0)
        {
            transform.Translate(new Vector3(-1, 0, 0));            
            x = (vec.x == 0) ? -1 : 0;
        }
        else if (vec.x >= 1919)
        {
            transform.Translate(new Vector3(1, 0, 0));
            x = (vec.x == 1919) ? 1 : 0;
        }

        if (vec.y <= 0)
        {
            transform.Translate(new Vector3(0, 0, -1));
            y = (vec.y == 0) ? -1 : 0;
        }
        else if (vec.y >= 1079)
        {
            transform.Translate(new Vector3(0, 0, 1));
            y = (vec.y == 1079) ? 1 : 0;
        }

        MngPack.instance.MCS.MoveSetPos(x, y);

        if (transform.position.x < toZero)
            transform.position = new Vector3(toZero, transform.position.y, transform.position.z);
        else if (transform.position.x > toOut)
            transform.position = new Vector3(toOut, transform.position.y, transform.position.z);

        if (transform.position.z <= -4f)
            transform.position = new Vector3(transform.position.x, transform.position.y, -4f);
        else if (transform.position.z >= 110)
            transform.position = new Vector3(transform.position.x, transform.position.y, 110);
    }
}
