using System;
using System.Reflection;
using System.Collections.Generic;
// using System.Runtime.Serialization.bformatters.Binary;
using System.IO;

namespace Hungry.Developer
{
    // Copyright 헝그리개발자(http://bemeal2.tistory.com)
    // 소스는 자유롭게 사용가능합니다. Copyright 는 삭제하지 마세요.

    // C# 객체를 복사해주는 클래스
    public class ObjectCopy
    {
        // 1. Deep Clone 구현
        public static T DeepClone<T>(T obj)
        {
            if (obj == null)
                throw new ArgumentNullException("Object cannot be null.");


            return (T)Process(obj, new Dictionary<object, object>() { });
        }



        private static object Process(object obj, Dictionary<object, object> circular)
        {
            if (obj == null)
                return null;

            Type type = obj.GetType();

            if (type.IsValueType || type == typeof(string))
            {
                return obj;
            }

            if (type.IsArray)
            {
                if (circular.ContainsKey(obj))
                    return circular[obj];



                string typeNoArray = type.FullName.Replace("[]", string.Empty);
                Type elementType = Type.GetType(typeNoArray + ", " + type.Assembly.FullName);
                var array = obj as Array;
                Array arrCopied = Array.CreateInstance(elementType, array.Length);

                circular[obj] = arrCopied;



                for (int i = 0; i < array.Length; i++)
                {
                    object element = array.GetValue(i);
                    object objCopy = null;



                    if (element != null && circular.ContainsKey(element))
                        objCopy = circular[element];
                    else
                        objCopy = Process(element, circular);



                    arrCopied.SetValue(objCopy, i);
                }



                return Convert.ChangeType(arrCopied, obj.GetType());
            }



            if (type.IsClass)
            {
                if (circular.ContainsKey(obj))
                    return circular[obj];



                object objValue = Activator.CreateInstance(obj.GetType());
                circular[obj] = objValue;
                FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);



                foreach (FieldInfo field in fields)
                {
                    object fieldValue = field.GetValue(obj);

                    if (fieldValue == null)
                        continue;



                    object objCopy = circular.ContainsKey(fieldValue) ? circular[fieldValue] : Process(fieldValue, circular);
                    field.SetValue(objValue, objCopy);
                }

                return objValue;
            }
            else
                throw new ArgumentException("Unknown type");
        }

        /*
        // 2. Serializable 객체에 대한  Deep Clone
        public static T SerializableDeepClone<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                var bformatter = new Binarybformatter();
                bformatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)bformatter.Deserialize(ms);
            }
        }*/
    }
}