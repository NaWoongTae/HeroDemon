using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class Grid : MonoBehaviour
{
    // 내부 클래스
    // 현재 클래스 내부에서만 사용가능
    [Serializable]
    public class TerrainType
    {
        public LayerMask _terrainMask;
    }

    public LayerMask _unwalkableMask;
    // 맵크기
    public Vector2 _gridWorldSize;
    // 노드의 반지름
    public float _nodeRadius;

    public TerrainType[] _walkableRegions;
    public int _obstaclePromiGrade = 10;
    LayerMask _walkableMask;

    Node[,] grid;

    public Node[,] GRID
    {
        get { return grid; }
    }

    // 노드의 지름
    float _nodeDiameter;
    // 가로,세로 노드 개수
    int _gridsizeX, _gridsizeY;

    public List<Node> _ltPath;
    public bool _DisplayDrawGizmos;

    public int _MapSizeW
    {
        get { return _gridsizeX; }
    }

    public int _MapSizeH
    {
        get { return _gridsizeY; }
    }

    // Use this for initialization
    public bool _Init()
    {
        _nodeDiameter = _nodeRadius * 2;
        _gridsizeX = Mathf.RoundToInt(_gridWorldSize.x / _nodeDiameter);
        _gridsizeY = Mathf.RoundToInt(_gridWorldSize.y / _nodeDiameter);

        return CreateGrid();       
    }
	
	// Update is called once per frame
	bool CreateGrid()
    {
        // 동적 생성
        grid = new Node[_gridsizeX, _gridsizeY];
        TextAsset textAsset = Resources.Load<TextAsset>("Data\\imgData");
        string[] Line = textAsset.text.Split('\n');
        
        // 전맵의 왼쪽밑으로 이동
        Vector3 worldBottomLft = new Vector3(0, 0, 0);
        for (int y = 0; y < _gridsizeY; y++)
        {
            for (int x = 0; x < _gridsizeX; x++)
            {
                // 각 셀을 돌면서 그 셀의 중점위치
                Vector3 worldPoint = worldBottomLft + (Vector3.right * (x * _nodeDiameter + _nodeRadius)) + (Vector3.forward * (y * _nodeDiameter + _nodeRadius));
                //                   physics체크       위치에서  반지름만큼 돌려서 잇나없나
                bool walkable = true;
                bool buildable = (Line[(Line.Length-1)-y][x] == '0') ? false : true;
                
                // 값
                grid[x, y] = new Node(walkable, buildable, worldPoint, x, y);
            }
        }

        return true;
    }    

    // 선택된 노드 주변의 노드를 받아오는 
    public List<Node> GetNeighbor(Node node)
    {
        List<Node> neighbor = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < _gridsizeX && checkY >= 0 && checkY < _gridsizeY)
                {
                    neighbor.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbor;
    }

    // 현재 위치를 그리드의 위치로 전환
    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        int x = (int)(worldPosition.x / _nodeDiameter);
        int y = (int)(worldPosition.z / _nodeDiameter);        

        return grid[x, y];
    }

    // 드로우 큐브
    void OnDrawGizmos()
    {
        //Gizmos.DrawWireCube(transform.position, new Vector3(_gridWorldSize.x, 1, _gridWorldSize.y));
        Gizmos.DrawWireCube(transform.position + new Vector3(_gridWorldSize.x/2,0, _gridWorldSize.y/2), new Vector3(_gridWorldSize.x, 1, _gridWorldSize.y));
        if (grid != null && _DisplayDrawGizmos)
        {
            foreach (Node area in grid)
            {
                // Gizmos.color = Color.Lerp(Color.white, Color.black, Mathf.InverseLerp(_penaltyMin, _penaltyMax, area.movementPenalty));
                Gizmos.color = (area.walkable) ? Gizmos.color : Color.red;
                Gizmos.DrawCube(area.worldPosition, new Vector3(_nodeDiameter - 0.1f, 0.1f, _nodeDiameter - 0.1f));
            }
        }
    }
}
