using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingMovement : MonoBehaviour   
{
    [SerializeField] Rigidbody2D myRb;
    [SerializeField] LayerMask targetMask;
    [SerializeField] LayerMask ringMask;
    [SerializeField] float radius;
    [SerializeField] float speed;
    public Transform TrucXoay;
    private Camera cam;
    private bool isDrag = false;
    private bool isTouchToTarget = false;
    private bool isLastTouch = false;
    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if (( (GameController.lastSwing != null && !GameController.lastSwing.IsFinish())) && IsConnected())
        {
            return;
        }

        Vector2 mPos = cam.ScreenToWorldPoint(Input.mousePosition);

        Collider2D hitRing = Physics2D.OverlapCircle(transform.position, radius, targetMask);

        if (!isLastTouch && hitRing)
        {
            isTouchToTarget = true;
            if (!isLastTouch)
            {
                if (!isLastTouch)
                {
                    isLastTouch = true;
                    isDrag = false;
                    myRb.velocity = Vector3.zero;
                    transform.position = new Vector3(hitRing.transform.position.x, hitRing.transform.position.y, -0.04f);
                }
            }
        }
        else
        {
            if (!hitRing)
            {
                if (isTouchToTarget)
                {
                }
                isLastTouch = false;
                isTouchToTarget = false;
            }
        }



        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hitRingRC = Physics2D.Raycast(mPos, Vector3.zero, Mathf.Infinity, ringMask);
            if (hitRingRC.transform != null)
            {
                if (GameController.lastSwing == null ||( GameController.lastSwing.IsFinish() && GameController.lastSwing))
                {
                    isDrag = true;
                }
            }
        }


        


        // Dragging
        if (isDrag)
        {
            Vector2 direction = (Vector2)mPos - (Vector2)transform.position;
            myRb.velocity = direction * speed;

            if (direction.magnitude != 0)
            {
                TrucXoay.Rotate(0, 0, -10);
            }
        }
        

        if (Input.GetMouseButtonUp(0))
        {
            isDrag = false;
            myRb.velocity = Vector3.zero;
        }
    }

    public bool IsConnected()
    {
        return isTouchToTarget;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
#endif
}
