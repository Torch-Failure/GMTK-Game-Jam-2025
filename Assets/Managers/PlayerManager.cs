using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerManager : MonoBehaviour
{

    // These events talk to the loop manager
    public event Action characterThreadTimeUp;

    public int loopLengthTicks;
    private int currentTick;


    private struct PlayerInputRecord
    {
        public Vector2 move;
        public Quaternion rotation;
        public bool doesAttack;
    }

    // Stores history of one character for the current loop
    private struct CharacterThread
    {
        // Some space for history
        // Starting time
        // Current time we are up to
        // Dead or not dead?
        // Update when active
        public List<PlayerInputRecord> history;
        
        // How much time has been played out in this thread
        public uint threadTicks; 

        public Character threadCharacter;

        public void PushHistory(PlayerInputRecord record)
        {
            history.Add(record);
            threadTicks++;
            // assert()
        }

        public void Reset()
        {
            history.Clear();
            threadTicks = 0;
        }
    }

    private InputAction moveAction;
    private InputAction attackAction;
    private InputAction pointAction;
    private InputAction cycleNextAction;
    private InputAction cyclePreviousAction;
    private InputAction selectAction;

    public Character playerPrefab;
    private Character currentPlayer;

    private List<Character> previousPlayers = new();
    private List<List<PlayerInputRecord>> inputHistory = new();
    private List<CharacterThread> characterThreads = new();
    
    // That the player is currently playing
    // private CharacterThread activeThread; 
    // private Character currentPlayer => activeThread.threadCharacter;

    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        attackAction = InputSystem.actions.FindAction("Attack");
        pointAction = InputSystem.actions.FindAction("Point");
        cycleNextAction = InputSystem.actions.FindAction("Next");
        cyclePreviousAction = InputSystem.actions.FindAction("Previous");
        selectAction = InputSystem.actions.FindAction("Interact");
    }

    public void PlayNextCharacter() {

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

    public void CharacterSelectionUpdate()
    {

    }

    public void ThreadPlayingUpdate()
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
            if (currentTick >= inputHistory[i].Count) continue;
            previousPlayers[i].transform.rotation = inputHistory[i][currentTick].rotation;
            previousPlayers[i].Move(inputHistory[i][currentTick].move);
            if (inputHistory[i][currentTick].doesAttack) previousPlayers[i].Attack();
        }

        if (inputHistory.Count == 0) {
            throw new System.Exception("Cannot record player input: inputHistory is empty.");
        }
        inputHistory[inputHistory.Count - 1].Add(inputRecord);

        currentTick++;

        if (currentTick > loopLengthTicks)
        {
            // characterThreadTimeUp?.Invoke();
        }
    }

    
    public void RestartLoop()
    {
        if (currentPlayer != null) {
            previousPlayers.Add(currentPlayer);
        }
        foreach (var player in previousPlayers) {
            player.transform.position = Vector3.zero;
        }

        // Set up new player.
        inputHistory.Add(new());
        currentPlayer = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity).GetComponent<Character>();
        currentTick = 0;
    }
}