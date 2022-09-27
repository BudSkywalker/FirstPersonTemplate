using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the player of the game and handles calls for <see cref="Tool"/>s and <see cref="Interactable"/>s
/// </summary>
/// <seealso cref="Tool"/>
/// <seealso cref="Interactable"/>
/// <seealso cref="Attackable"/>
public class PlayerController : MonoBehaviour
{
    #region Vars
    #region Refs
    /// <summary>
    /// Unity's built in way to controls the player
    /// </summary>
    [SerializeField]
    private CharacterController Controller;
    /// <summary>
    /// The Unity camera to act as the player's "eyes"
    /// </summary>
    [SerializeField]
    private Camera PlayerCamera;
    /// <summary>
    /// Checks to see if there is anything under the player's feet
    /// </summary>
    [SerializeField]
    private SphereCollider GroundChecker;
    /// <summary>
    /// Checks to see if there's anything above the player's head
    /// </summary>
    [SerializeField]
    private SphereCollider HeadChecker;
    /// <summary>
    /// Empty <see cref="GameObject"/> that is the parent of any <see cref="MoveableInteractable"/> that this player carries visually in front of them
    /// </summary>
    public GameObject CarrySlot;
    /// <summary>
    /// Empty <see cref="GameObject"/> that is the parent of any <see cref="Tool"/> the player weilds
    /// </summary>
    public GameObject HandSlot;
    /// <summary>
    /// The <see cref="LayerMask"/> level to use to determine what is collidable
    /// </summary>
    [SerializeField]
    private LayerMask CollisionMask;
    #endregion

    #region Player Info
    [Header("About Player")]
    /// <summary>
    /// How fast does the player move while walking in meters per second
    /// </summary>
    [Tooltip("Player Speed in M/S")]
    [SerializeField]
    private float PlayerSpeed = 3.5f;
    /// <summary>
    /// What number to multiply to the <see cref="PlayerSpeed"/> to calculate sprinting speed
    /// </summary>
    [SerializeField]
    private float SprintModifier = 2;
    /// <summary>
    /// How high the player can jump in meters
    /// </summary>
    [SerializeField]
    private float JumpHeight = 0.7f;
    /// <summary>
    /// What is the gravity
    /// </summary>
    [SerializeField]
    private float Gravity = 9.81f;
    /// <summary>
    /// What number to multiply the player's <see cref="Controller.height"/> by when crouching
    /// </summary>
    [SerializeField]
    private float CrouchModifier = 0.5f;
    /// <summary>
    /// The player's mass in kilograms
    /// </summary>
    [Tooltip("Mass in KG")]
    [SerializeField]
    private float Mass = 70;
    /// <summary>
    /// How the the player can reach in meters
    /// </summary>
    [SerializeField]
    private float Reach = 10;
    #endregion

    #region Settings
    [Header("Settings")]
    /// <summary>
    /// What to multiply the mouse input by when looking around
    /// </summary>
    private float MouseSensativity;
    /// <summary>
    /// What to multiply the controller input by when looking around
    /// </summary>
    private float ControllerSensativity;
    /// <summary>
    /// Whether crouching should be trigger by holding Crouch or tapping Crouch
    /// </summary>
    private bool ToggleCrouch;
    /// <summary>
    /// Whether sprinting should be trigger by holding Sprint or tapping Sprint
    /// </summary>
    private bool ToggleSprint;
    /// <summary>
    /// Inverts the Y control
    /// </summary>
    private bool InvertY;
    /// <summary>
    /// Should the <see cref="PlayerCamera"/>'s FOV move when sprinting (can cause motion sickness)
    /// </summary>
    [SerializeField]
    private bool FOVModifier = true;
    #endregion

