using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapViewCtrl : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField] Image[] _minimapCase;

    CtrlManager CTM;
    MoveCamera moveCam;

    Vector3 minimapPos;

    // Use this for initialization
    void Start ()
    {
        CTM = GetComponentInParent<CtrlManager>();
        moveCam = Camera.main.GetComponentInParent<MoveCamera>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        _minimapCameraMark();  
    }

    // 미니맵 클릭
    public void OnPointerDown(PointerEventData data) // 명령이없을땐 좌클릭 명령이 있을땐 우클릭
    {
        float x = (data.position.x - 20.0f) / 400.0f * 128.0f;
        float y = (data.position.y - 20.0f) / 400.0f * 128.0f;// - 12.0f; ? 뭐지

        minimapPos = new Vector3(x, 0, y);

        if (data.button == ((!CTM.IsORDER) ? PointerEventData.InputButton.Right : PointerEventData.InputButton.Left)) // 우 -//- 미니맵에서 이동,랠리포인트 설정
        {
            if (data.button == PointerEventData.InputButton.Right)
                minimaposCheck(minimapPos);
            else if (data.button == PointerEventData.InputButton.Left)
                minimaposCheck(minimapPos, true);
        }
        if (data.button == ((!CTM.IsORDER) ? PointerEventData.InputButton.Left : PointerEventData.InputButton.Right)) // 좌 -//- 화면이동
        {
            moveCam.transform.position = new Vector3(minimapPos.x, 0, minimapPos.z);
        }
        // throw new System.NotImplementedException();
    }

    // 미니맵에서 드래그로 화면이동
    public void OnDrag(PointerEventData data)
    {
        if (data.button == PointerEventData.InputButton.Left)
        {
            float x = (data.position.x - 20.0f) / 400.0f * 128.0f;
            float y = (data.position.y - 20.0f) / 400.0f * 128.0f;

            minimapPos = new Vector3(x, 0, y);

            moveCam.transform.position = new Vector3(minimapPos.x, 0, minimapPos.z);
        }
    }

    void minimaposCheck(Vector3 minimapPos , bool order = false)
    {
        RaycastHit hit;
        Physics.Raycast(minimapPos + new Vector3(0, 15f, 0), Vector3.down, out hit, 20, (1 << 9));

        Vector3 _targetPos = hit.point;

        CTM.makeMoveSign(_targetPos);

        if (!order)
        {
            for (int i = 0; i < KingdomManager.isSelct.SelectObj.Count; i++)
            {
                KingdomManager.isSelct.SelectObj[i].GetComponent<Filter>().Adjustment(_targetPos);
            }
        }
        else
        {
            for (int i = 0; i < KingdomManager.isSelct.SelectObj.Count; i++)
            {
                KingdomManager.isSelct.SelectObj[i].GetComponent<Filter>().middleOrder(hit);
            }
            CTM.GetComponent<UIManager>().ScreenInterface();
        }
    }

    // 미니맵에 카메라 위치 만들기
    void _minimapCameraMark()
    {
        Vector3[] screen = { new Vector3(0, 0, 0), new Vector3(1920, 0, 0), new Vector3(0, 1080, 0), new Vector3(1920, 1080, 0) };
        Vector2[] minimap = new Vector2[4];

        for (int i = 0; i < 4; i++)
        {
            Ray r = Camera.main.ScreenPointToRay(screen[i]);
            RaycastHit hit;
            if (Physics.Raycast(r, out hit))
            {
                Vector2 vec = new Vector2();
                vec.x = (hit.point.x < 0) ? 20 : (hit.point.x / 128 * 400) + 20;
                vec.y = (hit.point.z < 0) ? 20 : (hit.point.z / 128 * 400) + 20;
                minimap[i] = vec;
            }
        }

        for (int i = 0; i < _minimapCase.Length; i++)
        {
            _minimapCase[0].GetComponent<DrawLine>().DrawGraph(minimap[0], minimap[1]);
            _minimapCase[1].GetComponent<DrawLine>().DrawGraph(minimap[2], minimap[3]);
            _minimapCase[2].GetComponent<DrawLine>().DrawGraph(minimap[0], minimap[2]);
            _minimapCase[3].GetComponent<DrawLine>().DrawGraph(minimap[1], minimap[3]);
        }
    }
}
