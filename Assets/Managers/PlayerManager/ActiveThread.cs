
using UnityEngine;

namespace PlayerManager {
    // Helper class for rendering active character in red
    class ActiveThread
    {
        private CharacterThread _activeThread;
        public CharacterThread activeThread
        {
            get  => _activeThread;
            set
            {
                if (_activeThread != null)
                {
                    _activeThread.threadCharacter.GetComponent<SpriteRenderer>().color = _activeThread.threadCharacter.GetCurrentColor();
                }
                value.threadCharacter.GetComponent<SpriteRenderer>().color = Color.red;
                _activeThread = value;
            }
        }
    }
}
