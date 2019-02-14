using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyBezieMove : AbstractEnemyController {

    private Vector3[] ways;
    public float moveTime = 0.1f;
    public float reachDist = 1.0f;

    private readonly List<Vector3> bezierPath = new List<Vector3>();

    public void Awake() {
        base.Init();
    }

    public void Start() {
        ways = new Vector3[WayPointController.instance.waySimple[0].positionCount];
        WayPointController.instance.waySimple[0].GetPositions(ways);
        inverseMoveTime = 1f / moveTime;
        SetPosition();
        StartCoroutine(PathBezierCurve(10));
    }

    public override IEnumerator TurnCharacter() {
        yield return null;
    }

    protected override void SetPosition() {
        rb2dTransform.position = ways[0];
    }

    public IEnumerator PathBezierCurve(int count_points) {
        bezierPath.Clear();
        var positions = ways;
        var size = positions.Length;
        for (int i = 0; i < size - 3; i += 3) {
            Vector3 p0 = positions[i];
            Vector3 p1 = positions[i + 1];
            Vector3 p2 = positions[i + 2];
            Vector3 p3 = positions[i + 3];
            if (i == 0) {
                bezierPath.Add(BezierCurve.CubicBezier(0, p0, p1, p2, p3));
            }
            for (int J = 1; J <= count_points; J++) {
                float t = J / (float)count_points;
                bezierPath.Add(BezierCurve.CubicBezier(t, p0, p1, p2, p3));
                yield return null;
            }
        }
        StartCoroutine(MoveToPoint());
    }

    private IEnumerator MoveToPoint() {
#if UNITY_EDITOR
        DrawBezier();
#endif
        while (isMoving) {
            int count = bezierPath.Count;
            for (int i = 0; i < count - 1; i++) {
                Vector3 end_pos = bezierPath[i];
                while (true) {
                    yield return new WaitForFixedUpdate();

                    float distance = Vector3.Distance(end_pos, rb2dTransform.position);
                    rb2dTransform.position = Vector3.MoveTowards(rb2dTransform.position, end_pos, speed * Time.deltaTime);
                    Vector3 dest = (end_pos - rb2dTransform.position).normalized;
                    float angle = VectorUtil.GetAngle(Vector3.down, dest) * Mathf.Rad2Deg;
                    rb2dTransform.rotation = Quaternion.Slerp(rb2dTransform.rotation, Quaternion.Euler(0, 0, angle),
                        speedAngle * Time.deltaTime);
                    if (distance <= reachDist) {
                        break;
                    }
                }
            }
        }
        

    }
#if UNITY_EDITOR
    private void DrawBezier() {
        //  if (bezierPath.Count==0) return;
        Vector3 firstPoint = bezierPath[0];
        bezierPath.ForEach(p => {
            Debug.DrawLine(firstPoint, p, Color.green, 100);
            firstPoint = p;
        });
    }
#endif


}
