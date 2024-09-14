using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Color materialColor;
    [Header("Physics")]
    [SerializeField] private float movementSpeed = .5f;
    [SerializeField] private float gravityScale = .5f;
    [SerializeField] private float jumpSpeed = 3f;
    [Header("Jumping Raycast")]
    [SerializeField] private float rayCastOffsetScale = .75f;
    [SerializeField] private float rayCastDistance = .25f;
    [Header("Turning")]
    [SerializeField] private float turningSpeed = 1f;
    [SerializeField] private float mouseTurnSpeed = 5f;
    private float yVelocity = 0f;
    private Vector3 movementVector = new Vector3();
    private Vector3 originalPosition;
    private Vector3 forwardMovementDirection = Vector3.forward;
    
    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.position;
        GetComponent<Renderer>().material.color = materialColor;
    }

    // Update is called once per frame
    void Update()
    {
        JumpingControls();
        ResetPosition();
        TurningControls();
    }

    void FixedUpdate()
    {
        MovementControls();
    }

    void MovementControls(){
        movementVector = new Vector3();
        
        if(Input.GetKey(KeyCode.W)){
            movementVector += Vector3.forward;
        } 
        if(Input.GetKey(KeyCode.S)){
            movementVector += Vector3.back;
        }
        if(Input.GetKey(KeyCode.A)){
            movementVector += Vector3.left;
        } 
        if(Input.GetKey(KeyCode.D)){
            movementVector += Vector3.right;
        }

        movementVector = movementVector.normalized * movementSpeed;
        movementVector = Quaternion.Euler(0f, forwardMovementDirection.y, 0f) * movementVector;

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

    void TurningControls(){
        float axisMouseX = Input.GetAxis("Mouse X");
        if(axisMouseX != 0 && Input.GetMouseButton(1)){
            transform.eulerAngles += new Vector3(0, axisMouseX * mouseTurnSpeed * Time.deltaTime, 0f);
            forwardMovementDirection += new Vector3(0, axisMouseX * mouseTurnSpeed * Time.deltaTime, 0f);
        } else {
            if(Input.GetKey(KeyCode.LeftArrow)){
                transform.eulerAngles += new Vector3(0, -turningSpeed * Time.deltaTime, 0f);
                forwardMovementDirection += new Vector3(0, -turningSpeed * Time.deltaTime, 0f);
            }
            if(Input.GetKey(KeyCode.RightArrow)){
                transform.eulerAngles += new Vector3(0, turningSpeed * Time.deltaTime, 0f);
                forwardMovementDirection += new Vector3(0, turningSpeed * Time.deltaTime, 0f);
            }
        }
        
    }
}
