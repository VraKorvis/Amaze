using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpiritBackForthMovingController : AbstractEnemyController {
    public int distance = 5;
    private TileM[] wayPoint;

    protected override void SetPosition() {
        var tiles = RandomMazeGenerator.instance.maze.GroundTiles;
        TileM focus = tiles[Random.Range(0, tiles.Length)];
        wayPoint[0] = focus;
        rb2dTransform.position = focus.Tile.transform.position;
    }

    public void Awake() {
        base.Init();
        tPoint = GameObject.Find("TurnPoint");
        layer = ~LayerMask.NameToLayer("Wall");
        wayPoint = new TileM[distance];
    }

    private bool CheckWall(Direction dir) {
        Vector2 d = dir == Direction.Right ? tPoint.transform.right : tPoint.transform.up;
        RaycastHit2D hit = Physics2D.Raycast(tPoint.transform.position, d, 10f);
        if (hit.collider != null) {
            Debug.DrawLine(tPoint.transform.position, hit.collider.transform.position, Color.red);
            return true;
        }

        return false;
    }

    public override IEnumerator TurnCharacter() {
        isMoving = true;
        while (isMoving) {
            if (!spiningNow) {
                if (!CheckWall(Direction.Right)) {
                    spiningNow = true;
                    StartCoroutine(TurnTo(Direction.Right));
                } else if (CheckWall(Direction.Top)) {
                    spiningNow = true;
                    StartCoroutine(TurnTo(Direction.Left));
                }
            }

            yield return new WaitForFixedUpdate();
        }

        yield return null;
    }

    public IEnumerator TurnTo(Direction dir) {
        float angle = 0;
        switch (dir) {
            case Direction.Right:
                angle = -90;
                break;
            case Direction.Left:
                angle = 90;
                break;
        }

        float timeCount = 0.0f;
        Quaternion endRot = rb2dTransform.rotation * Quaternion.Euler(0, 0, angle);
        Quaternion start_rb2Rot = rb2dTransform.rotation;
        while (timeCount < 1f) {
            rb2dTransform.rotation = Quaternion.Slerp(start_rb2Rot, endRot, timeCount);
            timeCount += Time.deltaTime * speedAngle;
            yield return new WaitForFixedUpdate();
        }

        rb2dTransform.rotation = endRot;
        spiningNow = false;
        yield return null;
    }

    public override IEnumerator MoveUp() {
        SetPosition();
        int index = 0;
        while (index < distance-1) {
            TileM focus = wayPoint[index];
            for (int i = focus.neighbors.Length - 1; i >= 0; i--) {
                TileM neighbor = focus.neighbors[i];
                if (neighbor.Type == TileTypeMaze.Ground) {
                    rb2dTransform.LookAt2D(rb2dTransform.up, neighbor.Tile.transform);
                    rb2d.MovePosition(rb2d.position + (Vector2) rb2dTransform.up * speed * Time.fixedDeltaTime);
                    index++;
                    wayPoint[index] = neighbor;
                    break;
                }

                yield return new WaitForFixedUpdate();
            }
        }

        index = wayPoint.Length - 1;
        bool pingpongswitch = true;
        while (isMoving) {
            TileM focus = wayPoint[index];
            rb2dTransform.LookAt2D(rb2dTransform.up, focus.Tile.transform);
            rb2d.MovePosition(rb2d.position + (Vector2) rb2dTransform.up * speed * Time.fixedDeltaTime);
            
            if (pingpongswitch) {
                index--;
                if (index == 0) pingpongswitch = false;
            } else {
                index++;
                if (index == wayPoint.Length - 1) pingpongswitch = true;
            }
            yield return new WaitForFixedUpdate();
        }

        yield return null;
    }
}