using UnityEngine;
using System.Collections;
using Cinemachine;

public abstract class AbstractPlayerController : MonoBehaviour {

    protected Rigidbody2D rb2d;
    protected Transform rb2dTransform;
    public bool isMoving;
    protected float inverseMoveTime;
    public float moveTime = 0.1f;

    [Range(0f, 10f)] public float speed;
    [Range(0, 10f)] public float speedAngle;

    public void Init() {
        rb2d = GetComponent<Rigidbody2D>();
        rb2dTransform = rb2d.transform;
        inverseMoveTime = 1f / moveTime;
        speed = 3.5f;
        speedAngle = 6f;
    }

    public abstract IEnumerator TurnCharacter();

    public abstract void Move();

}