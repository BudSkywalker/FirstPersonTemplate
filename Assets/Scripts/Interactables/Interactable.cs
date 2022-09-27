using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains the base functions that can be utilized by all interactable objects
/// </summary>
public abstract class Interactable : MonoBehaviour
{
    /// <summary>
    /// Called when first interacted with
    /// </summary>
    /// <param name="sender"><see cref="PlayerController"/> that called this method</param>
    public abstract void OnInteract(PlayerController sender);
    /// <summary>
    /// Called while interaction is ongoing
    /// </summary>
    /// <param name="sender"><see cref="PlayerController"/> that called this method</param>
    public abstract void WhileHeld(PlayerController sender);
    /// <summary>
    /// Called when interaction is terminated
    /// </summary>
    /// <param name="sender"><see cref="PlayerController"/> that called this method</param>
    public abstract void OnReleased(PlayerController sender);
}
