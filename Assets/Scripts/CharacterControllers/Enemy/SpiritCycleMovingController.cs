using System.Collections;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class SpiritCycleMovingController : AbstractEnemyController {

    private List<TileM> island;

    void Awake() {
        base.Init();
        tPoint = GameObject.Find("TurnPoint");
        layer = ~LayerMask.NameToLayer("Wall");
    }

    protected override void SetPosition() {
        SetWay();
        if (island == null || island.Count==0) {
            isMoving = false;
            return;
        } 
        isMoving = true;
        TileM emptyTile = null;
        TileM focus = null;
       
        do {
            for (int i = 0; i < island[0].neighbors.Length; i++) {
                emptyTile = island[0].neighbors[i];
                focus = island[0];

                if (emptyTile != null && emptyTile.Type == TileTypeMaze.Ground) {
                    break;
                }
            }
            
        } while (emptyTile.Type != TileTypeMaze.Ground);
        rb2dTransform.position = emptyTile.Tile.transform.position;
        rb2dTransform.LookAt2D(rb2dTransform.up, focus.Tile.transform);
       
    }

    private void SetWay() {
        int rInd = Random.Range(0, RandomMazeGenerator.instance.maze.IslandTiles.Count);
        if (RandomMazeGenerator.instance.maze.IslandTiles.Count != 0) {
            island = RandomMazeGenerator.instance.maze.IslandTiles[rInd];
            RandomMazeGenerator.instance.maze.IslandTiles.RemoveAt(rInd);
        }
    }

    private bool CheckWall(Direction dir) {
        Vector2 d = dir == Direction.Right ? tPoint.transform.right : tPoint.transform.up;
        RaycastHit2D hit = Physics2D.Raycast(tPoint.transform.position, d, 8f);
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
            case Direction.Right: angle = -90;
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

}