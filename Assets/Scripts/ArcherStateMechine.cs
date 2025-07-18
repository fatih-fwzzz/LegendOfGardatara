using UnityEngine;
using System.Collections;


public class ArcherStateMechine : MonoBehaviour
{
    public ArcherState archerState;
    public float detectionRange = 6;
    public LayerMask enemyLayer;
    public DefenderMovement defenderMovement;
    public Bow bow;
    public SpriteRenderer bowSr;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
     archerState = ArcherState.Walking;   
     bow.enabled = false;
     bowSr.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
     if(archerState == ArcherState.Walking)
     {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, detectionRange, enemyLayer);
        if(hit.collider != null)
        {
            archerState = ArcherState.Shooting;
            bow.enabled = true;
            bowSr.enabled = true;
            defenderMovement.enabled = false;
        }
     }   
    }
}

public enum ArcherState
{
Idle,
Walking,
Shooting
};