using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    [Header("Physics")]
    [SerializeField] private float movementSpeed = .5f;
    [SerializeField] private float gravityScale = .5f;
    [SerializeField] private float jumpSpeed = 3f;
    [Header("Jumping Raycast")]
    [SerializeField] private float rayCastOffsetScale = .75f;
    [SerializeField] private float rayCastDistance = .25f;
    private float yVelocity = 0f;
    private Vector3 movementVector = new Vector3();
    private Vector3 originalPosition;
    
    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        JumpingControls();
        ResetPosition();
    }

    void FixedUpdate()
    {
        MovementControls();
    }

    void MovementControls(){
        movementVector = new Vector3();
        
        if(Input.GetKey(KeyCode.W)){
            movementVector += Vector3.forward;
        } else if(Input.GetKey(KeyCode.S)){
            movementVector += Vector3.back;
        } else if(Input.GetKey(KeyCode.A)){
            movementVector += Vector3.left;
        } else if(Input.GetKey(KeyCode.D)){
            movementVector += Vector3.right;
        } 
        movementVector = movementVector.normalized * movementSpeed;
        characterController.Move(movementVector);
    }

    void JumpingControls(){
        if(Input.GetKeyDown(KeyCode.Space) && Physics.Raycast(new Ray(transform.position + Vector3.down * rayCastOffsetScale, Vector3.down), rayCastDistance)){
            yVelocity = jumpSpeed;
        }

        characterController.Move(Vector3.up * yVelocity * Time.deltaTime);
        yVelocity += Physics.gravity.y * gravityScale * Time.deltaTime;
    }

    void ResetPosition(){
        if(Input.GetKeyDown(KeyCode.R)){
            characterController.enabled = false;
            transform.position = originalPosition;
            characterController.enabled = true;
        }
    }
}
