
using HeartOfIron.Structs;

/// <summary>
/// Defines an interface for objects that can take damage and be repaired.
/// Implementing classes should handle damage application and repair logic.
/// </summary>
public interface IDamageable
{
    /// <summary>
    /// Applies damage to the object at the specified location.
    /// </summary>
    /// <param name="damageAmount">The amount of damage to apply.</param>
    /// <param name="location">The location on the object where the damage is applied.</param>
    /// <returns>A report detailing the damage taken.</returns>
    DamageReport TakeDamage(float damageAmount, int location = 1);

    /// <summary>
    /// Repairs the object at the specified location.
    /// </summary>
    /// <param name="repairAmount">The amount of repair to apply.</param>
    /// <param name="location">The location on the object where the repair is applied.</param>
    void Repair(float repairAmount, int location = 1);
}
