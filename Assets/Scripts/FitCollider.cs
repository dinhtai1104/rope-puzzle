using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FitCollider : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var z = transform.localEulerAngles.z;
        PolygonCollider2D pol = GetComponent<PolygonCollider2D>();
        var sr = GetComponent<SpriteRenderer>();
        var scaleX = sr.size.x;
        var scaleY = sr.size.y;
        Vector2[] list = pol.points;
        for (int i = 0; i < list.Length; i++)
        {
            list[i] = new Vector2((list[i].x/Mathf.Abs(list[i].x)) * 0.5f * scaleX, (list[i].y / Mathf.Abs(list[i].y)) * 0.5f * scaleY); 
        }
        pol.points = list;
        transform.localEulerAngles = Vector3.forward * z;

    }
    private void OnValidate()
    {
        Start();
    }
}
