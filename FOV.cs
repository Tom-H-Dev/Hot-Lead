using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOV : MonoBehaviour
{
    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;
    public float meshResolution;
    public float edgeDstThreshold;

    public int edgeResolveIterations;

    [HideInInspector] public bool playerSeen = false;

    public LayerMask targetMask;
    public LayerMask obstacleMask;
    public LayerMask playerMask;

    [HideInInspector] public List<Transform> visableTargets = new List<Transform>();


    public MeshFilter viewMeshFilter;
    Mesh viewMesh;

    private EnemyBehavior eB;

    [SerializeField] private GameObject player;


    private void Start()
    {
        StartCoroutine("FindTargetsWithDelay", 0.2f);

        viewMesh = new Mesh();
        viewMesh.name = "ViewMesh";
        viewMeshFilter.mesh = viewMesh;

        eB = GetComponent<EnemyBehavior>();
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);

            FindVisableTargets();
        }
    }

    void FindVisableTargets()
    {
        visableTargets.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, playerMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, directionToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, dstToTarget, obstacleMask))
                {
                    eB.AttackEnemy();
                    visableTargets.Add(target);
                }
            }
        }
    }

    

    private void LateUpdate()
    {
        //print(enemyState);
        DrawFieldOfView();
    }

    void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
        float stepAngleSize = viewAngle / stepCount;

        List<Vector3> viewPoints = new List<Vector3>();

        ViewCastInfo oldViewCast = new ViewCastInfo();

        for (int i = 0; i < stepCount; i++)
        {
            float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);

            if (i > 0)
            {
                bool edgeDstThresholdExeeded = Mathf.Abs(oldViewCast.dst - newViewCast.dst) > edgeDstThreshold;
                if (oldViewCast.hit != newViewCast.hit)
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if (edge.pointA != Vector3.zero || (oldViewCast.hit && newViewCast.hit && edgeDstThresholdExeeded))
                    {
                        viewPoints.Add(edge.pointA);
                    }
                    if (edge.pointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointB);
                    }
                }
            }

            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertacies = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertacies[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertacies[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        viewMesh.Clear();
        viewMesh.vertices = vertacies;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }

    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);

            bool edgeDstThresholdExeeded = Mathf.Abs(minViewCast.dst - maxViewCast.dst) > edgeDstThreshold;

            if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExeeded)
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }
        return new EdgeInfo(minPoint, maxPoint);

    }

    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirectionFromAngle(globalAngle, true);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, dir, out hit, viewRadius, obstacleMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
        }
    }

    public Vector3 DirectionFromAngle(float angleDegrees, bool angleIsBlobal)
    {
        if (!angleIsBlobal)
        {
            angleDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleDegrees * Mathf.Deg2Rad));
    }
    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float dst;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle)
        {
            hit = _hit;
            point = _point;
            dst = _dst;
            angle = _angle;
        }
    }
    public struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }
}
