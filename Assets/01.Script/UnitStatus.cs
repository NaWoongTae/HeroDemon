using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStatus
{
    string Name;
    string type;
    int cost;    
    float hp;
    float maxhp;
    int str;
    int strup;
    int def;
    int defup;
    float attspeed;
    float critical;
    int mp;
    int maxmp;
    float range;
    float notice;
    float sight;
    int speed;
    string manufacturer;
    int population;

    public string NAME
    {
        get { return Name; }
        set { Name = value; }
    }
    public int COST
    {
        get { return cost; }
        set { cost = value; }
    }
    public string TYPE
    {
        get { return type; }
        set { type = value; }
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
    public int STR(bool bl)
    {
        if (bl)
        {
            return str + KingdomManager.isSelct.str_count * strup;
        }

        return str;
    }
    public int STRUP
    {
        set { strup = value; }
    }
    public int DEF(bool bl)
    {
        if (bl)
        {
            return def + KingdomManager.isSelct.def_count * defup; 
        }
        return def;
    }
    public int DEFUP
    {
        set { defup = value; }
    }
    public float ATTSPEED
    {
        get { return attspeed; }
        set { attspeed = value; }
    }
    public float CRITICAL
    {
        get { return critical; }
        set { critical = value; }
    }
    public int MP
    {
        get { return mp; }
        set { mp = value; }
    }
    public int MAXMP
    {
        get { return maxmp; }
        set { maxmp = value; }
    }
    public float RANGE
    {
        get { return range; }
        set { range = value; }
    }
    public float NOTICE
    {
        get { return notice; }
        set { notice = value; }
    }
    public float SIGHT
    {
        get { return sight; }
        set { sight = value; }
    }
    public int SPEED
    {
        get { return speed; }
        set { speed = value; }
    }
    public string MANUFACTURER
    {
        get { return manufacturer; }
        set { manufacturer = value; }
    }
    public int POPULATION
    {
        get { return population; }
        set { population = value; }
    }

    public UnitStatus()
    {
        Name = null;
        cost = 0;
        type = null;
        hp = 0;
        maxhp = 0;
        str = 0;
        def = 0;
        attspeed = 0;
        critical = 0;
        mp = 0;
        maxmp = 0;
        range = 0;
        notice = 0;
        sight = 0;
        speed = 0;
        manufacturer = null;
        population = 0;
    }

    public UnitStatus(string name)
    {
        Name = name;
        cost = 0;
        type = null;
        hp = 0;
        maxhp = 0;
        str = 0;
        def = 0;
        attspeed = 0;
        critical = 0;
        mp = 0;
        maxmp = 0;
        range = 0;
        notice = 0;
        sight = 0;
        speed = 0;
        manufacturer = null;
        population = 0;
    }

    public UnitStatus(string _name, string _type, int _cost, float _hp, int _str, int _def, float _attspeed, float _critical,
    int _mp, float _range, float _notice, float _sight, int _speed, string _manufacturer, int _population)
    {
        Name = _name;
        cost = _cost;
        type = _type;
        hp = _hp;
        maxhp = _hp;
        str = _str;
        def = _def;
        attspeed = _attspeed;
        critical = _critical;
        mp = _mp;
        maxmp = _mp;
        range = _range;
        notice = _notice;
        sight = _sight;
        speed = _speed;
        manufacturer = _manufacturer;
        population = _population;
    }
}
