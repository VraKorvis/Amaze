using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ExitPoint : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D col) {
       // Debug.Log("Exit  " + col.tag );
        if (col.CompareTag("Player")) {
            SceneManager.LoadScene(0);
        }
    }
}
