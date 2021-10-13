using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DIR { LEFT, RIGHT }
public class SawMovement : MonoBehaviour
{
    public SpriteRenderer sr;
    public Transform objectMove;
    public float speed;
    public DIR dir;

    private float boundLeft = 0;
    private float boundRight = 0;
    private float current = 0;
    private void Start()
    {
        boundLeft = -sr.bounds.size.x / 2f;
        boundRight = sr.bounds.size.x / 2f;
        if (dir == DIR.LEFT)
        {
            current = boundLeft;
        } else
        {
            current = boundRight;
        }
    }
    private void Update()
    {
        objectMove.localPosition = Vector2.MoveTowards(objectMove.localPosition, new Vector2(current, 0), speed * Time.deltaTime);
        if (Vector2.Distance(objectMove.localPosition, new Vector2(current, 0)) < 0.05f)
        {
            current *= -1;
        }
    }
}
