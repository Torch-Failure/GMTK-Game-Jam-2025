using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

namespace PlayerManager {
    public class PlayerManager : MonoBehaviour
    {
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
        
        // Thread that the player is currently playing
        private ActiveThread activeThreadContainer = new();
        
        // Helper helper class for rendering character in red
        private CharacterThread  activeThread 
        {
            get => activeThreadContainer.activeThread;
            set => activeThreadContainer.activeThread = value; 
        }

        private Character currentPlayer => activeThread.threadCharacter;

        // Used for character selection:
        private int selectedCharacterIndex = 0;
        public bool isCharacterSelected {get; private set;} = false;

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
            isCharacterSelected = false;
        }

        public void ThreadPlayingUpdate()
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
            // Check validity
            var activatableThreads = GetActivatableThreads();        
            if (activatableThreads.Count == 0)
            {
                throw new InvalidOperationException("Should not be in character selection state if no selectable characters");
            }

            // Update selection from inputs
            if (cycleNextAction.WasPressedThisFrame())
            {
                selectedCharacterIndex++;
            }

            if (cyclePreviousAction.WasPressedThisFrame())
            {
                selectedCharacterIndex--;
            }
            
            // Wrap index around
            if (selectedCharacterIndex >= activatableThreads.Count)
            {
                selectedCharacterIndex = 0;
            }

            if (selectedCharacterIndex < 0)
            {
                selectedCharacterIndex = activatableThreads.Count - 1;
            }

            // Apply selection
            activeThread = activatableThreads[selectedCharacterIndex];

            if (selectAction.WasPressedThisFrame())
            {
                isCharacterSelected = true;
            }
        }

        public void ThreadPlayingFixedUpdate(int loopTick)
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
                var thread = characterThreads[i];

                // Don't play out history of active character
                // This is kind of ugly
                if (thread == activeThread) continue;

                var inputHistory = thread.history;
                var character = thread.threadCharacter;

                // Don't play out past history we have saved
                if (loopTick >= inputHistory.Count) continue;
                character.transform.rotation = inputHistory[loopTick].rotation;
                character.Move(inputHistory[loopTick].move);
                if (inputHistory[loopTick].doesAttack) character.Attack();
            }
        }

        public void NextLoop()
        {
            for (int i = 0; i < characterThreads.Count; i++)
            {
                var thread  = characterThreads[i];
                
                // Smoother experience by maintaining selection between loops
                if (thread == activeThread)
                {
                    selectedCharacterIndex = i;
                }

                thread.Reset();
            }
        }

        private List<CharacterThread> GetActivatableThreads()
        {
            List<CharacterThread> activatableThreads = new();
            foreach (var thread in characterThreads)
            {
                if (thread.CanActivate())
                {
                    activatableThreads.Add(thread);
                }
            }
            return activatableThreads;
        }

        // Returns true if any player can be activated
        // Used by loop manager to decide whether to advance to next loop
        public bool CanActivateCharacter()
        {
            return GetActivatableThreads().Count > 0;
        }
    }
}