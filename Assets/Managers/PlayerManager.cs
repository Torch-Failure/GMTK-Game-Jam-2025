using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    private struct PlayerInputRecord
    {
        public Vector2 move;
        public bool doesAttack;
        public bool doesInteract;
    }

    private InputAction moveAction;

    [SerializeField]
    private Character playerPrefab;
    private Character currentPlayer;

    private int currentFrame;
    private List<Character> previousPlayers = new();
    private List<List<PlayerInputRecord>> inputHistory = new();

    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
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
        PlayerInputRecord inputRecord = new();
        
        currentPlayer.Move(moveAction.ReadValue<Vector2>());
        inputRecord.move = moveAction.ReadValue<Vector2>();
        
        for (int i = 0; i < previousPlayers.Count; i++)
        {
            if (currentFrame >= inputHistory[i].Count) continue;
            previousPlayers[i].Move(inputHistory[i][currentFrame].move);
        }

        if (inputHistory.Count == 0) {
            throw new System.Exception("Cannot record player input: inputHistory is empty.");
        }
        inputHistory[inputHistory.Count - 1].Add(inputRecord);

        currentFrame++;
    }
}