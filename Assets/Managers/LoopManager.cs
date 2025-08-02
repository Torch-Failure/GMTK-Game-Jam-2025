using System;
using PlayerManager;
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
        public PlayerManager.PlayerManager playerManager;

        private LoopState state = LoopState.PlayerSelection;

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

        // Restores everything to where it was at the start of the current loop
        // Player history should be preserved
        public void RestartLoop()
        {   
            // This can eventually operate on a list of common interfaces/super classes
            playerManager.LoadLoopStart();
            currentTick = 0;
        }

        // Will move to next loop
        void NextLoop()
        {
            Debug.Log("Next looop.....");
            playerManager.NextLoop();
            playerManager.SaveLoopStart();
        }

        private void HandleLoopEnd()
        {
            // If no more characters can be played this loop,
            // move to the next one
            if (!playerManager.CanActivateCharacter())
            {
                NextLoop();
            }

            RestartLoop();
            state = LoopState.PlayerSelection;
            UI.SetState(UIState.PlayerSelection);

            if (!playerManager.CanActivateCharacter())
            {
                throw new InvalidOperationException("No characters available after incrementing loop. Game should end but thats not done yet.");
            }
        }
}
