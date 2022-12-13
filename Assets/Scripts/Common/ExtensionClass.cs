using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 扩展方法
/// </summary>
public static class ExtensionClass
{
    // 给List<Point>类型的对象添加扩展方法
    public static Vector3[] ToVector3Array(this List<Point> list){
        Vector3[] vector3s = new Vector3[list.Count];
        for (int i = 0; i < list.Count; i++)
        {
            vector3s[i] = new Vector3(list[i].x, list[i].y, 0);
        }
        return vector3s;
    }

    // Vector3 转 Vector3Int
    public static Vector3Int ToVector3Int(this Vector3 vector3){
        Vector3Int vector3Int = new Vector3Int(Mathf.RoundToInt(vector3.x), Mathf.RoundToInt(vector3.y), 0);
        return vector3Int;
    }
}
