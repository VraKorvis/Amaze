using UnityEngine;
using System.Collections;

public enum ModeController {
    Static,
    Dynamic,
}

public class GameController : MonoBehaviour {

    public static GameController instance;
    public GameObject character;
    private AbstractPlayerController characterController;
    public ModeController modeOfController;

    void Awake() {
        instance = this;
    }
    
    // Use this for initialization
    void Start() {
        SetController(modeOfController);
    }

    // Update is called once per frame
    void Update() {
        characterController.TurnCharacter();
    }

    void FixedUpdate() {
        characterController.MoveUp();
    }

    public void SetController(ModeController mode) {
        switch (mode) {
            case ModeController.Dynamic:
                characterController = character.GetComponent<PlayerController>();
                break;
            case ModeController.Static:
                characterController = character.GetComponent<DirectionController>();
                break;
        }
    }

    public void SwitchMode() {
        modeOfController = modeOfController == ModeController.Static ? ModeController.Dynamic : ModeController.Static;
        SetController(modeOfController);
    }

}
