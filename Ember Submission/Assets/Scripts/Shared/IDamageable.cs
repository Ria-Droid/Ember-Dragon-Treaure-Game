using UnityEngine;

//Shared contract for anything that can take damage: Kael and every enemy type.
public interface IDamageable
{
    //Apply damage 
    /// <param name="amount">How much damage/how many "hits" this represents.</param>
    void TakeHit(int amount);

    //Current health
    int CurrentHealth { get; }

    //Whether this thing has died.
    bool IsDead { get; }
}
