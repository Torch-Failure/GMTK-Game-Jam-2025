using System;
using PlayerManager;
using UnityEngine;

public class LoopManager : MonoBehaviour
{
        
        enum LoopState
        {
            PlayerSelection,
            ThreadPlaying
        };

        [SerializeField] 
        private int maxLoopLength = 200;

        public int LoopNumber { get; private set; }
        private int loopLengthTicks = 200; // How long each loop is
        private int currentTick = 0; // Current tick in this thread of the loop
        public PlayerManager.PlayerManager playerManager;
        public EnemyManager enemyManager;
        public ProjectileManager projectileManager;

        private LoopState state = LoopState.PlayerSelection;

        void Start()
        {
            playerManager.OnActivePlayerKilled += HandleActivePlayerDeath;
            InitLoop();
            Freeze();
        }

        // Should dispatch updates to pretty much everything else in the game depending on state
        public void InternalFixedUpdate()
        {
            switch(state)
            {
                case LoopState.PlayerSelection:
                    // Player is selecting next character to play as
                    // Handled in update instead of fixed update
                    break;
                case LoopState.ThreadPlaying:
                    // Player is playing as a character
                    playerManager.ThreadPlayingFixedUpdate(currentTick);
                    enemyManager.ThreadPlayingFixedUpdate();
                    projectileManager.ThreadPlayingFixedUpdate();
                    currentTick++;
                    if (currentTick >= loopLengthTicks)
                    {
                        HandleLoopEnd();
                    }
                    break;
            }
        }

        public void InternalUpdate()
        {
            switch(state)
            {
                case LoopState.PlayerSelection:
                    // Player is selecting next character to play as
                    playerManager.CharacterSelectionUpdate();
                    if (playerManager.isCharacterSelected)
                    {
                        // At this point physics is frozen, so unfreeze
                        Unfreeze();
                        state = LoopState.ThreadPlaying;
                        UI.SetState(UIState.ThreadPlaying);
                    }
                    break;
                case LoopState.ThreadPlaying:
                    // Player is playing as a character
                    playerManager.ThreadPlayingUpdate();
                    break;
            }
        }  

        private void HandleActivePlayerDeath()
        {
            Debug.Log("Handling play death...");
            HandleLoopEnd();
        }

        public void InitLoop()
        {
            projectileManager = ProjectileManager.instance;
            playerManager.InitLoop();
            enemyManager.InitLoop();
            projectileManager.InitLoop();
        }

        // Restores everything to where it was at the start of the current loop
        // Player history should be preserved
        public void RestartLoop()
        {   
            Debug.Log("Restarting loop...");
            // This can eventually operate on a list of common interfaces/super classes
            playerManager.LoadLoopStart();
            enemyManager.LoadLoopStart();
            projectileManager.LoadLoopStart();
            currentTick = 0;
        }

        void SaveStates()
        {
            playerManager.SaveLoopStart();
            enemyManager.SaveLoopStart();
            projectileManager.SaveLoopStart();
        }

        // Will move to next loop
        void NextLoop()
        {
            Debug.Log("Next looop.....");
            loopLengthTicks = maxLoopLength;
            playerManager.NextLoop();
            SaveStates();

        }

        private void HandleLoopEnd()
        {   
            // Handle timing out threads
            playerManager.OnLoopEnd();

            // If no more characters can be played this loop,
            // move to the next one
            if (!playerManager.CanActivateCharacter())
            {
                NextLoop();
            }

            RestartLoop();
            Freeze();

            state = LoopState.PlayerSelection;
            UI.SetState(UIState.PlayerSelection);

            if (!playerManager.CanActivateCharacter())
            {
                throw new InvalidOperationException("No characters available after incrementing loop. Game should end but thats not done yet.");
            }
        }

        public void Unfreeze()
        {
            // TODO uncomment when adding rigid bodies
            // playerManager.Unfreeze();
            // enemyManager.Unfreeze();
            projectileManager.Unfreeze();
        }

        public void Freeze()
        {
            // playerManager.Freeze();
            // enemyManager.Freeze();
            projectileManager.Freeze();
        }

        // CutLoop sets the loopLength to the current amount that has passed, ending the current cycle of the loop and cutting all players to this length for the round.
        public void CutLoop() {
            if (state == LoopState.ThreadPlaying)
                loopLengthTicks = currentTick;
        }
}
