using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using UnityEngine;
using CodeMonkey.Utils;
using JetBrains.Annotations;
using UnityEngine.Diagnostics;
using UnityEngine.Tilemaps;
using Vector3 = UnityEngine.Vector3;

public class Testing : MonoBehaviour
{
    public findingPaths pathfinding;
    [SerializeField] private bool showDebug;
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private int cellsize;
    [SerializeField] private Vector3 originPos;
    [SerializeField] [ItemCanBeNull] private List<Tilemap> map;
    public Transform Destination;
    public GameObject player;
    
    private List<Vector3> troll = null;
    private int CurrentPathIndex = 0;
    
    private void Start()
    {
        pathfinding =new findingPaths(width,height,cellsize,showDebug,originPos);
        pathfinding.setNotWalkable(map);
    }

    // private void Update()
    // {
    //     if (Input.GetMouseButtonDown(0))
    //     {
    //         Vector3 worldPos = UtilsClass.GetMouseWorldPosition();
    //         pathfinding.getGrid().getXY(worldPos, out int x, out int y);
    //         // Debug.Log($"world pos:{worldPos.x},{worldPos.y}");
    //         // Debug.Log($"lol{x},{y}");
    //         Pathnode node = pathfinding.getGrid().GetObject(new Vector3(worldPos.x, worldPos.y ));
    //         Debug.Log(node.walkable);
    //         List<Pathnode> path = pathfinding.FindPath(4, 12, x, y);
    //         foreach (Pathnode pn in path)
    //         {
    //             Debug.Log($"Node:{pn.x},{pn.y}");
    //         }
    //     }
    //
    //     if (Input.GetMouseButtonDown(1))
    //     {
    //         Vector3 worldPos = UtilsClass.GetMouseWorldPosition();
    //         Vector3 start = pathfinding.getGrid().GetWorldPos(4, 12);
    //         List<Vector3> path = pathfinding.FindPath(start, worldPos);
    //         foreach (Vector3 vec in path)
    //         {
    //             Debug.Log($"move {vec}");
    //         }
    //         pathfinding.getGrid().getXY(path[path.Count-1],out int x, out int y);
    //         Debug.Log($"last node:{x},{y}");
    //     }
    // }

    // void chase()
    // {
    //     Vector3 destination = Destination.position;
    //     if (troll == null)
    //     {
    //         troll = findingPaths.Instance.FindPath(player.transform.position, destination);
    //     }
    //     
    //     if (troll != null)
    //     {
    //         if (Vector3.Distance(player.transform.position, troll[CurrentPathIndex]) > 0.2f)
    //         {
    //             player.transform.position = Vector3.MoveTowards(player.transform.position, troll[CurrentPathIndex], 1 * Time.deltaTime);
    //         }
    //         else
    //         {
    //             CurrentPathIndex++;
    //             if (CurrentPathIndex > troll.Count)
    //             {
    //                 troll = null;
    //             }
    //         }
    //     }
    // }
}
