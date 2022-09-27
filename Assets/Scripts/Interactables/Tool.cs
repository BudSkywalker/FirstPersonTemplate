using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains the base functions that can be utilized by all tools
/// </summary>
public abstract class Tool : MonoBehaviour
{
    /// <summary>
    /// The amount of damage the tool will deal
    /// </summary>
    public int Damage;

    /// <summary>
    /// Called when primary action is first clicked
    /// </summary>
    /// <param name="sender"><see cref="PlayerController"/> that called this method</param>
    public abstract void OnPrimaryFire(PlayerController sender);
    /// <summary>
    /// Called when primary action is held down
    /// </summary>
    /// <param name="sender"><see cref="PlayerController"/> that called this method</param>
    public abstract void OnPrimaryHeld(PlayerController sender);
    /// <summary>
    /// Called when primary action is released
    /// </summary>
    /// <param name="sender"><see cref="PlayerController"/> that called this method</param>
    public abstract void OnPrimaryRelease(PlayerController sender);

    /// <summary>
    /// Called when secondary action is first clicked
    /// </summary>
    /// <param name="sender"><see cref="PlayerController"/> that called this method</param>f
    public abstract void OnSecondaryFire(PlayerController sender);
    /// <summary>
    /// Called when secondary action is held down
    /// </summary>
    /// <param name="sender"><see cref="PlayerController"/> that called this method</param>
    public abstract void OnSecondaryHeld(PlayerController sender);
    /// <summary>
    /// Called when secondary action is released
    /// </summary>
    /// <param name="sender"><see cref="PlayerController"/> that called this method</param>
    public abstract void OnSecondaryRelease(PlayerController sender);
}