    #region Input
    //protected
    /// <summary>
    /// <see cref="PlayerCamera"/>'s pitch
    /// </summary>
    protected float pitch;
    /// <summary>
    /// <see cref="PlayerCamera"/>'s field of view
    /// </summary>
    protected float fov;
    /// <summary>
    /// Player's current verticle velocity
    /// </summary>
    protected float velocity;
    /// <summary>
    /// The player's terminal velocity. This is calculated in <see cref="Start"/> using the actual equation for terminal velocity
    /// </summary>
    protected float tVelo;
    /// <summary>
    /// A place holder for the character's height to reset <see cref="Controller.height"/> back to after crouching
    /// </summary>
    protected float height;
    /// <summary>
    /// A place holder for the character's setp offset to reset <see cref="Controller.stepOffset"/> after jumping
    /// </summary>
    protected float stepOffset;
    /// <summary>
    /// Used to manage Interact keystrokes
    /// </summary>
    protected bool interactDown;
    /// <summary>
    /// Used to manage Primary Action keystrokes
    /// </summary>
    protected bool action1Down;
    /// <summary>
    /// Used to manage Secondary Action keystrokes
    /// </summary>
    protected bool action2Down;
    /// <summary>
    /// Tracks crouch state when <see cref="ToggleCrouch"/> is true
    /// </summary>
    protected bool cToggle;
    /// <summary>
    /// Used to toggle the crouch state when
    /// </summary>
    protected bool cPress;
    /// <summary>
    /// Tracks sprint state when <see cref="ToggleSprint"/> is true
    /// </summary>
    protected bool sToggle;
    /// <summary>
    /// Used to toggle the sprint state when
    /// </summary>
    protected bool sPress;
    /// <summary>
    /// Used to track what object is currently being interacted with
    /// </summary>
    protected Interactable interactingObject;
    /// <summary>
    /// Used to track what tool the player is currently using
    /// </summary>
    protected Tool heldTool;
    #endregion

    #region Game specific
    #endregion
    #endregion

    #region Unity Funcs
    /// <summary>
    /// Unity's Start function
    /// </summary>
    protected void Start()
    {
        UpdateSettings();
        Cursor.lockState = CursorLockMode.Locked;
        fov = PlayerCamera.fieldOfView;
        tVelo = Mathf.Sqrt(2 * Mass * Gravity / 0.8575f * Mathf.PI * Mathf.Pow(Controller.radius, 2));
        height = Controller.height;
        stepOffset = Controller.stepOffset;

        Controller.detectCollisions = true;
    }

    /// <summary>
    /// Unity's Update function
    /// </summary>
    protected void Update()
    {
        Look();
        Crouch();
        Sprint();
        HandleTool();
    }

    /// <summary>
    /// Unity's FixedUpdate function
    /// </summary>
    protected void FixedUpdate()
    {
        if (PauseMenu.isPaused) return;

        Jump();

        Interact();
    }
    #endregion

    #region Funcs
    /// <summary>
    /// Updates the settings based on user input
    /// </summary>
    public void UpdateSettings()
    {
        MouseSensativity = PlayerPrefs.GetFloat("Mouse Sensativity", 5);
        ControllerSensativity = PlayerPrefs.GetFloat("Controller Sensativity", 15);
        ToggleCrouch = PlayerPrefs.GetInt("Toggle Crouch", 0) == 1;
        ToggleSprint = PlayerPrefs.GetInt("Toggle Sprint", 0) == 1;
        InvertY = PlayerPrefs.GetInt("Invert Y", 0) == 1;
    }

    /// <summary>
    /// Handles camera movement and looking around. Called on <see cref="Update"/>
    /// </summary>
    protected virtual void Look()
    {
		if (!PauseMenu.isPaused)
		{
            float lookX = (Input.GetAxis("Mouse X") * MouseSensativity) + (Input.GetAxis("Controller X") * ControllerSensativity);
            float lookY = (Input.GetAxis("Mouse Y") * MouseSensativity) + (Input.GetAxis("Controller Y") * ControllerSensativity);
            if (InvertY) lookY *= -1;

            pitch = Mathf.Clamp(pitch - lookY, -90, 90);

            PlayerCamera.transform.localRotation = Quaternion.Euler(pitch, 0, 0);
            transform.Rotate(Vector3.up * lookX);
        }
    }

