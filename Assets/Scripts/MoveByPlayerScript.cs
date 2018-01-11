using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveByPlayerScript : MonoBehaviour {
    
    private Rigidbody rb;
    private float moveSpeed;
    private Vector3 temp;
    [HideInInspector]
    private bool canMove = true;
    private Vector3 start;
    private Vector3 end;

    [HideInInspector]
    public bool isMoving = false;
    [HideInInspector]
    public bool isMoved;
    public LayerMask BlockingLayer;
    public bool CanMove
    {
        get
        {
            return canMove;
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        moveSpeed = 1 / GameController.instance.boardManagerScript.playerMoveTime;
        temp = new Vector3(0f, 0.2f, 0f);
    }
    public bool AttemptMove(Vector3 Dir)
    {
        start = transform.position;
        end = start + Dir;
        canMove = !Physics.Linecast(start + temp, end + temp, BlockingLayer);
        if (canMove)
        {
            if (!isMoving)
            {
                StartCoroutine(SmoothMove(Dir));
            }
        }
        //else
        //    Debug.Log(canMove);

        return CanMove;
    }
    IEnumerator SmoothMove(Vector3 toward)
    {
        isMoving = true;
        isMoved = true;
        Vector3 targetPositon = transform.position + toward;
        float distanceRemain = (transform.position - targetPositon).sqrMagnitude;
        while (distanceRemain > float.Epsilon)
        {
            //Debug.Log("is moving to " + rb.transform.position + toward);
            Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPositon, Time.deltaTime * moveSpeed);
            rb.MovePosition(newPosition);
            distanceRemain = (newPosition - targetPositon).sqrMagnitude;
            yield return null;
        }
        isMoving = false;
        //rb.MovePosition(new Vector3((int)(transform.position.x + 0.1), transform.position.y, (int)(transform.position.z + 0.1)));
    }
}
