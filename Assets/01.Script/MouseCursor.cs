using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCursor : MonoBehaviour
{
    public enum Mode { IDLE, ATT, CK_MY, CK_EN, MOVE}

    [SerializeField] Texture2D[] cursorTexture;
    [SerializeField] Texture2D[] sideCursor;
    [SerializeField] bool hotSpotIsCenter = false;
    
    Vector2 adjustHotSpot = Vector2.zero;
    Vector2 hotSpot;

    Texture2D nowCursor;
    Texture2D moveCursor;
    Mode mode = Mode.IDLE;

    void Start()
    {
        moveCursor = sideCursor[4];
        MngPack.instance.MCS = this;
        setMode(Mode.IDLE);
        StartCoroutine("MyCursor");
    }

    IEnumerator MyCursor()
    {
        yield return new WaitForEndOfFrame();

        if (hotSpotIsCenter)
        {
            hotSpot.x = nowCursor.width / 2;
            hotSpot.y = nowCursor.height / 2;
        }
        else
        {
            hotSpot = adjustHotSpot;
        }

        Cursor.SetCursor(nowCursor, hotSpot, CursorMode.Auto);
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void setMode(Mode md)
    {
        switch (md)
        {
            case Mode.IDLE:
                nowCursor = moveCursor;
                hotSpot = adjustHotSpot;
                break;
            case Mode.ATT:
                nowCursor = cursorTexture[1];
                hotSpot = adjustHotSpot;
                break;
            case Mode.CK_MY:
                nowCursor = cursorTexture[2];
                hotSpot = new Vector2(32, 32);
                break;
            case Mode.CK_EN:
                nowCursor = cursorTexture[3];
                hotSpot = new Vector2(32, 32);
                break;
            case Mode.MOVE:
                nowCursor = moveCursor;
                break;
        }

        Cursor.SetCursor(nowCursor, hotSpot, CursorMode.Auto);
    }

    public void MoveSetPos(int ver = 0, int hor = 0)
    {
        int arrow = 4;

        if (ver > 0)
        {
            hotSpot.x = nowCursor.width;
            arrow += 1;
        }
        else if (ver < 0)
        {
            hotSpot.x = 0;
            arrow -= 1;
        }

        if (hor > 0)
        {
            hotSpot.y = 0;
            arrow -= 3;
        }
        else if (hor < 0)
        {
            hotSpot.y = nowCursor.height;
            arrow += 3;
        }

        moveCursor = sideCursor[arrow];

        if (ver != 0 || hor != 0)
        {
            setMode(Mode.MOVE);
        }
        else if (ver == 0 && hor == 0 && mode == Mode.MOVE)
        {
            setMode(Mode.IDLE);
        }
    }
}
