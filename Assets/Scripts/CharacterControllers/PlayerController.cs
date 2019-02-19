using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : AbstractPlayerController {
    public CinemachineVirtualCamera vCam;
    [HideInInspector] public Transform vCamTransform;
    [Range(0.1f, 1f)] public float motionCoef;
    [SerializeField] private Slider sliderMotion;
    [SerializeField] private Slider sliderSpeed;
    public Vector2 vector_direction = new Vector2(0, 1);

    public Tilemap tilemap;
    [HideInInspector] private GridInformation gridInf;
    [SerializeField] private GridLayout gridLayout;

    private readonly Vector3 cellLocalPos = new Vector3(0.5f, 0.4f, 0.0f);

    void Awake() {
        base.Init();
        //vCam = GameObject.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>();
        vCamTransform = vCam.GetComponent<Transform>();
    }

    void Start() {
        gridInf = gridLayout.GetComponent<GridInformation>();
    }

    public override IEnumerator TurnCharacter() {
        while (true) {
            if (Input.GetKeyDown(KeyCode.Mouse0)) {
                Vector2 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
                if (pos.x > 0.5) {
                    TurnTo(Direction.Right);
                } else if (pos.x < 0.5) {
                    TurnTo(Direction.Left);
                }
            }
            yield return null;
        }
    }

    public void TurnTo(Direction direction) {
        int sign = direction == Direction.Right ? -1 : 1;

        
        if (isSpiningNow) {
            StopCoroutine("Turn");
            float currentAngel = vCamTransform.rotation.eulerAngles.z;
            float divisionRightAngle = Mathf.Abs(currentAngel % 90) * sign;
            float divisionLeftAngle = 90 - Mathf.Abs(currentAngel % 90) * sign;
            float angle = direction == Direction.Right ? divisionRightAngle : divisionLeftAngle;
            StartCoroutine(Turn(angle, -1));
        } else {
            StartCoroutine(Turn(sign * 90, sign));
            vector_direction = Quaternion.Euler(0, 0, 90 * sign) * vector_direction;
        }
    }

    public void SetPlayerPosition(Vector3 pos) {
        Vector3Int cellPosition = gridLayout.LocalToCell(pos);
       
        rb2d.position = gridLayout.CellToLocalInterpolated(cellPosition + cellLocalPos);

        rb2dTransform.rotation = Quaternion.Euler(0, 0, 0);
        vCam.transform.rotation = transform.rotation;
        vector_direction = new Vector2(0, 1);
    }

    public override void Move() {
        //  StartCoroutine(SmoothMove());
        StartCoroutine(TileMovement());
    }

    private IEnumerator TileMovement() {
        while (true) {
            //Vector3Int localPlace = tilemap.WorldToCell(rb2d.position);
            //localPlace.x = localPlace.x + 1;
            //localPlace.y = localPlace.y + 0;
            if (!isMoving) {
                Vector2 startPos = rb2d.position;
                Vector2 endPos = startPos + vector_direction;

                // Vector3 endPos = gridLayout.CellToLocalInterpolated(tilemap.CellToWorld(localPlace) + cellLocalPos);
                //Vector2 endPos = tilemap.GetCellCenterWorld(localPlace);
                bool isObstracle = tilemap.HasObstacleTile(endPos);
                if (!isObstracle) {
                    isMoving = true;
                    StartCoroutine(SmoothMove2(endPos));
                }
            }

            yield return null;
        }
    }

    private IEnumerator SmoothMove2(Vector2 end) {
        float sqrRemainingDistance = (rb2d.position - end).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon) {
            motionCoef = sliderMotion.value;
            float motion = isSpiningNow ? motionCoef : 1f;
            speed = sliderSpeed.value * motion;
            
            Vector3 nPos = Vector3.MoveTowards(rb2d.position, end, inverseMoveTime * Time.deltaTime * speed);
            rb2dTransform.position = nPos;
            // rb2d.MovePosition(nPos);
            sqrRemainingDistance = (rb2d.position - end).sqrMagnitude;
            yield return null;
        }

        isMoving = false;
    }


    public IEnumerator SmoothMove() {
        while (isMoving) {
            motionCoef = sliderMotion.value;
            speed = sliderSpeed.value;
            float motion = isSpiningNow ? motionCoef : 1f;
            rb2d.MovePosition(rb2d.position + (Vector2) rb2dTransform.up * speed * Time.fixedDeltaTime * motion);
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
            StopAllCoroutines();
            GameController.instance.Restart();
        }

        if (other.CompareTag("Exit")) {
            StopAllCoroutines();
            GameController.instance.Restart();
        }
    }

    public void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("EnemySpirit")) {
            StopAllCoroutines();
            GameController.instance.Restart();
        }

        if (other.gameObject.CompareTag("Exit")) {
            StopAllCoroutines();
            GameController.instance.Restart();
        }
    }
}