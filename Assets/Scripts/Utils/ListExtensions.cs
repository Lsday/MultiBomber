using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

static class ListExtensions
{
    public static T Pop<T>(this IList<T> list)
    {
        if (list is null || list.Count == 0) return default(T);
        T item = list[list.Count - 1];
        list.RemoveAt(list.Count - 1);
        return item;
    }
}
