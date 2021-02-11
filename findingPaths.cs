using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngineInternal;
using Vector3 = UnityEngine.Vector3;
using UnityEngine.Tilemaps;

public class findingPaths{
    private Grid<Pathnode> grid;
    private Vector3 originPos;
    private int cellsize;
    public static findingPaths Instance { get; private set; }

    private List<Pathnode> openList; // contains list of nodes that haven't been searched yet
    private List<Pathnode> closedList; // contains list of nodes that have been searched

    private const int MOVE_DIAG = 14; // one unit move diagonally multiplied by ten to get an integer value calculated with pythag
    private const int MOVE_STRAIGHT = 10;

    public findingPaths(int width, int height,int cellsize, bool showdebug, Vector3 originPos)
    {
        grid = new Grid<Pathnode>(width,height,cellsize,originPos, 
            (Grid<Pathnode> g, int x, int y) => new Pathnode(grid,x,y),showdebug);
        this.originPos = originPos;
        this.cellsize = cellsize;
        Instance = this;
    }

    public List<Pathnode> FindPath(int startx, int starty, int endx, int endy)
    {
        Pathnode startNode = grid.GetObject(startx, starty);
        Pathnode endNode = grid.GetObject(endx, endy);
        openList = new List<Pathnode> {startNode};   // contains the nodes that haven't been searched yet
        closedList = new List<Pathnode>();  // searched nodes
        
        for (int x = 0; x < grid.width; x++)
        {
            for (int y = 0; y < grid.height; y++)
            {
                Pathnode pathnode = grid.GetObject(x, y);
                pathnode.calculateWorldPos(cellsize,originPos);
                pathnode.GCost = int.MaxValue;  // gcost defaults to max so that below in neighbours loop a real cost will be found
                pathnode.calculateFcost();
                pathnode.cameFrom = null;
            }
        }

        // calculate costs for the start node
        startNode.GCost = 0;
        startNode.HCost = calculateDistance(startNode, endNode);
        startNode.calculateFcost();

        while (openList.Count > 0)
        {
            Debug.Log("The while loop has started");  // debug statement 
            Pathnode currentNode = GetLowestFcost(openList);
            Debug.Log($"{currentNode.x},{currentNode.y}");
            if (currentNode == endNode)
            {
                return calculateNodeList(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (Pathnode neighbourNode in findNeighbours(currentNode))
            {
                // print(findNeighbours(currentNode));
                if (closedList.Contains(neighbourNode)) continue;
                
                if (neighbourNode.walkable == false)
                {
                    closedList.Add(neighbourNode);
                    continue;
                }
                
                // Debug.Log($"Neightbour node: {neighbourNode.x},{neighbourNode.y}");
                int holderGcost = currentNode.GCost + calculateDistance(currentNode, neighbourNode);
                if (neighbourNode.GCost > holderGcost)
                {
                    // update values for costs in neighbour nodes
                    neighbourNode.cameFrom = currentNode; // used to calculate the final path 
                    neighbourNode.GCost = holderGcost;
                    neighbourNode.HCost = calculateDistance(neighbourNode, endNode);
                    neighbourNode.calculateFcost();
                    // Debug.Log($"{neighbourNode.x},{neighbourNode.y}, worldpos:{neighbourNode.worldPos}");
                    
                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                    
                }
            }
            // print(openList);
        }

        return null;
    }
    
    public List<Vector3> FindPath(Vector3 startPos, Vector3 endPos)
    {
        grid.getXY(startPos,out int startx, out int starty);
        grid.getXY(endPos,out int endx, out int endy);
        List<Pathnode> path = FindPath(startx, starty, endx, endy);
        if (path != null)
        {
            List<Vector3> vects = new List<Vector3>();
            foreach (Pathnode node in path)
            {
                // Vector3 noob = new Vector3(node.x, node.y) * grid.cellsize + Vector3.one * grid.cellsize * 0.5f;
                vects.Add(new Vector3(node.x,node.y)*grid.cellsize+Vector3.one*grid.cellsize * 0.5f + originPos);
            }
        
            return vects;
        }
        return null;
    }

    private List<Pathnode> findNeighbours(Pathnode currentNode)
    {
        List<Pathnode> neighbours = new List<Pathnode> { };
        //left
        if (currentNode.x - 1 >= 0)
        {
            neighbours.Add(grid.GetObject(currentNode.x-1,currentNode.y));
            if (currentNode.y-1 >= 0) neighbours.Add(grid.GetObject(currentNode.x-1,currentNode.y-1));
            if (currentNode.y+1 <= grid.height) neighbours.Add(grid.GetObject(currentNode.x-1,currentNode.y+1));
        }
        //right
        if (currentNode.x - 1 <= grid.width)
        {
            neighbours.Add(grid.GetObject(currentNode.x+1,currentNode.y));
            if (currentNode.y-1 >= 0) neighbours.Add(grid.GetObject(currentNode.x+1,currentNode.y-1));
            if (currentNode.y+1 <= grid.height) neighbours.Add(grid.GetObject(currentNode.x+1,currentNode.y+1));
        }
        //up
        if (currentNode.y + 1 <= grid.height)
        {
            neighbours.Add(grid.GetObject(currentNode.x,currentNode.y+1));
        }
        //down
        if (currentNode.y - 1 >= 0)
        {
            neighbours.Add(grid.GetObject(currentNode.x,currentNode.y-1));
        }

        return neighbours;
    }
    
    private List<Pathnode> calculateNodeList(Pathnode endNode)
    {
        List<Pathnode> path = new List<Pathnode> { };
        path.Add(endNode);
        Pathnode current = endNode;
        while (current.cameFrom != null)
        {
            path.Add(current.cameFrom);
            current = current.cameFrom;
        }
        path.Reverse(); // the path is now from the start node to the end node 
        return path;
    }
    public int calculateDistance(Pathnode a, Pathnode b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - a.y);
        int remainD = Mathf.Abs(yDistance - xDistance);
        return MOVE_DIAG * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT * remainD; // using pythag to calculate distance 
    }

    private Pathnode GetLowestFcost(List<Pathnode> pNodes)
    {
        // the node we want to search is the lowest fcost node:
        Pathnode Lowest = pNodes[0];
        for (int i = 0; i < pNodes.Count; i++)
        {
            if (pNodes[i].FCost < Lowest.FCost)
            {
                Lowest = pNodes[i];
            }
        }

        return Lowest;
    }

    public Grid<Pathnode> getGrid()
    {
        return this.grid;
    }

    public void print(List<Pathnode> path)
    {
        Debug.Log("this is the openlist contents");
        for (int i = 0; i < path.Count; i++)
        {
            Debug.Log($"Pathnode: {path[i].x},{path[i].y},walkable:{path[i].walkable}");
        }
    }

    public void setNotWalkable(List<Tilemap> map)
    {
        if (map != null)
        {
            foreach (Pathnode node in grid.gridArray)
            {
            
                Debug.Log($"node xy: {node.getX()},{node.getY()}");
                node.calculateWorldPos(cellsize,originPos);
                Debug.Log($"node worldPos: {node.worldPos}");
                int x = 0;
                while (x < map.Count)
                {
                    Vector3Int lol = map[x].WorldToCell(node.worldPos);
                    TileBase tileBelow = map[x].GetTile(lol);
                    if (tileBelow != null)
                    {
                        node.walkable = false;
                        break;
                    }

                    x++;
                }
            
                Debug.Log($"walkable: {node.walkable}");
            }
        }
        
        
    }
}



    
