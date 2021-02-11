using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using Unity.Mathematics;
using UnityEngine.Diagnostics;
using UnityEngine.Tilemaps;

public class Grid<TGridobject>  
{
    public int width;
    private Vector3 originPos;
    public int height;
    public float cellsize;
    public TGridobject[,] gridArray;
    private bool showDebug;
    
    // This is a generic grid class 
    public Grid(int width, int height, float cellsize, Vector3 originPos, Func< Grid<TGridobject>, int, int, TGridobject> createObject, bool showD )
    {
        // The func<tGridobject> enables us to create grids with any object inside - path nodes for the pathfinding
        
        this.width = width;
        this.height = height;
        this.cellsize = cellsize;
        this.originPos = originPos;

        gridArray = new TGridobject[width, height]; // The array containing the objects in the grid 
        setDebug(showD);
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                gridArray[x, y] = createObject(this,x,y);
            }
        }


        if (showDebug)
        {
            TextMesh[,] debugTextArray = new TextMesh[width,height];
            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < gridArray.GetLength(1); y++)
                {
                    debugTextArray[x, y] = UtilsClass.CreateWorldText(gridArray[x, y].ToString(), null,
                        GetWorldPos(x, y) + new Vector3(cellsize, cellsize) * 0.5f, 5, Color.white,
                        TextAnchor.MiddleCenter);
                    Debug.DrawLine(GetWorldPos(x, y), GetWorldPos(x, y + 1), Color.green, 100f);
                    Debug.DrawLine(GetWorldPos(x, y), GetWorldPos(x + 1, y), Color.green, 100f);
                }

                Debug.DrawLine(GetWorldPos(0, height), GetWorldPos(width, height), Color.green, 100f);
                Debug.DrawLine(GetWorldPos(width, 0), GetWorldPos(width, height), Color.green, 100f);
            }
        }
        
    }

    public Vector3 GetWorldPos(int x, int y)
    {
        return new Vector3(x, y) * cellsize + originPos;
    }

    public void getXY(Vector3 Worldpos,out int x, out int y)
    {
        x = Mathf.FloorToInt((Worldpos- originPos).x / cellsize );
        y = Mathf.FloorToInt((Worldpos- originPos).y / cellsize );
    }

    public void SetValue(int x, int y, TGridobject value)
    {
        if (x >= 0 && y >=0 && x < width && y < height)
        {
            gridArray[x, y] = value;
        }
    }

    public void SetValue(Vector3 Worldpos, TGridobject value)
    {
        int x, y;
        getXY(Worldpos,out x,out y);
        SetValue(x,y,value);
    }

    public TGridobject GetObject(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            return gridArray[x, y];
        }
        else
        {
            return default(TGridobject);
        }
    }

    public TGridobject GetObject(Vector3 WorldPos)
    {
        int x, y;
        getXY(WorldPos,out x, out y);
        return GetObject(x, y);
    }

    public void setDebug(bool show)
    {
        if (!show)
            showDebug = false;
        else
        {
            showDebug = true;
        }
    }
}

