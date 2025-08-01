using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    private struct PlayerInputRecord
    {
        public Vector2 move;
        public Quaternion rotation;
        public bool doesAttack;
    }

    private InputAction moveAction;
    private InputAction attackAction;
    private InputAction pointAction;

    [SerializeField]
    private Character playerPrefab;
    private Character currentPlayer;

    private int currentFrame;
    private List<Character> previousPlayers = new();
    private List<List<PlayerInputRecord>> inputHistory = new();

    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        attackAction = InputSystem.actions.FindAction("Attack");
        pointAction = InputSystem.actions.FindAction("Point");
    }

    public void PlayNextCharacter() {
        if (currentPlayer != null) {
            previousPlayers.Add(currentPlayer);
        }
        foreach (var player in previousPlayers) {
            player.transform.position = Vector3.zero;
        }
        // Set up new player.
        inputHistory.Add(new());
        currentPlayer = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity).GetComponent<Character>();
        currentFrame = 0;
    }

    public void InternalUpdate()
    {
        if (attackAction.WasPressedThisFrame()) {
            currentPlayer.Attack();
            var currentHistory = inputHistory[inputHistory.Count - 1];
            var lastRecord = currentHistory[currentHistory.Count - 1];
            lastRecord.doesAttack = true;
            currentHistory[currentHistory.Count - 1] = lastRecord;
        }
    }

    public void InternalFixedUpdate()
    {
        PlayerInputRecord inputRecord = new();
        
        var m = moveAction.ReadValue<Vector2>();
        currentPlayer.Move(m);
        inputRecord.move = m;

        // Face towards point
        Vector2 screenPoint = pointAction.ReadValue<Vector2>();
        Vector2 point = Camera.main.ScreenToWorldPoint(screenPoint);
        Vector2 playerPos = currentPlayer.transform.position;
        Vector2 direction = point - playerPos;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        currentPlayer.transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
        inputRecord.rotation = currentPlayer.transform.rotation;
        
        for (int i = 0; i < previousPlayers.Count; i++)
        {
            if (currentFrame >= inputHistory[i].Count) continue;
            previousPlayers[i].transform.rotation = inputHistory[i][currentFrame].rotation;
            previousPlayers[i].Move(inputHistory[i][currentFrame].move);
            if (inputHistory[i][currentFrame].doesAttack) previousPlayers[i].Attack();
        }

        if (inputHistory.Count == 0) {
            throw new System.Exception("Cannot record player input: inputHistory is empty.");
        }
        inputHistory[inputHistory.Count - 1].Add(inputRecord);

        currentFrame++;
    }
}