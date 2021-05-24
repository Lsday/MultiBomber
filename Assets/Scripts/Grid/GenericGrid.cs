using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GenericGrid<TGridObject>
{
    

    public event EventHandler<OnGridObjectChangedEventHandlerArgs> OnGridObjectChanged;
    public class OnGridObjectChangedEventHandlerArgs : EventArgs
    {
        public int x;
        public int y;
    }

    private int width;
    private int height;
    private float cellSize;
    private Vector3 originPosition;
    private TGridObject[,] gridArray;
    bool showDebug = false;
    TextMeshPro[,] debugTextArray;


    public GenericGrid(int width, int height, float cellSize, Vector3 originPosition, Func<GenericGrid<TGridObject>, int, int, TGridObject> constructor)
    {

        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;

        gridArray = new TGridObject[width, height];


        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int z = 0; z < gridArray.GetLength(1); z++)
            {
                gridArray[x, z] = constructor(this, x, z);
            }
        }

        if (showDebug)
        {
            Transform debugParent = GameObject.Find("Debug").transform;
            if (debugParent)
            {
                debugTextArray = new TextMeshPro[width, height];


                for (int x = 0; x < gridArray.GetLength(0); x++)
                {
                    for (int z = 0; z < gridArray.GetLength(1); z++)
                    {
                        debugTextArray[x, z] = Utils.CreateWorldText("GridData " + x + " " + z + " ", gridArray[x, z]?.ToString(), debugParent, GetGridObjectWorldPosition(x, z) + new Vector3(cellSize * 0.5f, 0, cellSize * 0.5f) + originPosition, 2, Color.white, TextAnchor.MiddleCenter);

                        Debug.DrawLine(GetGridObjectWorldPosition(x, z), GetGridObjectWorldPosition(x, z + 1), Color.white, 100f);
                        Debug.DrawLine(GetGridObjectWorldPosition(x, z), GetGridObjectWorldPosition(x + 1, z), Color.white, 100f);
                    }
                }
                Debug.DrawLine(GetGridObjectWorldPosition(0, height), GetGridObjectWorldPosition(width, height), Color.white, 100f);
                Debug.DrawLine(GetGridObjectWorldPosition(width, 0), GetGridObjectWorldPosition(width, height), Color.white, 100f);
            }
           
        }
    }

    public int GetWidth() => width;
    public int GetHeight() => height;
    public float GetCellsize() => cellSize;

    public Vector3 GetGridObjectWorldPosition(int x, int y)
    {
        return new Vector3(x, 0, y) * cellSize + originPosition;
    }

    public void GetXY(Vector3 worldPosition, out int x, out int y)
    {

        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).z / cellSize);

         Debug.Log("GetXY : " + x + ", " + y);
    }
    public void SetValue(int x, int y, TGridObject value)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            gridArray[x, y] = value;
            OnGridObjectChanged?.Invoke(this, new OnGridObjectChangedEventHandlerArgs { x = x, y = y });
        }
    }
    public void SetValue(Vector3 worldPosition, TGridObject value)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetValue(x, y, value);
    }
    public TGridObject GetGridObject(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
            return gridArray[x, y];
        else
            return default(TGridObject);
    }
    public TGridObject GetValue(Vector3 worldPosition)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return GetGridObject(x, y);
    }

    public void OnGridObjectModified(TGridObject GridObject)
    {
        if (showDebug)
        {
            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < gridArray.GetLength(1); y++)
                {
                    debugTextArray[x, y].text = gridArray[x, y].ToString();
                }
            }
        }
        
    }


}
