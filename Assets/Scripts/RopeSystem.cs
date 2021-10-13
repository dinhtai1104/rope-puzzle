using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
namespace WrappingRope
{
    public class RopeSystem : Singleton<RopeSystem>
    {
        
        public static float widthSizeLine = 0.12f;


        [Header("End TEST")]
        [Space(3)]
        public LineRenderer lineRenderer;
        public Transform endPoint;
        public Transform startPos;
        private List<Vector2> pointLine = new List<Vector2>();
        private List<int> pointShape = new List<int>();
        private int pointCount = 0;

        public class pair
        {
            public Vector2 hinge;
            public int isClock;
        }

        private Renderer rend;
        public LayerMask ObjectMask;

        private bool wrapping = false;
        private List<int> wrappedLookup = new List<int>();
        private Gradient color = null;

        private EdgeCollider2D edge;
        private Vector2[] edgePoints = new Vector2[2];
        void Awake()
        {

            
        }

        public void AddPoint(Vector2 point)
        {
            if (!CheckDirectionOfCircle(point)) return;
            if (Vector2.Distance(point, pointLine[pointCount - 2]) > 0.01f)
            {
                //pointCount++;
                pointLine.RemoveAt(pointCount - 1);
                pointLine.Add(point);
                pointLine.Add(endPoint.position);
                pointCount++;
                wrappedLookup.Add(0);
                pointShape.Add(1);
                // UnRopeWrapping();
            }
        }


        private bool CheckDirectionOfCircle(Vector2 point)
        {
            var RinkToPoint = (Vector2)endPoint.position - point;
            var PointToLine = point - pointLine[pointCount - 2];
            if (Vector2.Angle(RinkToPoint, PointToLine) < 90)
            {
                return true;
            }
            return false;
        }
        // Start is called before the first frame update
        void Start()
        {
            rend = GetComponent<Renderer>();
            edge = gameObject.GetComponent<EdgeCollider2D>();
            edge.edgeRadius = 0;

            color = lineRenderer.colorGradient;
            pointCount = 2;
            pointLine.Add(startPos.position);
            lineRenderer.positionCount = pointCount;
            lineRenderer.SetPositions(MyVector3Extension.toVector2Array(pointLine));
            Vector2 pos = endPoint.position;
            pointLine.Add(pos);
            lineRenderer.SetPosition(pointCount - 1, pos);
            lineRenderer.startWidth = lineRenderer.endWidth = widthSizeLine;

        }
        private bool isDrag = false;

        int x = 0;
        // Update is called once per frame
        void FixedUpdate()
        {
            RopeWrapping();
        }

        void Update()
        {
            UnRopeWrapping();
        }

        public int isLeft(Vector2 a, Vector2 b, Vector2 c)
        {
            float x = (b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x);
            return x > 0 ? 1 : -1;
        }

        private string Vector2ToString(Vector2 point, int value)
        {
            return "[(" + point.x + "-" + point.y + ")-" + value + "]";
        }
        private void LateUpdate()
        {
            Vector2 pos = endPoint.position;
            lineRenderer.positionCount = pointCount;
            pointLine[pointCount - 1] = pos;
            lineRenderer.SetPositions(MyVector3Extension.toVector2Array(pointLine));
            edgePoints[0] = pointLine[pointCount - 2];
            edgePoints[1] = pos;
            edge.points = edgePoints;
        }
        private bool isFirst = false;

        public List<Vector2> getPath()
        {
            return pointLine;
        }

        public void RopeWrapping()
        {
            Vector2 originPos = pointLine[pointCount - 2];
            Vector2 direction = -(Vector2)endPoint.position + originPos;
            float distance = Vector2.Distance(endPoint.position, originPos);
            RaycastHit2D hit = Physics2D.Raycast(endPoint.position, direction.normalized, distance - 0.1f, ObjectMask);
            if (hit)
            {

                PolygonCollider2D collider = hit.collider as PolygonCollider2D;
                if (collider != null)
                {
                    wrapping = true;
                    if (!isFirst) isFirst = true;
                    Vector2 hitPoint = GetClosestColliderPointFromRaycastHit(hit, collider);
                    if (Vector2.Distance(hitPoint, pointLine[pointCount - 2]) < 0.05f) return;


                    wrappedLookup.Add(0);
                    if (collider.points.Length > 30)
                    {
                        pointShape.Add(1);
                    }
                    else
                    {
                        pointShape.Add(0);
                    }
                    pointLine.RemoveAt(pointLine.Count - 1);

                    pointLine.Add(hitPoint);
                    pointCount++;
                    pointLine.Add(endPoint.position);
                }
            }
            else
            {
            }
        }

        public void ResetColor()
        {
            if (lineRenderer != null && color != null)
                lineRenderer.colorGradient = color;
        }

