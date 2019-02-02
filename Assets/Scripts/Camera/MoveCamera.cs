using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour {

    public float speed = 4.0f;

    private Vector3 startPos;
    private bool isMoving;

    void FixedUpdate() {
        if (Input.GetMouseButtonDown(1)) {
            startPos = Input.mousePosition;
            isMoving = true;
        }

        if (Input.GetMouseButtonUp(1) && isMoving) {
            isMoving = false;
        }

        if (isMoving) {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - startPos);
            Vector3 move = new Vector3(pos.x*speed, pos.y * speed, 0);
            transform.Translate(move, Space.Self);
        }
    }
}
