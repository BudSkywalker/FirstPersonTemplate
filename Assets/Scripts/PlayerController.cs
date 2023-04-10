using System;
using Menu;
using UnityEngine;
using UnityEngine.InputSystem;

// ReSharper disable Unity.InefficientPropertyAccess

/// <summary>
/// Manages the player of the game and handles calls for <see cref="ITool" />s and <see cref="IInteractable" />s
/// </summary>
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public sealed class PlayerController : MonoBehaviour
{
    #region Vars
    #region Refs
    /// <summary>
    /// Unity's built in way to control the player
    /// </summary>
    private CharacterController controller;
    /// <summary>
    /// The Unity camera to act as the player's "eyes"
    /// </summary>
    [SerializeField]
    private Camera playerCamera;
    /// <summary>
    /// Checks to see if there is anything under the player's feet
    /// </summary>
    [SerializeField]
    private SphereCollider groundChecker;
    /// <summary>
    /// Checks to see if there's anything above the player's head
    /// </summary>
    [SerializeField]
    private SphereCollider headChecker;
    /// <summary>
    /// Empty <see cref="GameObject" /> that is the parent of any <see cref="MoveableInteractable" /> that this player carries visually in front of them
    /// </summary>
    public GameObject carrySlot;
    /// <summary>
    /// Empty <see cref="GameObject" /> that is the parent of any <see cref="ITool" /> the player wields
    /// </summary>
    public GameObject handSlot;
    /// <summary>
    /// The <see cref="LayerMask" /> level to use to determine what can be collided with
    /// </summary>
    [SerializeField]
    private LayerMask collisionMask;
    /// <summary>
    /// Unity's new input system attached to this GameObject
    /// </summary>
    private PlayerInput input;
    #endregion

    #region Player Info
    /// <summary>
    /// How fast does the player move while walking in meters per second
    /// </summary>
    [Header("About Player")]
    [Tooltip("Player Speed in M/S")]
    [SerializeField]
    private float playerSpeed = 3.5f;
    /// <summary>
    /// What number to multiply to the <see cref="playerSpeed" /> to calculate sprinting speed
    /// </summary>
    [SerializeField]
    private float sprintModifier = 2;
    /// <summary>
    /// How high the player can jump in meters
    /// </summary>
    [SerializeField]
    private float jumpHeight = 0.7f;
    /// <summary>
    /// What is the gravity
    /// </summary>
    [SerializeField]
    private float gravity = 9.81f;
    /// <summary>
    /// What number to multiply the player's <see cref="controller" />'s height by when crouching
    /// </summary>
    [SerializeField]
    private float crouchModifier = 0.5f;
    /// <summary>
    /// The player's mass in kilograms
    /// </summary>
    [Tooltip("Mass in KG")]
    [SerializeField]
    private float mass = 70;
    /// <summary>
    /// How the the player can reach in meters
    /// </summary>
    [SerializeField]
    private float reach = 10;
    #endregion

    #region Input
    //protected
    /// <summary>
    /// <see cref="playerCamera" />'s pitch
    /// </summary>
    private float pitch;
    /// <summary>
    /// <see cref="playerCamera" />'s field of view
    /// </summary>
    private float fov;
    /// <summary>
    /// Player's current vertical velocity
    /// </summary>
    private float velocity;
    /// <summary>
    /// The player's terminal velocity. This is calculated in <see cref="Start" /> using the actual equation for terminal velocity
    /// </summary>
    private float tVelocity;
    /// <summary>
    /// A place holder for the character's height to reset <see cref="controller" />'s height back to after crouching
    /// </summary>
    private float height;
    /// <summary>
    /// A place holder for the character's step offset to reset <see cref="controller" />'s stepOffset after jumping
    /// </summary>
    private float stepOffset;
    /// <summary>
    /// Used to manage Interact keystrokes
    /// </summary>
    private bool interactDown;
    /// <summary>
    /// Used to manage Primary Action keystrokes
    /// </summary>
    private bool action1Down;
    /// <summary>
    /// Used to manage Secondary Action keystrokes
    /// </summary>
    private bool action2Down;
    /// <summary>
    /// Tracks crouch state when <see cref="GameplaySettings.toggleCrouch" /> is true
    /// </summary>
    private bool cToggle;
    /// <summary>
    /// Used to toggle the crouch state when
    /// </summary>
    private bool cPress;
    /// <summary>
    /// Tracks sprint state when <see cref="GameplaySettings.toggleSprint" /> is true
    /// </summary>
    private bool sToggle;
    /// <summary>
    /// Used to toggle the sprint state when
    /// </summary>
    private bool sPress;
    /// <summary>
    /// Used to track what object is currently being interacted with
    /// </summary>
    private IInteractable interactingObject;
    /// <summary>
    /// Used to track what tool the player is currently using
    /// </summary>
    private ITool heldTool;
    #endregion

    #region Game specific
    public int Health { get; set; }
    #endregion
    #endregion

    #region Unity Funcs
    /// <summary>
    /// Unity's Awake function
    /// </summary>
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        input = GetComponent<PlayerInput>();
        Settings.GetSettings().keybindSettings.LoadOverrides(ref input);
    }

    /// <summary>
    /// Unity's Start function
    /// </summary>
    private void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        fov = playerCamera.fieldOfView;
        tVelocity = Mathf.Sqrt(2 * mass * gravity / 0.8575f * Mathf.PI * Mathf.Pow(controller.radius, 2));
        height = controller.height;
        stepOffset = controller.stepOffset;

        controller.detectCollisions = true;
    }

    /// <summary>
    /// Unity's Update function
    /// </summary>
    private void Update()
    {
        Look();
        Crouch();
        HandleMovement();
        HandleTool();
    }

    /// <summary>
    /// Unity's FixedUpdate function
    /// </summary>
    private void FixedUpdate()
    {
        if (PauseMenu.IsPaused) return;

        Jump();

        Interact();
    }
    #endregion

    #region Custom Funcs
    /// <summary>
    /// Handles camera movement and looking around. Called on <see cref="Update" />
    /// </summary>
    private void Look()
    {
        if (PauseMenu.IsPaused) return;

        float lookX = input.actions.FindAction("Look").ReadValue<Vector2>().x;
        float lookY = input.actions.FindAction("Look").ReadValue<Vector2>().y;

        switch (input.currentControlScheme)
        {
            case "Mouse Controls":
                lookX *= Settings.GetSettings().gameplaySettings.mouseSensitivity;
                lookY *= Settings.GetSettings().gameplaySettings.mouseSensitivity;
                break;
            case "Controller Controls":
                lookX *= Settings.GetSettings().gameplaySettings.controllerSensitivity;
                lookY *= Settings.GetSettings().gameplaySettings.controllerSensitivity;
                break;
        }

        if (Settings.GetSettings().gameplaySettings.invertY) lookY *= -1;

        pitch = Mathf.Clamp(pitch - lookY, -90, 90);

        playerCamera.transform.localRotation = Quaternion.Euler(pitch, 0, 0);
        transform.Rotate(Vector3.up * lookX);
    }

    /// <summary>
    /// Handles crouching. Called on <see cref="Update" />
    /// </summary>
    private void Crouch()
    {
        if (Settings.GetSettings().gameplaySettings.toggleCrouch)
        {
            if (input.actions.FindAction("Crouch").IsPressed())
            {
                if (!cPress)
                {
                    cToggle = !cToggle;
                    cPress = true;
                }
            }
            else
            {
                cPress = false;
            }

            if (cToggle)
            {
                controller.height = height * crouchModifier;
                controller.center = new(0, -controller.height / 2, 0);
                playerCamera.transform.localPosition = new(0, controller.center.y + controller.height / 2 - controller.height * 0.125f, 0);
            }
            else if (!Physics.CheckSphere(headChecker.transform.position, headChecker.radius, collisionMask))
            {
                controller.height = height;
                controller.center = new(0, 0, 0);
                playerCamera.transform.localPosition = new(0, controller.height / 2 - controller.height * 0.125f, 0);
            }
        }
        else
        {
            if (input.actions.FindAction("Crouch").IsPressed())
            {
                controller.height = height * crouchModifier;
                controller.center = new(0, -controller.height / 2, 0);
                playerCamera.transform.localPosition = new(0, controller.center.y + controller.height / 2 - controller.height * 0.125f, 0);
            }
            else if (!Physics.CheckSphere(headChecker.transform.position, headChecker.radius, collisionMask))
            {
                controller.height = height;
                controller.center = new(0, 0, 0);
                playerCamera.transform.localPosition = new(0, controller.height / 2 - controller.height * 0.125f, 0);
            }
        }
    }

    /// <summary>
    /// Handles sprinting and generic movement. Called on <see cref="Update" />
    /// </summary>
    private void HandleMovement()
    {
        if (Settings.GetSettings().gameplaySettings.toggleSprint)
        {
            if (input.actions.FindAction("Sprint").IsPressed())
            {
                if (!sPress)
                {
                    sToggle = !sToggle;
                    sPress = true;
                }
            }
            else
            {
                sPress = false;
            }

            Move(sToggle);
        }
        else
        {
            Move(input.actions.FindAction("Sprint").IsPressed());
        }
    }

    /// <summary>
    /// Helper function for <see cref="HandleMovement" />
    /// </summary>
    /// <param name="sprinting">Whether to add <see cref="sprintModifier" /> or not</param>
    private void Move(bool sprinting)
    {
        Vector2 movementInput = input.actions.FindAction("Move").ReadValue<Vector2>() * playerSpeed;

        if (sprinting)
        {
            controller.Move(transform.forward * (movementInput.y * sprintModifier * Time.deltaTime) +
                            transform.right * (movementInput.x * sprintModifier * Time.deltaTime));
            if (Settings.GetSettings().gameplaySettings.fovModifier && playerCamera.fieldOfView < fov * (1 + sprintModifier * 0.1f))
                playerCamera.fieldOfView += 0.5f;
            if (playerCamera.fieldOfView > fov) playerCamera.fieldOfView = fov;
        }
        else
        {
            controller.Move(transform.forward * (movementInput.y * Time.deltaTime) + transform.right * (movementInput.x * Time.deltaTime));
            if (playerCamera.fieldOfView > fov) playerCamera.fieldOfView -= 0.5f;
            if (playerCamera.fieldOfView < fov) playerCamera.fieldOfView = fov;
        }
    }

    /// <summary>
    /// Handles what happens when there's a tool in the hand. Called on <see cref="Update" />
    /// </summary>
    /// <seealso cref="PrimaryAction" />
    /// <seealso cref="SecondaryAction" />
    private void HandleTool()
    {
        //TODO: Multiple hands/inventory system

        heldTool = handSlot.GetComponentInChildren<ITool>();

        if (heldTool == null) return;

        PrimaryAction();
        SecondaryAction();
    }

    /// <summary>
    /// Handles what happens when the primary action is activated. Called by <see cref="HandleTool" />
    /// </summary>
    /// <seealso cref="SecondaryAction" />
    private void PrimaryAction()
    {
        if (input.actions.FindAction("Primary Action").IsPressed() && !action1Down)
        {
            action1Down = true;

            heldTool.OnPrimaryFire(this);
        }
        else if (input.actions.FindAction("Primary Action").IsPressed() && action1Down)
        {
            heldTool.OnPrimaryHeld(this);
        }
        else if (!input.actions.FindAction("Primary Action").IsPressed() && action1Down)
        {
            heldTool.OnPrimaryRelease(this);
            action1Down = false;
        }
    }

    /// <summary>
    /// Handles what happens when the secondary action is activated. Called by <see cref="HandleTool" />
    /// </summary>
    /// <seealso cref="PrimaryAction" />
    private void SecondaryAction()
    {
        if (input.actions.FindAction("Secondary Action").IsPressed() && !action2Down)
        {
            action2Down = true;

            heldTool.OnSecondaryFire(this);
        }
        else if (input.actions.FindAction("Secondary Action").IsPressed() && action2Down)
        {
            heldTool.OnSecondaryHeld(this);
        }
        else if (!input.actions.FindAction("Secondary Action").IsPressed() && action2Down)
        {
            heldTool.OnSecondaryRelease(this);
            action2Down = false;
        }
    }

    /// <summary>
    /// Handles jumping and also gravity. Called on <see cref="FixedUpdate" />
    /// </summary>
    private void Jump()
    {
        if (input.actions.FindAction("Jump").IsPressed() &&
            Physics.CheckSphere(groundChecker.transform.position, groundChecker.radius, collisionMask))
        {
            velocity = Mathf.Sqrt(jumpHeight * 2 * gravity);
            controller.stepOffset = 0.001f;
        }
        else if (!Physics.CheckSphere(groundChecker.transform.position, groundChecker.radius, collisionMask))
        {
            if (velocity > -tVelocity) velocity -= gravity * Time.fixedDeltaTime;
        }
        else
        {
            velocity = -0.1f;
            controller.stepOffset = stepOffset;
        }

        controller.Move(new(0, velocity * Time.fixedDeltaTime, 0));
    }

    /// <summary>
    /// Handles calling <see cref="IInteractable" />. Called on <see cref="FixedUpdate" />
    /// </summary>
    private void Interact()
    {
        if (Physics.Linecast(playerCamera.transform.position, playerCamera.transform.position + playerCamera.transform.forward * reach / 8,
                out RaycastHit hit, collisionMask))
            carrySlot.transform.position = hit.point;
        else
            carrySlot.transform.position = playerCamera.transform.position + playerCamera.transform.forward * reach / 8;

        foreach (Transform t in carrySlot.GetComponentsInChildren<Transform>()) t.rotation = playerCamera.transform.rotation;

        IInteractable child = carrySlot.GetComponentInChildren<IInteractable>();
        if (input.actions.FindAction("Interact").IsPressed() && !interactDown)
        {
            interactDown = true;

            if (Physics.Linecast(playerCamera.transform.position, playerCamera.transform.position + playerCamera.transform.forward * reach,
                    out RaycastHit info))
            {
                interactingObject = info.transform.GetComponent<IInteractable>();
                interactingObject?.OnInteract(this);
            }
            else
            {
                child?.OnInteract(this);
            }
        }
        else if (input.actions.FindAction("Interact").IsPressed() && interactDown)
        {
            if (interactingObject != null)
                interactingObject.WhileHeld(this);
            else
                child?.OnInteract(this);
        }
        else if (!input.actions.FindAction("Interact").IsPressed() && interactDown)
        {
            if (interactingObject != null)
                interactingObject.OnReleased(this);
            else child?.OnInteract(this);
            interactingObject = null;

            interactDown = false;
        }
    }
    #endregion
}