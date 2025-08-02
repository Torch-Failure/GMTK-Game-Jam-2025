using UnityEngine;


namespace PlayerManager {
    struct PlayerInputRecord
    {
        public Vector2 move;
        public Quaternion rotation;
        public bool doesAttack;
    }
}