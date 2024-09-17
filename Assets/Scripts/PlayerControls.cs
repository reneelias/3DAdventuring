using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Color materialColor;
    [Header("Physics")]
    [Tooltip("How quickly the player picks up speed.")]
    [SerializeField] private float movementSpeed = .05f;
    [Tooltip("Scale by which movement speed gets dampened when player is in the air.")]
    [SerializeField] private float inAirMoveDamp = .5f;
    [SerializeField] private float gravityScale = .5f;
    [SerializeField] private float jumpSpeed = 3f;
    [Tooltip("That maximum speed that the player can move it using directional input.")]
    [SerializeField] private float moveVelMaxSpeed = 1f;
    [Tooltip("Scale by which velocity will be multiplied each update when no movement input detected.")]
    [SerializeField] private float moveVelSlowRate = .1f;
    private Vector3 moveVelocity = Vector3.zero;
    [Header("Jumping Raycast")]
    [SerializeField] private GameObject raycastPointsParent;
    [SerializeField] private float rayCastDistance = .25f;
    private bool grounded = false;
    [Header("Turning")]
    [SerializeField] private float turningSpeed = 1f;
    [SerializeField] private float mouseTurnSpeed = 5f;
    private float upVelocity = 0f;
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

        if(movementVector.magnitude == 0f){
            if(moveVelocity.magnitude > 0f){
                moveVelocity *= moveVelSlowRate;
            }
        } else {
            movementVector = movementVector.normalized * movementSpeed * (grounded ? 1f : inAirMoveDamp);
            movementVector = Quaternion.Euler(0f, forwardMovementDirection.y, 0f) * movementVector;
            moveVelocity += movementVector;
            if(moveVelocity.magnitude > moveVelMaxSpeed){
                moveVelocity = moveVelocity.normalized * moveVelMaxSpeed;
            }
        }

        characterController.Move(moveVelocity);
    }

    void JumpingControls(){
        int i = 0;
        foreach(Transform transform in raycastPointsParent.transform){
            if(Physics.Raycast(new Ray(transform.position, Vector3.down), rayCastDistance)){
                if(!grounded){
                    upVelocity = 0f;
                }
                grounded = true;
                break;
            }
            i++;
        }
        if(i == raycastPointsParent.transform.childCount){
            grounded = false;
        }

        if(grounded){
            if(Input.GetKeyDown(KeyCode.Space)){
                upVelocity = jumpSpeed;
            }
        } else {
            upVelocity += Physics.gravity.y * gravityScale * Time.deltaTime;
        }
        
        characterController.Move(Vector3.up * upVelocity * Time.deltaTime);
    }

    void ResetPosition(){
        if(Input.GetKeyDown(KeyCode.R)){
            characterController.enabled = false;
            transform.position = originalPosition;
            characterController.enabled = true;
            // yVelocity = 0f;
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

    void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("GravObj")){
            Debug.Log($"Name: {other.name}");
            GravitationalObject gravObj = other.transform.parent.GetComponent<GravitationalObject>();
            if(gravObj == null){
                print("GravObj is null");
                return;
            }
            Debug.Log($"Gravity Type: {gravObj.GravityType}");
            Debug.Log($"Shape Type: {gravObj.ObjectShape}");
            Debug.Log($"Gravity Scale: {gravObj.GravityScale}");
        }
    }
}
