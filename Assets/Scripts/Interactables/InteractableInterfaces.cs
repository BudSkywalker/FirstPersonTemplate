public interface IInteractable
{
    /// <summary>
    /// Called when first interacted with
    /// </summary>
    /// <param name="sender"><see cref="PlayerController" /> that called this method</param>
    void OnInteract(PlayerController sender);

    /// <summary>
    /// Called while interaction is ongoing
    /// </summary>
    /// <param name="sender"><see cref="PlayerController" /> that called this method</param>
    void WhileHeld(PlayerController sender);

    /// <summary>
    /// Called when interaction is terminated
    /// </summary>
    /// <param name="sender"><see cref="PlayerController" /> that called this method</param>
    void OnReleased(PlayerController sender);
}

/// <summary>
/// Contains the base functions for any object that can by attacked by a <see cref="ITool" />
/// </summary>
public interface IAttackable
{
    /// <summary>
    /// Health of the object
    /// </summary>
    public int Health { get; set; }

    /// <summary>
    /// Called when primary action effects this object
    /// </summary>
    /// <param name="attacker">The tool that attacked this object</param>
    public void OnHit(ITool attacker);
}

public interface ITool
{
    /// <summary>
    /// The amount of damage the tool will deal
    /// </summary>
    public int Damage { get; set; }

    /// <summary>
    /// Called when primary action is first clicked
    /// </summary>
    /// <param name="sender"><see cref="PlayerController" /> that called this method</param>
    void OnPrimaryFire(PlayerController sender);

    /// <summary>
    /// Called when primary action is held down
    /// </summary>
    /// <param name="sender"><see cref="PlayerController" /> that called this method</param>
    void OnPrimaryHeld(PlayerController sender);

    /// <summary>
    /// Called when primary action is released
    /// </summary>
    /// <param name="sender"><see cref="PlayerController" /> that called this method</param>
    void OnPrimaryRelease(PlayerController sender);

    /// <summary>
    /// Called when secondary action is first clicked
    /// </summary>
    /// <param name="sender"><see cref="PlayerController" /> that called this method</param>
    /// f
    void OnSecondaryFire(PlayerController sender);

    /// <summary>
    /// Called when secondary action is held down
    /// </summary>
    /// <param name="sender"><see cref="PlayerController" /> that called this method</param>
    void OnSecondaryHeld(PlayerController sender);

    /// <summary>
    /// Called when secondary action is released
    /// </summary>
    /// <param name="sender"><see cref="PlayerController" /> that called this method</param>
    void OnSecondaryRelease(PlayerController sender);
}