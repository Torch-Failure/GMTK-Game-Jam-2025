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
        private int currentLoopTick = 0; // Sent down to us by the loop manager

        public int numberOfCharacters = 4;
        private List<CharacterThread> characterThreads = new();
        private List<CharacterThread> activatableThreads = new();

        
        // Thread that the player is currently playing
        private ActiveThread activeThreadContainer = new();
        
        // Helper helper class for rendering character in red
        private CharacterThread  activeThread 
        {
            get => activeThreadContainer.activeThread;
            set => activeThreadContainer.activeThread = value; 
        }

        public Character currentPlayer 
        {
            get => activeThread.threadCharacter;
            private set => activeThread.threadCharacter = value;
        }

        // Used for character selection:
        private int selectedCharacterIndex = 0;
        public bool isCharacterSelected {get; private set;} = false;
        public event Action OnActivePlayerKilled;

        void Start()
        {
            moveAction = InputSystem.actions.FindAction("Move");
            attackAction = InputSystem.actions.FindAction("Attack");
            pointAction = InputSystem.actions.FindAction("Point");
            cycleNextAction = InputSystem.actions.FindAction("Next");
            cyclePreviousAction = InputSystem.actions.FindAction("Previous");
            selectAction = InputSystem.actions.FindAction("Interact");
        }

        // Called by loop manager at start of the very first loop
        public void InitLoop()
        {
            // Simple test implementation for creating characters
            for (int i = 0; i < numberOfCharacters; i++)
            {
                int xOffset = -9 + i * 4;
                Character currentPlayer = Instantiate(playerPrefab, Vector3.right * (float)xOffset, Quaternion.identity).GetComponent<Character>();
                currentPlayer.OnDeath += OnPlayerDeath;
                CharacterThread newThread = new(currentPlayer);
                characterThreads.Add(newThread);
            }
            
            // Ensure this is not null before first fixed update
            activeThread = characterThreads[selectedCharacterIndex];

            loopStartState.characterPositions = new();
            loopStartState.characterRotations = new();
            loopStartState.characterStates = new();
            loopStartState.characterHealths = new();
            loopStartState.characterAttackCooldowns = new();
            
            SaveLoopStart();
        }

        public void PreviousCharacter() {
            selectedCharacterIndex--;
            UpdateSelectedCharacter();
        }

        public void NextCharacter() {
            selectedCharacterIndex++;
            UpdateSelectedCharacter();
        }

        public void UpdateSelectedCharacter()
        {
            // Check validity
            if (activatableThreads.Count == 0)
            {
                throw new InvalidOperationException("Should not be in character selection state if no selectable characters");
            }

            // Wrap index around
            if (selectedCharacterIndex < 0)
            {
                selectedCharacterIndex = activatableThreads.Count - 1;
            }

            // Wrap index around
            if (selectedCharacterIndex >= activatableThreads.Count)
            {
                selectedCharacterIndex = 0;
            }

            // Update selection
            activeThread = activatableThreads[selectedCharacterIndex];
        }

        public void SelectCharacter() {
                // Handle selection
                if (!(activeThread.state == CharacterThread.ThreadState.Unplayed ||
                      activeThread.state == CharacterThread.ThreadState.Inactive))
                {
                    throw new InvalidOperationException("Tried to select thread that was not in selectable state");
                }

                activeThread.state = CharacterThread.ThreadState.Active;
                isCharacterSelected = true;
        }

        public void SaveLoopStart()
        {   
            loopStartState.SaveState(characterThreads);
        }

        public void LoadLoopStart()
        {
            Debug.Log("Loading state...");
            loopStartState.LoadState(characterThreads);
            isCharacterSelected = false;
            currentLoopTick = 0;
            activatableThreads = GetActivatableThreads();
        }

        // Called by character death handler when a player dies
        public void OnPlayerDeath(Character character)
        {
            foreach (var thread in characterThreads)
            {
                if (thread.threadCharacter == character)
                {
                    thread.SetDeathInfo(currentLoopTick);
                }
            }

            if (activeThread.threadCharacter == character)
            {
                    Debug.Log("Active player killed!");
                    OnActivePlayerKilled?.Invoke();
            }
        }

        public void ThreadPlayingUpdate()
        {
            if (attackAction.WasPressedThisFrame() && activeThread.IsUnderPlayerControl(currentLoopTick)) {
                currentPlayer.Attack();
                var currentHistory = activeThread.history;
                var lastRecord = currentHistory[currentHistory.Count - 1];
                lastRecord.doesAttack = true;
                currentHistory[currentHistory.Count - 1] = lastRecord;
            }
        }

        public void CharacterSelectionUpdate()
        {
            // Update selection from inputs
            if (cycleNextAction.WasPressedThisFrame())
            {
                NextCharacter();
            }

            if (cyclePreviousAction.WasPressedThisFrame())
            {
                PreviousCharacter();
            }
            
            // Guarantee that this gets called even if not next/previous action
            // This handles case when we first enter selection mode and the active
            // thread is whatever we just finished a loop as. 
            // Ideally this would be an OnEnterSelectionState thing but this is easier 
            // and works fine
            UpdateSelectedCharacter();

            if (selectAction.WasPressedThisFrame())
            {
                SelectCharacter();
            }
        }

        public void ThreadPlayingFixedUpdate(int loopTick)
        {
            foreach (var thread in characterThreads)
            {
                thread.threadCharacter.ThreadPlayingFixedUpdate();
            }

            currentLoopTick = loopTick;
            bool playerControlActive = activeThread.IsUnderPlayerControl(currentLoopTick);

            if (playerControlActive)
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
            }
            
            // Play out history of other characters
            for (int i = 0; i < numberOfCharacters; i++)
            {
                var thread = characterThreads[i];

                // If character is active AND is under player control, we shouldn't
                // play out thier history
                if (thread == activeThread && playerControlActive) continue;
                
                // Don't play history of anything which is dead
                if (thread.threadCharacter.state == Character.CharacterState.Dead) continue;

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

                if (thread.threadCharacter.state == Character.CharacterState.Dead)
                {
                    // If a thread is dead AND we are moving to the next loop, they are dead dead
                    thread.state = CharacterThread.ThreadState.PermenantlyDead;
                }

                thread.Reset();
            }
        }

        public void OnLoopEnd()
        {
            // If a loop ends and the active thread is alive, it has timed out
            if (activeThread != null)
            {
                if (activeThread.threadCharacter.state == Character.CharacterState.Alive)
                {
                    activeThread.state = CharacterThread.ThreadState.Complete;
                }
                else
                {
                    // We were cut short by something (death)
                    activeThread.state = CharacterThread.ThreadState.Inactive;
                }
            }

            // Handle threads that previously died and now lived long, happy lives
            foreach (var thread in characterThreads)
            {
                // If were inactive AND previously died (both of these captured in inactive state)
                // AND did not die this time 
                if (thread.state == CharacterThread.ThreadState.Inactive && 
                    thread.threadCharacter.state != Character.CharacterState.Dead)
                {
                    // Assign an arbitrairily high death tick
                    thread.SetDeathInfo(int.MaxValue);
                }   
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
        public void Freeze()
        {
            foreach (var player in characterThreads)
            {
                var rb = player.threadCharacter.GetComponent<Rigidbody2D>();
                if (rb == null)
                {
                    throw new InvalidOperationException("Player rigid body not there");
                }

                rb.simulated = false;
            }
        }

        public void Unfreeze()
        {
            foreach (var player in characterThreads)
            {
                var rb = player.threadCharacter.GetComponent<Rigidbody2D>();
                if (rb == null)
                {
                    throw new InvalidOperationException("Player rigid body not there");
                }

                rb.simulated = true;
            }
        }
    }
}