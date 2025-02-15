using System.Data.Common;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float rotationSpeed = 10f;

    [SerializeField] private Transform cameraTransform; // Assign Cinemachine Camera Transform

    // for the dash feature
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;

    private Rigidbody playerRB;
    private bool isGrounded;
    private int jumpCount = 0; // to keep track of how many jumps the player has done
    // for the dash feature
    private bool isDashing;
    private float dashCooldownTime;

    void Start()
    {
        playerRB = GetComponent<Rigidbody>();
    }

    void Update()
    {
        MovePlayer();

        // the jump logic
        if (Input.GetKeyDown(KeyCode.Space) && (isGrounded || jumpCount < 2)) // add the double jump logic
        {
            Jump();
        }

        // the dash logic
        if (Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownTime <= 0f && !isDashing)
        {
            Dash();
        }

        if (dashCooldownTime > 0f)
        {
            dashCooldownTime -= Time.deltaTime;
        }
    }

    private void MovePlayer()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // get the camera’s forward and right directions (ignoring Y axis)
        Vector3 cameraForward = cameraTransform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();

        Vector3 cameraRight = cameraTransform.right;
        cameraRight.y = 0;
        cameraRight.Normalize();

        // convert input into world space movement
        Vector3 moveDirection = cameraForward * vertical + cameraRight * horizontal;
        moveDirection.Normalize(); // Prevent faster diagonal movement

        // move the player
        Vector3 targetPosition = playerRB.position + moveDirection * moveSpeed * Time.deltaTime;
        playerRB.MovePosition(targetPosition);

        // rotate player to face movement direction
        if (moveDirection.magnitude > 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            playerRB.rotation = Quaternion.Slerp(playerRB.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void Jump()
    {
        playerRB.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        if (isGrounded)
        {
            jumpCount = 1; // the player is on the ground, so they have one jump
        }
        else
        {
            jumpCount = Mathf.Min(jumpCount + 1, 2); // increment the jumpCount but max it out at 2
        }
        isGrounded = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // check if the player is currently touching the ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            jumpCount = 0; // reset back to 0 when touching the ground
        }
    }

    private void Dash()
    {
        isDashing = true;

        // apply the dash force in the player's forward direction
        Vector3 dashDirection = transform.forward;
        playerRB.linearVelocity = dashDirection * dashSpeed; // Override velocity to perform dash
        
        // Start the dash cooldown
        dashCooldownTime = dashCooldown;
        
        // Reset the dash state after the dash duration
        Invoke("EndDash", dashDuration);
    }

    private void EndDash()
    {
        isDashing = false;
        playerRB.linearVelocity = Vector3.zero;
    }
}
