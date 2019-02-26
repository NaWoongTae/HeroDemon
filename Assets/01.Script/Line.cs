using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line
{

    // double과 float의 차이점
    // double은 정수형 실수 = 0.1이라면 딱 0.1
    // float은 지수로서 0.1000001처럼 0.1이라고 할수있는값을 0.1f이라고 해준다
    const float _verticalLineGradient = 1e5f;

    float _gradient;
    float _interceptY;
    Vector2 _pointOnLine_1;
    Vector2 _pointOnLine_2;
    bool _approachSide;
    float _gradientPer;

    public Line(Vector2 pointOnLine, Vector2 pointPerToLine)
    {
        float Dx = pointOnLine.x - pointPerToLine.x;
        float Dy = pointOnLine.y - pointPerToLine.y;

        if (Dx == 0)
        {
            _gradientPer = _verticalLineGradient;
        }
        else
        {
            _gradientPer = Dy / Dx;
        }

        if (_gradientPer == 0)
        {
            _gradient = _verticalLineGradient;
        }
        else
        {
            _gradient = -1 / _gradientPer;
        }

        /*if (Dy == 0)
        {
            _gradient = _verticalLineGradient;
        }
        else
        {
            // _gradient 값은 두 점 a,b 로 부터 생성된값 dx,dy로
            // x가 1일때의 y값을 얻어낸다. == 이 값은 원점(0,0)으로 부터의 방향성을 나타내며
            // y가 무한일때 == dx/dx 에서 dy가 0 == 의 경우는 이미 if (Dy == 0)로 부터 걸러진다.
            // 이로서 얻은 두점 a,b를 양끝으로 하는 선분과 직각을 이루는 직선((0,0)으로 부터의 방향성)생성
            _gradient = -Dx / Dy;
        }*/

        _interceptY = pointOnLine.y - _gradient * pointOnLine.x;
        _pointOnLine_1 = pointOnLine;
        _pointOnLine_2 = pointOnLine + new Vector2(1, _gradient);

        _approachSide = false;
        _approachSide = GetSide(pointPerToLine);
    }

    bool GetSide(Vector2 p)
    {
        return ((p.x - _pointOnLine_1.x) * (_pointOnLine_2.y - _pointOnLine_1.y)) > ((p.y - _pointOnLine_1.y) * (_pointOnLine_2.x - _pointOnLine_1.x));
    }

    public bool HasCrossedLine(Vector2 p)
    {
        return GetSide(p) != _approachSide;
    }

    public float DistanceFromPoint(Vector2 p)
    {
        float yInterceptPerpebdicular = p.y - _gradientPer * p.x;
        float interceptX = (yInterceptPerpebdicular - _interceptY) / (_gradient - _gradientPer);
        float interceptY = _gradient * interceptX + _interceptY;

        return Vector2.Distance(p, new Vector2(interceptX, interceptY));
    }

    public void DrawWithGizmos(float length)
    {
        // _gradient == 방향성의 값                   / normalized == 정규벡터(길이가 1인 벡터)로 바꿔준다
        Vector3 linDir = new Vector3(1, 0, _gradient).normalized;
        // 원점의값
        Vector3 lineCenter = new Vector3(_pointOnLine_1.x, 0, _pointOnLine_1.y) + Vector3.up;
        // 원점부터 정방향과 역방향으로 length의 절반씩 / 그래서 *5가 가능함
        Gizmos.DrawLine(lineCenter - linDir * length / 2f, lineCenter + linDir * length / 2f);
    }
}