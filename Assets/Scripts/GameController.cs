using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ModeController {
    Dynamic,
    Static
}

public enum SpiritType {
    Island,
    Weak,
    BackForth
}

public class GameController : MonoBehaviour {
    public static GameController instance;
    public GameObject character;
    private AbstractPlayerController characterController;
    public ModeController modeOfController;

    [SerializeField] private RandomMazeGenerator rmg;

    [SerializeField] private GameObject spiritsContainer;
    [SerializeField] private List<GameObject> spiritsIsland;
    [SerializeField] private List<GameObject> spiritsBackForth;

    void Awake() {
        instance = this;
        modeOfController = ModeController.Dynamic;
    }

    public void NewGame() {
        StopAllCoroutines();
        var childs = spiritsContainer.transform.GetComponentsInChildren<Transform>();
        for (int i = childs.Length-1; i > 0; i--) {
            Destroy(childs[i].gameObject);
        }
        GenerateMaze();
    }

    public void GenerateMaze() {
        Invoke("Generate", 1f);
    }

    private void Generate() {
        character.SetActive(false);
        ((PlayerController) characterController).vCam.gameObject.SetActive(false);
        rmg.MakeMaze();
    }

    void Start() {
        SetController(modeOfController);
        rmg.OnCreateMaze += InitialPlayerPosition;
        rmg.OnCreateMaze += InitialEnemyPosition;
    }

    private void InitialEnemyPosition() {
        foreach (var e in spiritsIsland) {
            var go = Instantiate<GameObject>(e);
            go.transform.SetParent(spiritsContainer.transform);
            AbstractPlayerController spirit = go.GetComponent<AbstractPlayerController>();
            StartCoroutine(spirit.TurnCharacter());
            StartCoroutine(spirit.MoveUp());
        }
        foreach (var e in spiritsBackForth) {
            var go = Instantiate<GameObject>(e);
            go.transform.SetParent(spiritsContainer.transform);
            AbstractPlayerController spirit = go.GetComponent<AbstractPlayerController>();
           // StartCoroutine(spirit.TurnCharacter());
            StartCoroutine(spirit.MoveUp());
        }

    }

    private void InitialPlayerPosition() {
        TileM startTile = rmg.maze.StartTile;
        if (startTile != null) {
            character.SetActive(true);
            character.transform.position = startTile.Tile.transform.position;
            characterController.isMoving = true;
            StartCoroutine(characterController.TurnCharacter());
            StartCoroutine(characterController.MoveUp());
            ((PlayerController) characterController).vCam.gameObject.SetActive(true);
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