using UnityEngine;
using System.Collections;

public enum ModeController {
    Dynamic,
    Static
    
}

public class GameController : MonoBehaviour {

    public static GameController instance;
    public GameObject character;
    private AbstractPlayerController characterController;
    public ModeController modeOfController;

   [SerializeField] private RandomMazeGenerator rmg;

    void Awake() {
        instance = this;
        modeOfController = ModeController.Dynamic;
    }

    public void GenerateMaze() {
        Invoke("Generate", 1f);
    }

    private void Generate() {
        StopAllCoroutines();
        character.SetActive(false);
        ((PlayerController)characterController).vCam.gameObject.SetActive(false);
        rmg.MakeMaze();
    }
    
    void Start() {
        SetController(modeOfController);
        rmg.OnCreateMaze += InitialPlayerPosition;
    }

    private void InitialPlayerPosition() {
        TileM startTile = rmg.maze.StartTile;
        if (startTile != null) {
            character.SetActive(true);
            character.transform.position = startTile.Tile.transform.position;
            characterController.move = true;
            StartCoroutine(characterController.TurnCharacter());
            StartCoroutine(characterController.MoveUp());
            ((PlayerController)characterController).vCam.gameObject.SetActive(true);
        }
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