    /// <summary>
    /// Handles crouching. Called on <see cref="Update"/>
    /// </summary>
    protected virtual void Crouch()
    {
        if (ToggleCrouch)
        {
            if (Input.GetAxis("Crouch") > 0)
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
                Controller.height = height * CrouchModifier;
                Controller.center = new Vector3(0, -Controller.height / 2, 0);
                PlayerCamera.transform.localPosition = new Vector3(0, Controller.center.y + Controller.height / 2 - Controller.height * 0.125f, 0);
            }
            else if (!Physics.CheckSphere(HeadChecker.transform.position, HeadChecker.radius, CollisionMask))
            {
                Controller.height = height;
                Controller.center = new Vector3(0, 0, 0);
                PlayerCamera.transform.localPosition = new Vector3(0, Controller.height / 2 - Controller.height * 0.125f, 0);
            }
        }
        else
        {
            if (Input.GetAxis("Crouch") > 0)
            {
                Controller.height = height * CrouchModifier;
                Controller.center = new Vector3(0, -Controller.height / 2, 0);
                PlayerCamera.transform.localPosition = new Vector3(0, Controller.center.y + Controller.height / 2 - Controller.height * 0.125f, 0);
            }
            else if (!Physics.CheckSphere(HeadChecker.transform.position, HeadChecker.radius, CollisionMask))
            {
                Controller.height = height;
                Controller.center = new Vector3(0, 0, 0);
                PlayerCamera.transform.localPosition = new Vector3(0, Controller.height / 2 - Controller.height * 0.125f, 0);
            }
        }
    }

    /// <summary>
    /// Handles sprinting and generic movement. Called on <see cref="Update"/>
    /// </summary>
    protected virtual void Sprint()
    {

        float x = Input.GetAxis("Horizontal") * PlayerSpeed;
        float y = Input.GetAxis("Vertical") * PlayerSpeed;

        if (ToggleSprint)
        {
            if (Input.GetAxis("Sprint") > 0)
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

            if (sToggle)
            {
                Controller.Move(transform.forward * y * SprintModifier * Time.deltaTime + transform.right * x * SprintModifier * Time.deltaTime);
                if (FOVModifier && PlayerCamera.fieldOfView < fov * (1 + (SprintModifier * 0.1f))) PlayerCamera.fieldOfView += 0.5f;
            }
            else
            {
                Controller.Move(transform.forward * y * Time.deltaTime + transform.right * x * Time.deltaTime);
                if (PlayerCamera.fieldOfView > fov) PlayerCamera.fieldOfView -= 0.5f;
            }
        }
        else
        {
            if (Input.GetAxis("Sprint") > 0)
            {
                Controller.Move(transform.forward * y * SprintModifier * Time.deltaTime + transform.right * x * SprintModifier * Time.deltaTime);
                if (FOVModifier && PlayerCamera.fieldOfView < fov * (1 + (SprintModifier * 0.1f))) PlayerCamera.fieldOfView += 0.5f;
            }
            else
            {
                Controller.Move(transform.forward * y * Time.deltaTime + transform.right * x * Time.deltaTime);
                if (PlayerCamera.fieldOfView > fov) PlayerCamera.fieldOfView -= 0.5f;
            }
        }
    }

    /// <summary>
    /// Handles what happens when there's a tool in the hand. Called on <see cref="Update"/>
    /// </summary>
    /// <seealso cref="PrimaryAction"/>
    /// <seealso cref="SecondaryAction"/>
    protected virtual void HandleTool()
    {
        heldTool = HandSlot.GetComponentInChildren<Tool>();
        if (heldTool != null)
        {
            PrimaryAction();
            SecondaryAction();
        }
    }

    /// <summary>
    /// Handles what happens when the primary action is activated. Called by <see cref="HandleTool"/>
    /// </summary>
    /// <seealso cref="SecondaryAction" />
    protected virtual void PrimaryAction()
    {
        if (Input.GetAxis("Primary Action") > 0 && !action1Down)
        {
            action1Down = true;

            heldTool.OnPrimaryFire(this);
        }
        else if (Input.GetAxis("Primary Action") > 0 && action1Down)
        {
            heldTool.OnPrimaryHeld(this);
        }
        else if (Input.GetAxis("Primary Action") == 0 && action1Down)
        {
            heldTool.OnPrimaryRelease(this);
            action1Down = false;
        }
    }

    /// <summary>
    /// Handles what happens when the secondary action is activated. Called by <see cref="HandleTool"/>
    /// </summary>
    /// <seealso cref="PrimaryAction"/>
    protected virtual void SecondaryAction()
    {
        if (Input.GetAxis("Secondary Action") > 0 && !action2Down)
        {
            action2Down = true;

            heldTool.OnSecondaryFire(this);
        }
        else if (Input.GetAxis("Secondary Action") > 0 && action2Down)
        {
            heldTool.OnSecondaryHeld(this);
        }
        else if (Input.GetAxis("Secondary Action") == 0 && action2Down)
        {
            heldTool.OnSecondaryRelease(this);
            action2Down = false;
        }
    }

    /// <summary>
    /// Handles jumping and also gravity. Called on <see cref="FixedUpdate"/>
    /// </summary>
    protected virtual void Jump()
    {
        if (Input.GetAxis("Jump") > 0 && Physics.CheckSphere(GroundChecker.transform.position, GroundChecker.radius, CollisionMask))
        {
            velocity = Mathf.Sqrt(JumpHeight * 2 * Gravity);
            Controller.stepOffset = 0.001f;
        }
        else if (!Physics.CheckSphere(GroundChecker.transform.position, GroundChecker.radius, CollisionMask))
        {
            if (velocity > -tVelo)
            {
                velocity -= Gravity * Time.fixedDeltaTime;
            }
        }
        else
        {
            velocity = -0.1f;
            Controller.stepOffset = stepOffset;
        }

        Controller.Move(new Vector3(0, velocity * Time.fixedDeltaTime, 0));
    }

    /// <summary>
    /// Handles calling <c>Interacable</c>. Called on <see cref="FixedUpdate"/>
    /// </summary>
    protected virtual void Interact()
    {
        if (Physics.Linecast(PlayerCamera.transform.position, PlayerCamera.transform.position + (PlayerCamera.transform.forward * Reach / 8), out RaycastHit hit, CollisionMask))
        {
            CarrySlot.transform.position = hit.point;
        }
        else
        {
            CarrySlot.transform.position = PlayerCamera.transform.position + (PlayerCamera.transform.forward * Reach / 8);
        }

        foreach(Transform t in CarrySlot.GetComponentsInChildren<Transform>())
        {
            t.rotation = PlayerCamera.transform.rotation;
        }

        Interactable child = CarrySlot.GetComponentInChildren<Interactable>();
        if (Input.GetAxis("Interact") > 0 && !interactDown)
        {
            interactDown = true;

            if (Physics.Linecast(PlayerCamera.transform.position, PlayerCamera.transform.position + (PlayerCamera.transform.forward * Reach), out RaycastHit info))
            {
                interactingObject = info.transform.GetComponent<Interactable>();
                if (interactingObject != null)
                {
                    interactingObject.OnInteract(this);
                }
            }
            else if (child != null)
            {
                child.OnInteract(this);
            }
        }
        else if (Input.GetAxis("Interact") > 0 && interactDown)
        {
            if (interactingObject != null)
            {
                interactingObject.WhileHeld(this);
            }
            else if (child != null)
            {
                child.OnInteract(this);
            }
        }
        else if (Input.GetAxis("Interact") == 0 && interactDown)
        {
            if (interactingObject != null)
            {
                interactingObject.OnReleased(this);
            }
            else if (child != null)
            {
                child.OnInteract(this);
            }
            interactingObject = null;

            interactDown = false;
        }
    }
    #endregion
}
