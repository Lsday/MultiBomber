using System.Collections;
using UnityEngine;
using TMPro;
using Mirror;
using System;
using System.Collections.Generic;

public static class Utils
{
    public static void FreeMemory()
    {
        GC.Collect();
        Resources.UnloadUnusedAssets();
    }

    public static int RoundedSign(float value, float threshold = 0.001f)
    {
        return (value < -threshold ? -1 : value > threshold ? 1 : 0);
    }
    public static TextMeshPro CreateWorldText(string name, string text, Transform parent = null, Vector3 localPosition = default(Vector3), int fontsize = 40, Color color = default, TextAnchor textAnchor = TextAnchor.UpperLeft, TextAlignmentOptions textAlignment = TextAlignmentOptions.Center, int sortingOrder = 5000)
    {
        if (color == null) color = Color.white;
        return CreateWorldText(name, parent, text, localPosition, fontsize, (Color)color, textAnchor, textAlignment, sortingOrder);
    }
    public static TextMeshPro CreateWorldText(string name, Transform parent, string text, Vector3 localPosition, int fontSize, Color color, TextAnchor textAnchor, TextAlignmentOptions textAlignment, int sortingOrder)
    {
        GameObject gameObject = new GameObject(name, typeof(TextMeshPro));
        Transform transform = gameObject.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;
        Quaternion rotation = Quaternion.Euler(90f, 0, 0);
        transform.rotation = rotation;
        TextMeshPro textMesh = gameObject.GetComponent<TextMeshPro>();
        //textMesh.anchor = textAnchor;
        textMesh.alignment = textAlignment;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color;
        textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        return textMesh;
    }
    private static Vector3 GetMouseWorldPosition2D(Vector3 screenPosition, Camera worldCamera)
    {
        Debug.Log(worldCamera.ScreenToWorldPoint(screenPosition));
        return worldCamera.ScreenToWorldPoint(screenPosition);
    }

    private static Vector3 GetMouseWorldPosition3D(Vector3 mousePos, Camera worldCamera)
    {
        Ray ray = worldCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        Debug.DrawLine(ray.origin, Vector3.zero, Color.red, 100f);
        if (Physics.Raycast(ray, out hit, 1000f))
        {
            Debug.DrawLine(worldCamera.transform.position, hit.point);
            Debug.Log(hit.point);
            return hit.point;

        }
        return Vector3.zero;
    }


    public static Vector3 GetMouseWorldPosition()
    {
        return GetMouseWorldPosition3D(Input.mousePosition, Camera.main);
    }

    public static ElementType SetFlag(ElementType a, ElementType b)
    {
        return a | b;
    }
    public static ElementType UnsetFlag(ElementType a, ElementType b)
    {
        return a & (~b);
    }
    // Works with "None" as well
    public static bool HasFlag(ElementType a, ElementType b)
    {
        return (a & b) == b;
    }
    public static ElementType ToogleFlag(ElementType a, ElementType b)
    {
        return a ^ b;
    }

}