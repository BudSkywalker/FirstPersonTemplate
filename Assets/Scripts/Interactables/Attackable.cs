using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains the base functions for any object that can by attacked by a <see cref="Tool"/>
/// </summary>
public abstract class Attackable : MonoBehaviour
{
    /// <summary>
    /// Health of the object
    /// </summary>
    public int Health;

    /// <summary>
    /// Called when primary action effects this object
    /// </summary>
    /// <param name="attacker">The tool that attacked this object</param>
    public abstract void OnHit(Tool attacker);
}
