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
    [SerializeField] private Transform startPoint;

    [SerializeField] private RandomMazeGenerator rmg;

    [HideInInspector] private GameObject spiritsContainer;
    [SerializeField] private GameObject spirit;
    [SerializeField] private GameObject spiritS;

    void Awake() {
        instance = this;
        modeOfController = ModeController.Dynamic;
        spiritsContainer = new GameObject("EnemyContainer");
        spiritsContainer.transform.position = new Vector3(-9.5f, -7.5f, 0);
    }

    public void NewGame() {
        StopAllCoroutines();
        var childs = spiritsContainer.transform.GetComponentsInChildren<Transform>();
        for (int i = childs.Length - 1; i > 0; i--) {
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
        InitialEnemyPosition();
        Restart();
    }

    public void SetPlayerPosition() {
        ((PlayerController)characterController).SetPlayerPosition(startPoint.position);
    }

    private void InitialPlayer() {
        StartCoroutine(characterController.TurnCharacter());
        characterController.isMoving = false;
        characterController.Move();
        ((PlayerController) characterController).vCam.gameObject.SetActive(true);
    }

    private void InitialEnemyPosition() {
        for (int i = 0; i < WayPointController.instance.wayIsland.Length; i++) {
            var go = Instantiate<GameObject>(spirit);
            go.transform.SetParent(spiritsContainer.transform, true);
            AbstractEnemyController enemy = go.AddComponent<EnemyWayPointMove>();
            enemy.ways = new Vector3[WayPointController.instance.wayIsland[i].positionCount];
            WayPointController.instance.wayIsland[i].GetPositions(enemy.ways);
            enemy.isMoving = true;
            enemy.Move();
        }

        for (int i = 0; i < WayPointController.instance.waySimple.Length; i++) {
            var go = Instantiate<GameObject>(spirit);
            go.transform.SetParent(spiritsContainer.transform, true);
            AbstractEnemyController enemy = go.AddComponent<SpiritBezieMoving>();
            enemy.ways = new Vector3[WayPointController.instance.waySimple[i].positionCount];
            WayPointController.instance.waySimple[i].GetPositions(enemy.ways);
            enemy.isMoving = true;
            enemy.Move();
        }
    }

    private void InitialPlayerPosition() {
        TileM startTile = rmg.maze.StartTile;
        if (startTile != null) {
            character.SetActive(true);
            character.transform.position = startTile.Tile.transform.position;
            characterController.isMoving = false;
            StartCoroutine(characterController.TurnCharacter());
            characterController.Move();
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

    public void Restart() {
        StopAllCoroutines();
        SetPlayerPosition();
        InitialPlayer();
        //InitialEnemyPosition();
    }
}