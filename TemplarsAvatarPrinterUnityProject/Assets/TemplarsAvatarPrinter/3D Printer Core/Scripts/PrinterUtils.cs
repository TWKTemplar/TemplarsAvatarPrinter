using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PrinterUtils
{
    private static Random rng = new Random();
    public static Vector3 LocalToWorldPos(Vector3 pos,Transform transform)
    {
        Matrix4x4 localToWorld = transform.localToWorldMatrix;
        return localToWorld.MultiplyPoint3x4(pos);
    }
    public static void Shuffle<T>(this IList<T> list)
    {
        System.Random rnd = new System.Random();
        for (var i = list.Count - 1; i > 0; i--)
        {
            var randomIndex = rnd.Next(i + 1); //maxValue (i + 1) is EXCLUSIVE
            list.Swap(i, randomIndex);
        }
    }
    public static void Swap<T>(this IList<T> list, int indexA, int indexB)
    {
        var temp = list[indexA];
        list[indexA] = list[indexB];
        list[indexB] = temp;
    }
}
