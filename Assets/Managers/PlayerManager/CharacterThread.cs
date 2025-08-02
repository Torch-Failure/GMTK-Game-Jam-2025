// Stores history of one character for the current loop
using System.Collections.Generic;


namespace PlayerManager {
    class CharacterThread
    {
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

        // Returns true if this thread can be selected in a character selection phase
        // First implementation, if thread ticks are zero?
        public bool CanActivate()
        {
            return threadTicks == 0;
        }
    }
}