using UnityEngine;
using System.Collections;
using Cinemachine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : AbstractPlayerController {

    public CinemachineVirtualCamera vCam;
    [HideInInspector] private Transform vCamTransform;

    void Awake() {
        base.Init();
        //vCam = GameObject.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>();
        vCamTransform = vCam.GetComponent<Transform>();
    }

    public override IEnumerator TurnCharacter() {
        while (isMoving) {
            if (Input.GetKeyDown(KeyCode.Mouse0)) {
                Vector2 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
                if (pos.x > 0.5) {
                    TurnTo(TurnDirection.RIGHT);
                } else if (pos.x < 0.5) {
                    TurnTo(TurnDirection.LEFT);
                }
            }
            yield return null;
        }
    }

    public void TurnTo(TurnDirection direction) {
        int sign = direction == TurnDirection.RIGHT ? -1 : 1;
        if (isSpiningNow) {
            StopCoroutine("Turn");
            float currentAngel = vCamTransform.rotation.eulerAngles.z;
            float divisionRightAngle = Mathf.Abs(currentAngel % 90) * sign;
            float divisionLeftAngle = 90 - Mathf.Abs(currentAngel % 90) * sign;
            float angle = direction == TurnDirection.RIGHT ? divisionRightAngle : divisionLeftAngle;
            StartCoroutine(Turn(angle, -1));
        } else {
            StartCoroutine(Turn(sign * 90, sign));
        }

    }

    public override IEnumerator Move() {
        while (isMoving) {
            rb2d.MovePosition(rb2d.position + (Vector2) rb2dTransform.up * speed * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }
    }

    private bool isSpiningNow;
    public IEnumerator Turn(float angle, int sign) {
        isSpiningNow = true;
        float timeCount = 0.0f;
        Quaternion endRot = vCamTransform.rotation * Quaternion.Euler(0, 0, angle);
        Quaternion startVcamRot = vCamTransform.rotation;
        Quaternion start_rb2Rot = rb2dTransform.rotation;
        while (timeCount < 1f) {
            vCamTransform.rotation = Quaternion.Slerp(startVcamRot, endRot, timeCount);
            rb2dTransform.rotation = Quaternion.Slerp(start_rb2Rot, endRot, timeCount);
            timeCount += Time.deltaTime * speedAngle;
            yield return new WaitForFixedUpdate();
        }

        vCamTransform.rotation = endRot;
        rb2dTransform.rotation = endRot;
        isSpiningNow = false;
        yield return null;
    }

    public void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("EnemySpirit")) {
            GameController.instance.SetPlayerPosition();
        }
    }
    
    public void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("EnemySpirit")) {
            GameController.instance.SetPlayerPosition();
        }
    }

}

public enum TurnDirection {
    RIGHT,
    LEFT,
    UP,
    DOWN
}