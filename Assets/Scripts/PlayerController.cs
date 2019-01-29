using UnityEngine;
using System.Collections;
using Cinemachine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : AbstractPlayerController {
    public CinemachineVirtualCamera vCam;
    [HideInInspector] private Transform vCamTransform;
    void Awake() {
        base.Init();
        vCamTransform = vCam.GetComponent<Transform>();
    }

    public override void TurnCharacter() {
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            Vector2 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            if (pos.x > 0.5) {
                TurnTo(TurnDirection.LEFT);
            }
            else if (pos.x < 0.5) {
                TurnTo(TurnDirection.RIGHT);
            }
        }
    }

    public void TurnTo(TurnDirection direction) {
        switch (direction) {
            case TurnDirection.LEFT:
                StartCoroutine(Turn(-90, -1));
                break;
            case TurnDirection.RIGHT:
                StartCoroutine(Turn(90, 1));
                break;
        }
    }

    public override void MoveUp() {
        rb2d.MovePosition(rb2d.position + (Vector2) rb2dTransform.up * speed * Time.fixedDeltaTime);
    }

    public IEnumerator Turn(float angle, int sign) {
        float timeCount = 0.0f;
        Quaternion endRot = vCamTransform.rotation * Quaternion.Euler(0, 0, angle);
        Quaternion startVcamRot = vCamTransform.rotation;
        Quaternion start_rb2Rot = rb2dTransform.rotation;
        while (timeCount < 1f) {
            vCamTransform.rotation = Quaternion.Slerp(startVcamRot, endRot, timeCount);
            rb2dTransform.rotation = Quaternion.Slerp(start_rb2Rot, endRot, timeCount);
            timeCount += Time.deltaTime * speedAngle;
            yield return new WaitForEndOfFrame();
        }

        vCamTransform.rotation = endRot;
        rb2dTransform.rotation = endRot;
        yield return null;
    }
}

public enum TurnDirection {
    RIGHT,
    LEFT,
    UP,
    DOWN
}