using UnityEngine;
using System.Collections;

public class DirectionController : AbstractPlayerController {
    private Vector2 direction;
    private bool isPressed;

    private float fingerStartTime = 0.0f;
    private Vector2 fingerStartPos = Vector2.zero;

    private bool isSwipe = false;
    [SerializeField] private float minSwipeDist = 1.0f;
    private float maxSwipeTime = 1.5f;
    private bool autoMove = true;

    void Awake() {
        base.Init();
        direction = rb2dTransform.up;
    }

    public override void TurnCharacter() { }

    public void MoveKeyDir() {
        if (Input.GetKey(KeyCode.RightArrow)) {
            direction = rb2dTransform.right;
        } else if (Input.GetKey(KeyCode.LeftArrow)) {
            direction = -rb2dTransform.right;
        } else if (Input.GetKey(KeyCode.DownArrow)) {
            direction = -rb2dTransform.up;
        } else if (Input.GetKey(KeyCode.UpArrow)) {
            direction = rb2dTransform.up;
        } else {
            return;
        }
        rb2d.MovePosition(rb2d.position + direction * speed * Time.fixedDeltaTime);
    }

    private void MoveTouchDir(TurnDirection dir) {
        switch (dir) {
            case TurnDirection.UP:
                direction = rb2dTransform.up;
                break;
            case TurnDirection.LEFT:
                direction = -rb2dTransform.right;
                break;
            case TurnDirection.RIGHT:
                direction = rb2dTransform.right;
                break;
            case TurnDirection.DOWN:
                direction = -rb2dTransform.up;
                break;
        }
    }

    public override void MoveUp() {
        if (autoMove) {
            rb2d.MovePosition(rb2d.position + direction * speed * Time.fixedDeltaTime);
        }

        MoveDir();
     //   MoveTapDir();
    }

    private void MoveDir() {
        foreach (var touch in Input.touches) {
            switch (touch.phase) {
                case TouchPhase.Began:
                    isSwipe = true;
                    fingerStartTime = Time.time;
                    fingerStartPos = touch.position;
                    break;
                case TouchPhase.Canceled:
                    isSwipe = false;
                    break;

                case TouchPhase.Ended:

                    float gestureTime = Time.time - fingerStartTime;
                    float gestureDist = (touch.position - fingerStartPos).sqrMagnitude;
                    if (isSwipe && gestureTime < maxSwipeTime && gestureDist > minSwipeDist) {
                        Vector2 direction = touch.position - fingerStartPos;
                        Vector2 swipeType = Vector2.zero;

                        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y)) {
                            // the swipe is horizontal:
                            swipeType = Vector2.right * Mathf.Sign(direction.x);
                        } else {
                            // the swipe is vertical:
                            swipeType = Vector2.up * Mathf.Sign(direction.y);
                        }

                        if (swipeType.x != 0.0f) {
                            if (swipeType.x > 0.0f) {
                                // MOVE RIGHT
                                MoveTouchDir(TurnDirection.RIGHT);
                            } else {
                                // MOVE LEFT
                                MoveTouchDir(TurnDirection.LEFT);
                            }
                        }

                        if (swipeType.y != 0.0f) {
                            if (swipeType.y > 0.0f) {
                                // MOVE UP
                                MoveTouchDir(TurnDirection.UP);
                            } else {
                                // MOVE DOWN
                                MoveTouchDir(TurnDirection.DOWN);
                            }
                        }
                    }

                    break;
            }
        }
    }

    private void MoveTapDir() {
       
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            Vector3 pos = Camera.main.WorldToScreenPoint(Input.mousePosition);
           Debug.Log(pos);
        }
    }
 
}