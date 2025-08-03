using UnityEngine;

public abstract class AttackComponent : MonoBehaviour
{
    [SerializeField]
    public int attackSpeedTicks; // Number of ticks per attack
    
    public int ticksTillNextAttack = 0; // Can attack if this is zero
    
    public abstract void Attack(Quaternion direction);

    public void ThreadPlayingFixedUpdate()
    {
        ticksTillNextAttack--;
        if (ticksTillNextAttack < 0)
        {
            ticksTillNextAttack = 0;
        }
    }
}