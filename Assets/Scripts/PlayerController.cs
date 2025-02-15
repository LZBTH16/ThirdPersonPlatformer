using System.Data.Common;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float rotationSpeed = 10f;

    [SerializeField] private Transform cameraTransform; // Assign Cinemachine Camera Transform

    private Rigidbody playerRB;
    private bool isGrounded;
    private int jumpCount = 0; // to keep track of how many jumps the player has done

    void Start()
    {
        playerRB = GetComponent<Rigidbody>();
    }

    void Update()
    {
        MovePlayer();
        if (Input.GetButtonDown("Jump") && (isGrounded || jumpCount < 2)) // add the double jump logic
        {
            Jump();
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
}
