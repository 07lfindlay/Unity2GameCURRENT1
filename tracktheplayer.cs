using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class tracktheplayer : MonoBehaviour
{
    private Seeker seeker;
    private Rigidbody2D rb;

    //pathfinding
    public bool reachedEndPath = false;
    private Path aipathToplayer;
    private Path aiPatrolpath;
    private int currentWaypoint = 1;
    public Transform player;
    public float moveSpeed = 10f;
    public float nextWayPointD;
    private Path aipath;
    
    //patrolling
    public List<Transform> PatrolPath;
    public int currentPoint=1;
    public float roundingDistance = 0.2f;
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        
        // InvokeRepeating("calculatePlayerTrackPath",0f,1f);
    }

    void FixedUpdate()
    {
        track();
    }
    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            aipath = p;
        }
    }
    void OnPathCompletePatrol(Path p)
    {
        if (!p.error)
        {
            aiPatrolpath = p;
        }
    }
    void OnPathCompletePlayer(Path p)
    {
        if (!p.error)
        {
            aipathToplayer = p;
        }
    }

    void calculatePlayerTrackPath()
    {
        if (seeker.IsDone())
            seeker.StartPath(transform.position, player.position, OnPathCompletePlayer);
    }

    void track()
    {
        seeker.StartPath(transform.position, player.position, OnPathComplete);
        if (aipath == null) return;
        if (currentWaypoint >= aipath.vectorPath.Count)
        {
            reachedEndPath = true;
            return;
        }
        
        Vector2 dir = ((Vector2) aipath.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 moveAmount = dir * moveSpeed * Time.deltaTime;
        Debug.Log(moveAmount);
        // Debug.Log(aipath.vectorPath[currentWaypoint]);
        rb.AddForce(moveAmount);
        
        float distance = Vector2.Distance(rb.position, aipath.vectorPath[currentWaypoint]);
        
        if (distance <= nextWayPointD)
        {
            currentWaypoint++;
        }
    }
    void trackPlayer()
    {
        // seeker.StartPath(transform.position, player.position, OnPathCompletePlayer);
        if (aipathToplayer == null) return;
        if (currentWaypoint >= aipathToplayer.vectorPath.Count)
        {
            reachedEndPath = true;
            return;
        }
        
        Vector2 dir = ((Vector2)aipathToplayer.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 moveAmount = dir * moveSpeed * Time.deltaTime;
        Debug.Log($"Move ammount {moveAmount}");
        Debug.Log($"position of the robot: {rb.position}");
        Debug.Log($"direction:{dir}");
        Debug.Log(aipathToplayer.vectorPath[currentWaypoint]);
        rb.AddForce(moveAmount);
        
        float distance = Vector2.Distance(rb.position, aipathToplayer.vectorPath[currentWaypoint]);
        
        if (distance <= nextWayPointD)
        {
            currentWaypoint++;
        }
    }
    void patrol()
    {
        seeker.StartPath(transform.position, PatrolPath[currentPoint].position, OnPathCompletePatrol);
        if (aiPatrolpath == null) return;
        if (currentWaypoint >= aiPatrolpath.vectorPath.Count)
        {
            reachedEndPath = true;
            return;
        }

        int speed = 10;
        Vector2 dir = ((Vector2) aiPatrolpath.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 moveAmount = dir * speed * Time.deltaTime;
        
        // Debug.Log(aipath.vectorPath[currentWaypoint]);
        rb.AddForce(moveAmount);
        
        float distance = Vector2.Distance(rb.position, aiPatrolpath.vectorPath[currentWaypoint]);
        
        if (distance <= nextWayPointD)
        {
            currentWaypoint++;
        }
    }
    
    private void changeGoal()
    {
        if (currentPoint == PatrolPath.Count - 1) 
        {
            // if the robot is at the endpoint of the patrol path: the last transform
            currentPoint = 0;
        }
        else
        {
            currentPoint = 1;
        }
    }
    
    
}
