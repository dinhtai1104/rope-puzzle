using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WrappingRope
{
    public class CircleObject : MonoBehaviour
    {
        private CircleCollider2D circle;
        private float radius;
        public LayerMask ropeLayer;
        // Start is called before the first frame update
        void Start()
        {
            circle = GetComponent<CircleCollider2D>();
            radius = transform.localScale.x * circle.radius * transform.parent.localScale.x;
        }

        // Update is called once per frame
        void Update()
        {
            Collider2D col = Physics2D.OverlapCircle(transform.position, radius, ropeLayer);
            if (col != null)
            {
                Vector2 pointClosest = col.ClosestPoint(transform.position);

                Ray ray = new Ray(transform.position, pointClosest - (Vector2)transform.position);
                Vector2 point = ray.GetPoint(radius + 0.02f);
                RopeSystem.Instance.AddPoint(point);
            }
        }
    }
}