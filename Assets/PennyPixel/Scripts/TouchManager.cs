using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class TouchManager : MonoBehaviour {
    public static TouchManager instance;

    public Rigidbody2D _characterRB;
   

    void Awake() {
        instance = this;
    }

}


