using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolInteractable : Interactable
{
    public override void OnInteract(PlayerController sender)
    {
        transform.SetParent(sender.HandSlot.transform);
        transform.localPosition = new Vector3(-2f, 1.5f, -1f);
        transform.GetComponent<Rigidbody>().useGravity = false;
        transform.GetComponent<Rigidbody>().isKinematic = true;
        Quaternion rot = new Quaternion();
        rot.eulerAngles = new Vector3(90, 0, 0);
        transform.localRotation = rot;

    }

    public override void OnReleased(PlayerController sender)
    {
        //throw new System.NotImplementedException();
    }

    public override void WhileHeld(PlayerController sender)
    {
        //throw new System.NotImplementedException();
    }
}
