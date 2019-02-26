using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IHeapItem<T> : IComparable<T>
{
    int _HeapIndex
    {
        get;
        set;
    }
}

public class Heap<T>
{
    // 힙
    // 배열로 만듬
    public T[] _items;
    int _ItemNum;
    int maxSize = 5;
    int count;

    // 노드의 개수
    public int _Count
    {
        get
        {
            return count;
        }
    }

    // 힙 생성(맵의 최대크기)
    public Heap()
    {
        _items = new T[0];
        _ItemNum = 0;
        count = 0;
    }  

    public void Add(T item)
    {
        count++;

        T[] sample = _items;
        _items = new T[count];

        for (int i = 0; i < count-1; i++)
        {
            _items[i] = sample[i];
        }
        _items[count-1] = item;
    }

    public T RemoveFirst()
    {
        T firstItem = _items[0];

        count--;
        T[] sample = _items;
        _items = new T[count];

        for (int i = 0; i < count; i++)
        {
            _items[i] = sample[i+1];
        }

        return firstItem;
    }

    public void Remove(int num)
    {
        if (count > 0)
        {
            count--;
            T[] sample = _items;
            _items = new T[count];

            for (int i = 0; i < count; i++)
            {
                int plus = (i >= num) ? 1 : 0;
                _items[i] = sample[i+ plus];
            }
        }
    }

    public bool RemoveAt(T t)
    {
        if (count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                if (_items[i].Equals(t))
                {
                    Remove(i);
                    return true;
                }
                else
                {
                    Debug.Log("알림 : 같은게 없어");
                    return false;
                }
            }
        }
        else
        {
            Debug.Log("알림 : 지울게 없어");
            return false;
        }

        return false;
    }

    public bool Contains(T item)
    {
        if (count == 0)
            return false;
        else
        {
            for (int i = 0; i < _Count; i++)
            {
                if (_items[i].Equals(item))
                    return true;
            }
        }

        return false;
    }

    public T First()
    {
        return _items[0];
    }

    public T GetT(int num)
    {
        return _items[num];
    }

    public void Swap(int t1, int t2)
    {
        T tmp = _items[t1];
        _items[t1] = _items[t2];
        _items[t2] = tmp;
    }

    public void Clear()
    {
        _items = new T[] { };
        count = 0;
    }

    public T this[int num]
    {
        get
        {
            return _items[num];
        }
    }
}
