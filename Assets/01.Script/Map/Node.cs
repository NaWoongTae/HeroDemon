using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>{

    public bool walkable;
    public bool buildable;

    public Vector3 worldPosition;
    public int gridX;
    public int gridY;       
    
    int _heapIndex;

    public int _HeapIndex
    {
        get
        {
            return _heapIndex;
        }
        set
        {
            _heapIndex = value;
        }
    }

    public Node(bool _walkable, bool _buildable, Vector3 _worldPosition, int gridx, int gridy)
    {
        this.walkable = _walkable;
        this.buildable = _buildable;
        this.worldPosition = _worldPosition;
        gridX = gridx;
        gridY = gridy;
    }

    public int CompareTo(Node nodeToCompare)
    {
        

        return 0;
    }
}
