using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract  class AbstractEnemyController : AbstractPlayerController {
    protected GameObject tPoint;
    protected LayerMask layer;
    protected bool spiningNow;

    public override IEnumerator MoveUp() {
        SetPosition();
        while (isMoving) {
            rb2d.MovePosition(rb2d.position + (Vector2)rb2dTransform.up * speed * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }
        yield return null;
    }

    protected abstract void SetPosition();
}
