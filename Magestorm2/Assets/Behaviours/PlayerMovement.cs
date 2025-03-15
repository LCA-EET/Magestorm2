using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    
    public CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private float playerSpeed = 2.0f;
    private float jumpHeight = 1.0f;
    private float gravityValue = -9.81f;
    private Ray _downCast;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
       
    }
    private bool isGrounded()
    {
        RaycastHit hitInfo;
        controller.Raycast(new Ray(transform.position, new Vector3(0, -1, 0)), out hitInfo, 5.0f);
        if (hitInfo.collider != null)
        {
            Debug.Log(hitInfo.collider.gameObject.name);
            return true;
        }
        return false;
    }
    private void MoveAlongAxis(string axisString, Vector3 direction)
    {
        float axis = Input.GetAxis(axisString);
        if (axis < 0.0f)
        {
            direction *= -1;
        }
        if (axis != 0.0f)
        {
            controller.Move(direction * Time.deltaTime * playerSpeed);
        }
    }
    void Update()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }
        MoveAlongAxis("Vertical", transform.forward);
        MoveAlongAxis("Horizontal", transform.right);
            

        /*
        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;
        }
        */

        // Makes the player jump
        /*
        if (Input.GetButtonDown("Jump") && isGrounded())
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        
        controller.Move(playerVelocity * Time.deltaTime);
        */
    }
}
