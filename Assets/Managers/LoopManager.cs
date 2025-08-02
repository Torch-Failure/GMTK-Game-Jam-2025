using System;
using UnityEngine;

public class LoopManager : MonoBehaviour
{
        
        enum LoopState
        {
            // When the u
            PlayerSelection,
            ThreadPlaying
        };

        public int loopLengthTicks = 1000; // How long each loop is
        private int currentTick = 0; // Current tick in this thread of the loop
        public PlayerManager playerManager;

        private LoopState state = LoopState.PlayerSelection;

        // Restores everything to where it was at the start of the current loop
        // Player history should be preserved
        public void RestartLoop()
        {   
            // This can eventually operate on a list of common interfaces/super classes
            playerManager.LoadLoopStart();
            // Should reset loop on every manager or entity
            playerManager.RestartLoop();
            currentTick = 0;
        }

        private void HandleLoopTransistion()
        {
            // If no more characters can be played this loop,
            // move to the next one
            if (!playerManager.CanActivateCharacter())
            {
                IncrementLoop();
            }

            RestartLoop();
            state = LoopState.PlayerSelection;

            if (!playerManager.CanActivateCharacter())
            {
                throw new InvalidOperationException("No characters available after incrementing loop. Game should end but thats not done yet.");
            }
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
                    currentTick++;
                    if (currentTick >= loopLengthTicks)
                    {
                        HandleLoopTransistion();
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
                        state = LoopState.ThreadPlaying;
                    }
                    break;
                case LoopState.ThreadPlaying:
                    // Player is playing as a character
                    playerManager.ThreadPlayingUpdate();
                    break;
            }
        }

        // Will move to next loop
        void IncrementLoop()
        {
            Debug.Log("Incrementing looop.....");
            playerManager.IncrementLoop();
            playerManager.SaveLoopStart();
        }
}
