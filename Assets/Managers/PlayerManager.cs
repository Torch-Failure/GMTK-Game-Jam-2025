using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerManager : MonoBehaviour
{
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

        public CharacterThread(Character threadChar)
        {
            history = new();
            threadTicks = 0;
            threadCharacter = threadChar;
        }        

        public void PushHistory(PlayerInputRecord record)
        {
            history.Add(record);
            threadTicks++;
        }

        public void Reset()
        {
            history.Clear();
            threadTicks = 0;
        }
    }

    private struct LoopStartState
    {
        // Need to store state of characters at start of loop
        // For now, just transform
        public List<Vector3> characterPositions;
        public List<Quaternion> characterRotations;

        public void SaveState(List<CharacterThread> threads)
        { 
            characterPositions.Clear();
            characterRotations.Clear();

            foreach (var thread in  threads)
            {
                var character = thread.threadCharacter;
                characterPositions.Add(character.transform.position);
                characterRotations.Add(character.transform.rotation);
            }
        }

        public void LoadState(List<CharacterThread> threads)
        {
            for (int i = 0; i < threads.Count; i++)
            {
                var character = threads[i].threadCharacter;
                character.transform.position = characterPositions[i];
                character.transform.rotation = characterRotations[i];
            }
        }
    }

    private InputAction moveAction;
    private InputAction attackAction;
    private InputAction pointAction;
    private InputAction cycleNextAction;
    private InputAction cyclePreviousAction;
    private InputAction selectAction;

    public Character playerPrefab;

    private LoopStartState loopStartState;

    public int numberOfCharacters = 4;
    private List<CharacterThread> characterThreads = new();
    
    // That the player is currently playing
    private CharacterThread activeThread; 
    private Character currentPlayer => activeThread.threadCharacter;

    // Temp before player selection
    private int currentPlayerIndex = 0;

    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        attackAction = InputSystem.actions.FindAction("Attack");
        pointAction = InputSystem.actions.FindAction("Point");
        cycleNextAction = InputSystem.actions.FindAction("Next");
        cyclePreviousAction = InputSystem.actions.FindAction("Previous");
        selectAction = InputSystem.actions.FindAction("Interact");

        // Simple test implementation for creating characters
        for (int i = 0; i < numberOfCharacters; i++)
        {
            int xOffset = -9 + i * 4;
            Character currentPlayer = Instantiate(playerPrefab, Vector3.right * (float)xOffset, Quaternion.identity).GetComponent<Character>();
            CharacterThread newThread = new(currentPlayer);
            characterThreads.Add(newThread);
        }

        loopStartState.characterPositions = new();
        loopStartState.characterRotations = new();

        SaveLoopStart();
    }

    public void SaveLoopStart()
    {   
        loopStartState.SaveState(characterThreads);
    }

    public void LoadLoopStart()
    {
        loopStartState.LoadState(characterThreads);
    }

    public void InternalUpdate()
    {
        if (attackAction.WasPressedThisFrame()) {
            currentPlayer.Attack();
            var currentHistory = activeThread.history;
            var lastRecord = currentHistory[currentHistory.Count - 1];
            lastRecord.doesAttack = true;
            currentHistory[currentHistory.Count - 1] = lastRecord;
        }
    }

    public void CharacterSelectionUpdate()
    {

    }

    public void ThreadPlayingUpdate(int loopTick)
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
        activeThread.PushHistory(inputRecord);
        
        // Play out history of other characters
        for (int i = 0; i < numberOfCharacters; i++)
        {
            // Don't play out history of active character
            // This is kind of ugly
            if (i == currentPlayerIndex) continue;

            var inputHistory = characterThreads[i].history;
            var character = characterThreads[i].threadCharacter;

            if (loopTick >= inputHistory.Count) continue;
            character.transform.rotation = inputHistory[loopTick].rotation;
            character.Move(inputHistory[loopTick].move);
            if (inputHistory[loopTick].doesAttack) character.Attack();
        }
    }
    
    public void RestartLoop()
    {
        // Temp before selecting characters
        currentPlayerIndex++;
        currentPlayerIndex %= numberOfCharacters;
        activeThread = characterThreads[currentPlayerIndex];
        activeThread.Reset(); // Temp before proper advacning to next loop
    }
}