        private void UnRopeWrapping()
        {

            if (pointCount < 3) return;

            // 1
            var anchorIndex = pointLine.Count - 3;
            // 2
            var hingeIndex = pointLine.Count - 2;
            // 3
            var anchorPosition = pointLine[anchorIndex];
            // 4
            var hingePosition = pointLine[hingeIndex];
            // 5
            var hingeDir = hingePosition - anchorPosition;
            // 6
            var hingeAngle = Vector2.Angle(anchorPosition, hingeDir);
            // 7
            var playerDir = (Vector2)endPoint.position - anchorPosition;
            // 8
            var playerAngle = Vector2.Angle(anchorPosition, playerDir);


            Vector2 dirPlayerHinge = (Vector2)endPoint.position - hingePosition;
            Vector2 dirAnchorHinge = anchorPosition - hingePosition;



            int hingeWrap = wrappedLookup.Count - 1;


            if (isLeft(anchorPosition, hingePosition, endPoint.position) == -1)
            {
                // 1
                if (wrappedLookup[hingeWrap] == 1)
                {
                    UnWrappingPosition(anchorIndex, hingeIndex);
                    return;
                }
                // 2
                if (wrappedLookup[hingeWrap] == 0)
                    wrappedLookup[hingeWrap] = -1;
            }
            else if (isLeft(anchorPosition, hingePosition, endPoint.position) == 1)
            {
                // 3
                if (wrappedLookup[hingeWrap] == -1)
                {
                    UnWrappingPosition(anchorIndex, hingeIndex);
                    return;
                }
                // 2
                if (wrappedLookup[hingeWrap] == 0)
                    wrappedLookup[hingeWrap] = 1;

            }


        }


        private float AngleBetweenVector2(Vector2 vec1, Vector2 vec2)
        {
            Vector2 diference = vec2 - vec1;
            float sign = (vec2.y < vec1.y) ? -1.0f : 1.0f;
            return Vector2.Angle(Vector2.right, diference) * sign;
        }

        private bool Unwrap2()
        {
            int anchorIndex = pointCount - 3;
            int hingeIndex = pointCount - 2;
            int playerIndex = pointCount - 1;

            Vector2 anchorPosition = pointLine[anchorIndex];
            Vector2 hingePosition = pointLine[hingeIndex];
            Vector2 playerPosition = pointLine[playerIndex];
            Vector2 trungDiem = (anchorPosition + playerPosition) * 0.5f;

            Vector2 directionAnchor = -playerPosition + anchorPosition;
            Vector2 directionHinge = -playerPosition + hingePosition;
            Vector2 directionTD = hingePosition - trungDiem;

            Vector2 playerDir = playerPosition - anchorPosition;


            float distanceAnchor = Vector2.Distance(playerPosition, anchorPosition);
            float distanceHinge = Vector2.Distance(playerPosition, hingePosition);
            float distanceTD = Vector2.Distance(hingePosition, trungDiem);


            Vector2 TDPlay_Anchor = (playerPosition + anchorPosition) / 2;
            Vector2 TDPlay_Hinge = (playerPosition + hingePosition) / 2;

            Vector2 TDPlay_AnchorDir = (-hingePosition + TDPlay_Anchor).normalized;
            Vector2 TDPlay_HingeDir = (-anchorPosition + TDPlay_Hinge).normalized;
            float distance1 = Vector2.Distance(hingePosition, TDPlay_Anchor) - 0.1f;
            float distance2 = Vector2.Distance(anchorPosition, TDPlay_Hinge) - 0.1f;
            if (wrapping && !Physics2D.Raycast(playerPosition, directionAnchor.normalized, distanceAnchor - 0.1f, ObjectMask)
                //!Physics2D.Raycast(playerPosition, directionHinge.normalized, distanceHinge - 0.1f, ObjectMask)&&
                /*&& !Physics2D.Raycast(trungDiem, directionTD.normalized, distanceTD - 0.01f, ObjectMask)*/)

            {
                UnWrappingPosition(anchorIndex, hingeIndex);
                return true;
            }

            return false;


        }

        private void UnWrappingPosition(int anchorIndex, int hingeIndex)
        {
            pointLine.RemoveAt(hingeIndex);
            pointShape.RemoveAt(pointShape.Count - 1);

            wrappedLookup.RemoveAt(wrappedLookup.Count - 1);
            pointCount--;
            if (pointCount < 3) wrapping = false;
        }

        private Vector2 GetClosestColliderPointFromRaycastHit(RaycastHit2D hit, PolygonCollider2D polyCollider)
        {
            // Transform polygoncolliderpoints to world space (default is local)
            var distanceDictionary = polyCollider.points.ToDictionary<Vector2, float, Vector2>(
                position => Vector2.Distance(hit.point, polyCollider.transform.TransformPoint(position)),
                position => polyCollider.transform.TransformPoint(position));

            var orderedDictionary = distanceDictionary.OrderBy(e => e.Key);

            return orderedDictionary.Any() ? orderedDictionary.First().Value : Vector2.zero;
        }
        private Vector2 getVertexPositionNearest(Vector3 pointHit, PolygonCollider2D collider)
        {
            Vector2 min = pointHit;
            float _min = 999999;
            return min;
        }

    }

    public static class MyVector3Extension
    {
        public static Vector3[] toVector2Array(List<Vector2> v2)
        {
            return System.Array.ConvertAll<Vector2, Vector3>(v2.ToArray(), getV2fromV3);
        }

        public static Vector3 getV2fromV3(Vector2 v3)
        {
            return new Vector3(v3.x, v3.y, 0);
        }
    }
}