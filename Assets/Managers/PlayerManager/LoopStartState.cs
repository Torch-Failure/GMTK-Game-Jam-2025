using System.Collections.Generic;
using UnityEngine;


namespace PlayerManager {
    struct LoopStartState
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
}