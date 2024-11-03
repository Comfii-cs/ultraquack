using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [Header("Ground Check")]
    public float checkRad;
    public Transform checkPos;
    public LayerMask layer;

    [Header("Player Inputs")]
    public Rigidbody rb;
    public Transform orientation;
    public ForceMode fm;

    [Header("Camera Inputs")]
    public Transform playerCam;
    public float sensitivity;

    [Header("Movement Inputs")]
    public float speed;
    public float speedCap;
    public float moveback;

    [Header("Jump Inputs")]
    public float jumpBuffer;
    public float jumpTime;
    public float jumpPower;
    public float coyoteTime;

    // input vectors
    private Vector3 moveInputVector;
    private Vector3 mouseInputVector;
    private Vector3 lookVector;
    private bool jumpBool;

    // degub :D
    private bool dbgCursor;

    public float delta;
    public float buffer;
    public float coyoteTimer;
    public float timeSinceJump;

    private void Start()
    {
        dbgCursor = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        delta = Time.deltaTime * Application.targetFrameRate;
        timeSinceJump += Time.deltaTime;

        DebugDisplay();

        BadInput();

        Qol();

        MovePlayer();

        TurnCamera();

        // unlock mouse

        Cursor.lockState = dbgCursor ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !dbgCursor;
    }

    void BadInput()
    {
        moveInputVector = new(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        mouseInputVector = new(-Input.GetAxisRaw("Mouse Y"), Input.GetAxisRaw("Mouse X"), 0);

        if (Input.GetKeyDown(KeyCode.L))
            dbgCursor = !dbgCursor;

        jumpBool = Input.GetKey(KeyCode.Space);
    }

    void MovePlayer()
    {
        rb.AddForce(orientation.TransformDirection(moveInputVector) * speed * delta, fm);

        // Drag

        Vector3 horizontalMove = new(rb.velocity.x, 0, rb.velocity.z);

        if (Mathf.Abs(moveInputVector.magnitude) < .1f || Mathf.Abs(rb.velocity.magnitude) > speedCap)
            rb.AddForce(-horizontalMove * moveback * delta, fm);

        // Jump

        if (coyoteTimer > 0f && buffer > 0f && timeSinceJump > jumpTime)
        {
            Jump();
        }
    }

    // Quality of life. IE: Jump buffering, Coyote time, etc.
    void Qol()
    {
        // Coyote Time

        if (GroundCheck())
            coyoteTimer = coyoteTime;
        else
            coyoteTimer -= Time.deltaTime;

        // Jump Buffer

        if (jumpBool)
            buffer = jumpBuffer;
        else
            buffer -= Time.deltaTime;
    }

    void Jump()
    {
        rb.velocity = (Vector3.up * jumpPower * delta) + rb.velocity;

        timeSinceJump = 0;
        buffer = 0;
    }

    bool GroundCheck()
    {
        return Physics.CheckSphere(checkPos.position, checkRad, layer, QueryTriggerInteraction.Ignore);
    }

    void TurnCamera()
    {
        lookVector += mouseInputVector * sensitivity * delta;

        playerCam.eulerAngles = lookVector;

        // for player movement
        orientation.eulerAngles = new(0, playerCam.eulerAngles.y);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = GroundCheck() ? Color.green : Color.red;
        Gizmos.DrawWireSphere(checkPos.position, checkRad);
    }

    #region Debug UI

    [Header("Degub :D")]
    public TMP_Text inputDisplay;
    public TMP_Text velDisplay;
    public TMP_Text magDisplay;

    void DebugDisplay()
    {
        inputDisplay.text = moveInputVector.ToString();
        velDisplay.text = rb.velocity.ToString();
        magDisplay.text = rb.velocity.magnitude.ToString();
    }

    #endregion
}
