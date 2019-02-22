using UnityEngine;
using System.Collections;
using Cinemachine;

public abstract class AbstractPlayerController : MonoBehaviour {

   [HideInInspector] public Rigidbody2D rb2d;
    [HideInInspector] public Transform rb2dTransform;
    public bool isMoving;
    protected float inverseMoveTime;
    public float moveTime = 0.4f;

    [Range(0f, 5f)] public float speed;
    [Range(0, 10f)] public float speedAngle;

    public void Init() {
        rb2d = GetComponent<Rigidbody2D>();
        rb2dTransform = rb2d.transform;
        inverseMoveTime = 1f / moveTime;
        speed = 2f;
        speedAngle = 3.5f;
    }

    public abstract IEnumerator TurnCharacter();

    public abstract void Move();

}