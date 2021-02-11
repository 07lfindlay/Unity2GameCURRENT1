using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;

public class Pathnode
{
    private Grid<Pathnode> grid;
    public int x;
    public int y;
    public Vector3 worldPos;

    public int GCost; // cost to get from one cell to another
    public int FCost; // Heuristic + Gcost
    public int HCost; // Heuristic cost to reach the endpoint

    public Pathnode cameFrom;
    public bool walkable; // is the grid spot walkable or not
    
    
    public Pathnode(Grid<Pathnode> grid ,int x, int y) 
    {
        this.x = x;
        this.y = y;
        // Vector3 worldpos = new Vector3(x - 25,y-24); // minus the origin pos to get the cell beneath
        // Vector3Int gridPosition = map.WorldToCell(worldpos);
        // TileBase clickedTile = map.GetTile(gridPosition);
        // if (tileData.tiles.Contains(clickedTile))
        // {
        //     walkable = tileData.walkable;
        // }
        // else
        // {
        //     walkable = true;
        // }
        walkable = true;
        this.grid = grid;
    }

    public void calculateFcost()
    {
        FCost = GCost + HCost;
    }

    public override string ToString()
    {
        return x + "," + y;
    }

    public void calculateWorldPos(int cellsize,Vector3 originPos)
    {
        worldPos = new Vector3(x,y) * cellsize + originPos;
    }

    public int getX()
    {
        return x;
    }

    public int getY()
    {
        return y;
    }
    
}

