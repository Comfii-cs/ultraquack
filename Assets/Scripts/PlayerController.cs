using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
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
    public float jumpDelay;
    public float jumpPower;
    public float coyoteTime;

    // input vectors
    private Vector3 moveInputVector;
    private Vector3 mouseInputVector;
    private Vector3 lookVector;
    private bool jumpBool;

    // degub :D
    private bool dbgCursor;

    private float delta;
    private float buffer;
    private float coyoteTimer;

    private void Start()
    {
        dbgCursor = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        delta = Time.deltaTime * Application.targetFrameRate;

        DebugDisplay();

        BadInput();

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

        if (Mathf.Abs(moveInputVector.magnitude) < .1f || Mathf.Abs(rb.velocity.magnitude) > speedCap)
            rb.AddForce(-rb.velocity * moveback * delta, fm);
    }

    void Jump()
    {

    }

    void TurnCamera()
    {
        lookVector += mouseInputVector * sensitivity * delta;

        playerCam.eulerAngles = lookVector;

        // for player movement
        orientation.eulerAngles = new(0, playerCam.eulerAngles.y);
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
