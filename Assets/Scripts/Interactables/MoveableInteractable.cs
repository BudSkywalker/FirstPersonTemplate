using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// When an <see cref="Interactable"/> object is interacted with it will be
/// parented to the <see cref="OnInteract(PlayerController)"/>'s <see cref="PlayerController.CarrySlot"/>
/// </summary>
public class MoveableInteractable : Interactable
{    
    /// <summary>
    /// Toggles carrying object
    /// </summary>
    /// <param name="sender"><see cref="PlayerController"/> that called this method</param>
    public override void OnInteract(PlayerController sender)
    {
        if (sender.CarrySlot.GetComponentsInChildren<MoveableInteractable>().Length > 0)
        {
            transform.parent = null;
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }
        else
        {
            transform.parent = sender.CarrySlot.transform;
            transform.localPosition = Vector3.zero;
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    /// <summary>
    /// Unused
    /// </summary>
    /// <param name="sender"><see cref="PlayerController"/> that called this method</param>
    public override void OnReleased(PlayerController sender)
    {
        
    }

    /// <summary>
    /// Unused
    /// </summary>
    /// <param name="sender"><see cref="PlayerController"/> that called this method</param>
    public override void WhileHeld(PlayerController sender)
    {

    }
}
