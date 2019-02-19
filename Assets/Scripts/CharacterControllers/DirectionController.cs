﻿using UnityEngine;
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

    public override IEnumerator TurnCharacter() {
        yield return null;
    }

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

    private void MoveTouchDir(Direction dir) {
        switch (dir) {
            case Direction.Top:
                direction = rb2dTransform.up;
                break;
            case Direction.Left:
                direction = -rb2dTransform.right;
                break;
            case Direction.Right:
                direction = rb2dTransform.right;
                break;
            case Direction.Bot:
                direction = -rb2dTransform.up;
                break;
        }
    }

    public override void Move() {
        StartCoroutine(SmoothMovement());
    }

    private IEnumerator SmoothMovement() {
        if (autoMove) {
            rb2d.MovePosition(rb2d.position + direction * speed * Time.fixedDeltaTime);
        }
        MoveDir();
        yield return null;
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
                                MoveTouchDir(Direction.Right);
                            } else {
                                // MOVE LEFT
                                MoveTouchDir(Direction.Left);
                            }
                        }

                        if (swipeType.y != 0.0f) {
                            if (swipeType.y > 0.0f) {
                                // MOVE UP
                                MoveTouchDir(Direction.Top);
                            } else {
                                // MOVE DOWN
                                MoveTouchDir(Direction.Bot);
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