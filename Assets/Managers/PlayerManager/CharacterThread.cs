// Stores history of one character for the current loop
using System;
using System.Collections.Generic;

namespace PlayerManager {
    class CharacterThread
    {
        public List<PlayerInputRecord> history;
        
        // How much time has been played out in this thread
        public uint threadTicks; 

        public Character threadCharacter;

        public enum ThreadState { 
                                  Unplayed, // Has not yet been touched by human hands this loop
                                  Active, // This character is active
                                  Inactive, // Has been active and died TODO rename to something slightly descriptive of what it is
                                  Complete, // Completed loop

                                  PermenantlyDead // Dead as dead can be, died and cannot be brought back 
                                };

        private int limitingDeathTick; // This is the tick which, if survived, will allow a replay
        private int lastLoopDeathTick; // The last loop death tick, if this is greater than the limiting tick we can replay

        public ThreadState state = ThreadState.Unplayed;

        public CharacterThread(Character threadChar)
        {
            history = new();
            threadCharacter = threadChar;
            Reset();
        }        

        // This records info about death timings so that, when a character is saved
        // that information can be tracked and acted on
        public void SetDeathInfo(int currentTick)
        {
            lastLoopDeathTick = currentTick;

            // If we died while active, update the limiting tick
            if (state == ThreadState.Active)
            {
                // If we are active and die, this is the furthest we have
                // got 'for realsies' (meaning while under player control)
                // This is the tick to beat!
                limitingDeathTick = Math.Max(currentTick, limitingDeathTick);
            }
        }

        public void PushHistory(PlayerInputRecord record)
        {
            history.Add(record);
            threadTicks++;
        }

        // This is relevant for threads which died previously and are now being replayed
        public bool IsUnderPlayerControl(int currentTick)
        {
            if (state != ThreadState.Active)
            {
                throw new InvalidOperationException("Should only be called on active threads!");
            }

            // If we are still at a point in history before the limiting death
            // we are still replaying history.
            // Once we get past that point, the player will take over
            if (currentTick >= limitingDeathTick)
            {
                return true;            
            }
            return false;
        }

        public void Reset()
        {
            history.Clear();
            threadTicks = 0;
            limitingDeathTick = 0;
            lastLoopDeathTick = 0;

            // We latch into permenant death state
            if (state != ThreadState.PermenantlyDead)
            {
                state = ThreadState.Unplayed;
            }
        }

        // Returns true if this thread can be selected in a character selection phase
        public bool CanActivate()
        {
            // If thread hasn't been played yet, we can always select them
            if (state == ThreadState.Unplayed)
            {
                return true;
            }

            // Else, if we have died we can activate if our last death was later than the limiting death tick
            // TODO this is a bit odd since you could save them one looop, choose not to play them, then they die earlier
            //      and now you have 'wasted' that saving
            if (state == ThreadState.Inactive && limitingDeathTick < lastLoopDeathTick)
            {
                return true;
            } 

            return false;
        }
    }
}