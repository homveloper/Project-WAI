using System;
using UnityEngine;

public static class TransformExtention
{
    public static Transform FirstOrDefault(this Transform transform, Func<Transform, bool> query)
    {
        if (query(transform)) {
            return transform;
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            var result = FirstOrDefault(transform.GetChild(i), query);
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }
}