using UnityEngine;

/// <summary>
/// When an <see cref="IInteractable" /> object is interacted with it will be
/// parented to the <see cref="OnInteract(PlayerController)" />'s <see cref="PlayerController.carrySlot" />
/// </summary>
public class MoveableInteractable : MonoBehaviour, IInteractable
{
    /// <summary>
    /// Toggles carrying object
    /// </summary>
    /// <param name="sender"><see cref="PlayerController" /> that called this method</param>
    public void OnInteract(PlayerController sender)
    {
        if (sender.carrySlot.GetComponentsInChildren<MoveableInteractable>().Length > 0)
        {
            transform.parent = null;
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }
        else
        {
            transform.parent = sender.carrySlot.transform;
            transform.localPosition = Vector3.zero;
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    public void OnReleased(PlayerController sender)
    {
    }

    public void WhileHeld(PlayerController sender)
    {
    }
}