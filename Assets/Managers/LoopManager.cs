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
        public PlayerManager playerManager;

        private LoopState state = LoopState.ThreadPlaying;


        void Start()
        {
            playerManager.loopLengthTicks = loopLengthTicks;
        }

        // Need to reset loop when:
        //  - Active thread is killed
        //  - Active thread length reaches loopLengthSeconds

        // Need to move to next loop when:
        //  - 

        // Restores everything to where it was at the start of the loop
        // Player history should be preserved
        public void RestartLoop()
        {
            // Should reset loop on every manager or entity
            playerManager.RestartLoop();

        }

        // Should dispatch updates to pretty much everything else in the game depending on state
        public void InternalFixedUpdate()
        {
            switch(state)
            {
                case LoopState.PlayerSelection:
                    // Player is selecting next character to play as
                    playerManager.CharacterSelectionUpdate();
                    break;
                case LoopState.ThreadPlaying:
                    // Player is playing as a character
                    playerManager.ThreadPlayingUpdate();
                    break;
            }
        }

        public void InternalUpdate()
        {
            switch(state)
            {
                case LoopState.PlayerSelection:
                    // Player is selecting next character to play as
                    break;
                case LoopState.ThreadPlaying:
                    // Player is playing as a character
                    playerManager.InternalUpdate();
                    break;
            }
        }

        // What calls this?
        // Also the player manager?
        void IncrementLoop()
        {
            // Store previous start of loop, update starting point
            // playerManager.setLoopStart();
        }

        void EndGame()
        {

        }
}
