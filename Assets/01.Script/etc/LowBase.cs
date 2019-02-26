using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public  class LowBase
{
    // Dictionary
    Dictionary<string, Dictionary<string, string>> nodeDic = new Dictionary<string, Dictionary<string, string>>();

    // nodeDic의 프로퍼티
	public Dictionary< string, Dictionary< string, string > > NODE
    {
        get
        { return nodeDic; }
        set
        { nodeDic = value; }
    }
	
    // int, string 의 키값을 string, string으로 변환
	string Str ( int Key, string subKey )
	{
		string keyStr = Key.ToString ();
		return Str ( keyStr, subKey ); // 매개변수가 다른 오버로드된 함수
	}
	
    // 같은 이름의 오버로드함수 string, string으로 받아와 값을 리턴
	public string Str( string Key, string subKey )
	{
		string findValue = string.Empty;
		if( nodeDic.ContainsKey ( Key ) )
			nodeDic[ Key ].TryGetValue ( subKey, out findValue );
		
		return findValue;
	}
	
    // string으로 리턴해준다.
	public string ToS( int Key, string subKey )
	{
		string keyStr = Key.ToString();
		string findValue = string.Empty;
		if( nodeDic.ContainsKey ( keyStr )) // nodeDic이 keyStr의 키를 포함 한다면 
            nodeDic[ keyStr ].TryGetValue ( subKey, out findValue ); // 값은 out에 의해 findValue로 들어가게된다
		
		return findValue;
	}

    // string형태의 숫자를 int로 변환
	private int TryParseToInt( string Key )
	{
		int Value = 0;
		int.TryParse (Key, out Value);
		return Value;
	}

    // string,string형태로 값을 찾아 반환(반환값 int)
	public int ToI( string Key, string subKey )
	{
		string findStr = Str (Key, subKey);
		return TryParseToInt (findStr);
	}

    // int string형태로 값을찾아 반환(반환값 int)
    public int ToI( int Key, string subKey )
	{
		string findStr = Str ( Key, subKey );
		return TryParseToInt (findStr);
	}

    // 어떤형태의 값이든 string형태로 변환해 refValue에 넣어줌
    public void To( object Key, object subKey, ref object refValue )
	{
		string MainKey = Key.ToString ();
		string SubKey = subKey.ToString ();
		refValue = Str (MainKey, SubKey); 
	}

    // 어떤형태의 값이든 string형태로 변환해 값을찾은후
    // System.Type t의 형태로 반환
    public object To( System.Type t, object Key, object subKey  )
	{

		string MainKey = Key.ToString ();
		string SubKey = subKey.ToString ();
		string findStr = Str (MainKey, SubKey);

		if (t == typeof(byte))
			return byte.Parse (findStr);
		else if( t == typeof( int ) )
			return int.Parse (findStr );
		else if( t == typeof( float ) )
			return float.Parse ( findStr );
		else if( t == typeof( double ) )
			return double.Parse ( findStr );
		else if( t == typeof( string ) )
			return findStr;
		else if( t == typeof( bool ) )
			return bool.Parse ( "FALSE" );

		return null;
	}

    // int, string으로 받아와 ushort의 형태로 반환
    public ushort ToUS( int Key, string subKey )
	{
		string findStr = Str ( Key, subKey );
		ushort Value = 0;
		ushort.TryParse ( findStr, out Value );
		return Value;
	}

    // int, string으로 받아와 float의 형태로 반환
    public float ToF( int Key, string subKey )
	{
		string findStr = Str ( Key, subKey );
		float Value = 0;
		float.TryParse ( findStr, out Value );
		return Value;
	}

    // int, string으로 받아와 double의 형태로 반환
    public double ToD( int Key, string subKey )
	{
		string findStr = Str ( Key, subKey );
		double Value = 0;
		double.TryParse ( findStr, out Value );
		return Value;
	}

    // int, string으로 받아와 bool의 형태로 반환
    public bool ToBl( int Key, string subKey )
	{
		string findStr = Str ( Key, subKey );
		bool Value = false;
		bool.TryParse ( findStr, out Value );
		return Value;
	}

    // int, string으로 받아와 byte의 형태로 반환
    public byte ToBy( int Key, string subKey )
	{
		string findStr = Str ( Key, subKey );
		byte Value = 0;
		byte.TryParse ( findStr, out Value );
		return Value;
	}
	
    // key, subkey의 여부 확인후 val(값) 삽입
	public void Add( string key, string subKey, string val )
	{
		if( ! nodeDic.ContainsKey ( key ) )
			nodeDic.Add ( key, new Dictionary< string, string >() );
			
		if (! nodeDic [key].ContainsKey (subKey))
			nodeDic [key].Add (subKey, val);
	}

    // 노드의 개수 반환
	public int MaxCount()
	{
		return nodeDic.Count;
	}

    // Cnt번째의 subKey의 값 반환
    public string ToSfromCount ( int Cnt, string subKey )
	{

		int count = 0;
		string findValue = "";
		foreach (KeyValuePair< string, Dictionary<string, string> > val in nodeDic ) 
		{
			if( count == Cnt )
			{
				val.Value.TryGetValue( subKey, out findValue );
				break;
			}

			++count;
		}
		//KeyValuePair
		return findValue;
	}

	public virtual void Load( string strJson ) {}

}