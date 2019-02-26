using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructStatus
{
    string Name;
    bool complete;
    int size;
    float hp;
    float maxhp;
    string produce;
    string connect;
    int Value;
    string precede;
    int delay;
    int[][] node;
    bool rally;

    public string NAME
    {
        get { return Name; }
        set { Name = value; }
    }
    public bool COMPLETE
    {
        get { return complete; }
        set { complete = value; }
    }
    public int SIZE
    {
        get { return size; }
        set { size = value; }
    }
    public float HP
    {
        get { return hp; }
        set { hp = value; }
    }
    public float MAXHP
    {
        get { return maxhp; }
        set { maxhp = value; }
    }
    public float PERCENT_HP
    {
        get { return hp / maxhp; }
    }
    public string PRODUCE
    {
        get { return produce; }
        set { produce = value; }
    }

    public string CONNECT
    {
        get { return connect; }
        set { connect = value; }
    }

    public int VALUE
    {
        get { return Value; }
        set { Value = value; }
    }    
    public string PRECEDE
    {
        get { return precede; }
        set { precede = value; }
    }
    public int DELAY
    {
        get { return delay; }
        set { delay = value; }
    }

    public int[][] NODE
    {
        get { return node; }
        set { node = value; }
    }
    public bool RALLY
    {
        get { return rally; }
        set { rally = value; }
    }

    public StructStatus()
    {
        Name = null;
        complete = false;
        size = 0;
        hp = 0;
        maxhp = 0;
        produce = null;
        connect = null;
        Value = 0;
        precede = null;
        delay = 0;
        node = null;
        rally = false;
    }

    public StructStatus(string _Name, bool _complete, int _size, float _hp, string _produce, string _connect, int _Value, string _precede, int _delay, int _rally)
    {
        Name = _Name;
        complete = _complete;
        size = _size;
        hp = _hp;
        maxhp = _hp;
        produce = _produce;
        connect = _connect;
        Value = _Value;
        precede = _precede;
        delay = _delay;
        node = new int[_size * _size][];
        for (int i = 0; i < 2; i++)
        {
            node[i] = new int[_size* _size];
        }
        RALLY = (_rally == 1);
    }
}
