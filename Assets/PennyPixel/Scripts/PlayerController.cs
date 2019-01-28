using UnityEngine;
using System.Collections;
using Cinemachine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour {
    private Rigidbody2D rb2d;
    private Transform rb2dTransform;
    private Transform _transform;
    public static PlayerController _instance;
    public GameObject _mazeField;
    [Range(0f, 10f)] public float speed;
    [Range(0, 10f)] public float speedAngle;

    private Vector2 direction;

    public CinemachineVirtualCamera vCam;
    private Transform vCamTransform;

    void Awake() {
        _instance = this;
        rb2d = GetComponent<Rigidbody2D>();
        rb2dTransform = rb2d.transform;
        _transform = transform;
        direction =  _transform.up;
        vCamTransform = vCam.GetComponent<Transform>();
    }

    void Update() {
        MovePlayer();
    }

    void FixedUpdate() {
        rb2d.MovePosition(rb2d.position + (Vector2)rb2dTransform.up * speed * Time.fixedDeltaTime);
    }

    private void Move() {
        Vector2 dest = (Vector2) _transform.position + direction;
        Vector2 p = Vector2.MoveTowards(transform.position, dest, Time.deltaTime * speed);
        if (Input.GetKey(KeyCode.RightArrow)) {
            dest = (Vector2) _transform.position + Vector2.right;
        }
        else if (Input.GetKey(KeyCode.LeftArrow)) {
            dest = (Vector2) _transform.position + Vector2.left;
        }
        else if (Input.GetKey(KeyCode.DownArrow)) {
            dest = (Vector2) _transform.position + Vector2.down;
        }
        else if (Input.GetKey(KeyCode.UpArrow)) {
            dest = (Vector2) _transform.position + Vector2.up;
        }
        else {
            return;
        }
        rb2d.MovePosition(p);
    }

    public void MovePlayer() {
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            Vector2 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            if (pos.x > 0.5) {
                TurnTo(Turn.Left);
            }else if (pos.x < 0.5) {
                TurnTo(Turn.Right);
            }
        }
    }

    public void TurnTo(Turn turn) {
       // if (_spiningNow) return;
       // StopAllCoroutines();

        switch (turn) {
            case Turn.Left:
                // StartCoroutine(SmoothRotateFieldAroundPlayer(90, 1));
                StartCoroutine(TurnCamera(-90, -1));
                StartCoroutine(TurnPlayer(-90, -1));
                break;
            case Turn.Right:
                // StartCoroutine(SmoothRotateFieldAroundPlayer(-90, -1));
                StartCoroutine(TurnCamera(90, 1));
                StartCoroutine(TurnPlayer(90, 1));
                break;
        }
    }

    private IEnumerator TurnCamera(float angle, int sign) {
        float timeCount = 0.0f;
        _spiningNow = true;
        Quaternion endROt = vCamTransform.rotation * Quaternion.Euler(0, 0, angle );
        Quaternion startRot = vCamTransform.rotation;
        while (timeCount < 1f) {
            vCamTransform.rotation = Quaternion.Slerp(startRot, endROt, timeCount);

            timeCount += Time.deltaTime * speedAngle;
            yield return new WaitForEndOfFrame();
        }

        vCamTransform.rotation = endROt;
        _spiningNow = false;
        yield return null;
    }
    public IEnumerator TurnPlayer(float angle, int sign) {
        float timeCount = 0.0f;
        _spiningNow = true;
        Quaternion endRot = rb2d.transform.rotation * Quaternion.Euler(0, 0, angle);
        Quaternion startRot = rb2dTransform.rotation;
        while (timeCount < 1f) {
            rb2d.transform.rotation =
                Quaternion.Slerp(startRot, endRot, timeCount);
            timeCount += Time.deltaTime*speedAngle;
            yield return new WaitForEndOfFrame();
        }

        rb2d.transform.rotation = endRot;
        _spiningNow = false;
        yield return null;
    }

    private IEnumerator SmoothRotateFieldAroundPlayer(float  angle, int sign) {
        _spiningNow = true;
        Quaternion qTO = _mazeField.transform.rotation * Quaternion.Euler(0, 0, angle);
        float totalAngle=0;
        while (sign*totalAngle< sign*angle) {
            float tmpAngleSpeed = speedAngle * sign * Time.deltaTime;
            _mazeField.transform.RotateAround(rb2d.transform.position, Vector3.forward, tmpAngleSpeed);
            totalAngle += tmpAngleSpeed;
            yield return new WaitForEndOfFrame();
        }

        _mazeField.transform.rotation = qTO;
        _spiningNow = false;
        yield return null;
    }

    #region SmoothRotateFieldAroundPlayerPoint - need FIX
    private IEnumerator SmoothRotateFieldAroundPlayerPoint(float angle, int sign) {
        Quaternion qTO = _mazeField.transform.rotation * Quaternion.Euler(0, 0, angle);
        float spAngle = 0;
        float totalAngle = 0;
        while (totalAngle * sign < angle * sign) {

            spAngle = speedAngle * sign * Time.deltaTime;
            _mazeField.transform.RotateAround(rb2d.transform.position, Vector3.forward, spAngle);
            totalAngle = totalAngle + spAngle;
            yield return new WaitForFixedUpdate();
        }
        _mazeField.transform.rotation = qTO;
        yield return null;
    }


    #endregion


    private bool _spiningNow;

   

}

public enum Turn {
    Right,
    Left
}

public enum MoveDirection {
    LEFT,
    RIGHT,
    UP,
    DOWN
}