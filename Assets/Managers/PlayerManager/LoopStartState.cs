using System.Collections.Generic;
using UnityEngine;


namespace PlayerManager {
    struct LoopStartState
    {
        // Need to store state of characters at start of loop
        // For now, just transform
        public List<Vector3> characterPositions;
        public List<Quaternion> characterRotations;
        public List<Character.CharacterState> characterStates; 

        // public void()
        // {
            // characterPositions = new();
            // characterRotations = new();
            // characterStates = new();
        // }

        public void SaveState(List<CharacterThread> threads)
        { 
            characterPositions.Clear();
            characterRotations.Clear();
            characterStates.Clear();

            foreach (var thread in  threads)
            {
                var character = thread.threadCharacter;
                characterPositions.Add(character.transform.position);
                characterRotations.Add(character.transform.rotation);
                characterStates.Add(character.state);
            }
        }

        public void LoadState(List<CharacterThread> threads)
        {
            for (int i = 0; i < threads.Count; i++)
            {
                var character = threads[i].threadCharacter;
                character.transform.position = characterPositions[i];
                character.transform.rotation = characterRotations[i];

                Debug.Log($"Character state is: {character.state}");
                Debug.Log($"Saved state is: {characterStates[i]}");


                if (characterStates[i] == Character.CharacterState.Alive && character.state == Character.CharacterState.Dead)
                {
                    Debug.Log("Undied...");
                    character.UnDie();
                }
            }
        }
    }
}