using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadLvlManager : MonoBehaviour {

    public static LoadLvlManager instance;

    public void Awake() {
        instance = this;
        DontDestroyOnLoad(this);
    }
    public void LoadNewGame() {
        SceneManager.LoadScene(1);
    }

    public void LoadMenu() {
        SceneManager.LoadScene(0);
    }


}
