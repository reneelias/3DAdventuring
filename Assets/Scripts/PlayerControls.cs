using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    [SerializeField] private bool usesRigidbody = true;
    [SerializeField] private new Rigidbody rigidbody;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Color materialColor;

    [Header("Movement")]
    [Tooltip("How quickly the player picks up speed.")]
    [SerializeField] private float movementSpeed = .05f;
    [Tooltip("Scale by which movement speed gets dampened when player is in the air.")]
    [SerializeField] private float inAirMoveDamp = .5f;
    [Tooltip("That maximum speed that the player can move it using directional input.")]
    [SerializeField] private float moveVelMaxSpeed = 1f;
    [Tooltip("Scale by which velocity will be multiplied each update when no movement input detected.")]
    [SerializeField] private float moveVelSlowRate = .1f;
    private Vector3 moveVelocity = Vector3.zero;

    [Header("Gravity")]
    [SerializeField] private float gravityScale = -9.87f;
    [SerializeField] private float jumpSpeed = 3f;
    public GravitationalObject GravitationalObject { get; private set;} = null;
    private Vector3 gravity = Vector3.down;
    private Vector3 upVector = Vector3.up;

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
        UpdateGravity();
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

        if(usesRigidbody){
            rigidbody.velocity += movementVector;
            rigidbody.rotation = Quaternion.Euler(0f, rigidbody.rotation.eulerAngles.y, 0f);
            rigidbody.angularVelocity = Vector3.zero;
        } else {
            characterController.Move(moveVelocity);
        }
    }

    void JumpingControls(){
        int i = 0;
        foreach(Transform transform in raycastPointsParent.transform){
            if(Physics.Raycast(new Ray(transform.position, gravity.normalized), rayCastDistance)){
                if(!grounded){
                    upVector = Vector3.zero;
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
                upVector = new Vector3(-gravity.x, -gravity.y, -gravity.z).normalized * jumpSpeed;
                if(usesRigidbody){
                    rigidbody.velocity += upVector;
                }
            }
        } else {
            if(usesRigidbody){
                rigidbody.velocity += gravity * Time.deltaTime;
            } else {
                upVector += gravity * Time.deltaTime;
            }
        }
        
        if(usesRigidbody){
            // rigidbody.velocity = moveVelocity;
        } else {
            characterController.Move(upVector * Time.deltaTime);
        }
    }

    void ResetPosition(){
        if(Input.GetKeyDown(KeyCode.R)){
            if(usesRigidbody){
                transform.position = originalPosition;
                rigidbody.velocity = Vector3.zero;
                rigidbody.angularVelocity = Vector3.zero;
            } else {
                characterController.enabled = false;
                transform.position = originalPosition;
                characterController.enabled = true;
            }
            GravitationalObject = null;
            transform.eulerAngles = Vector3.zero;
            forwardMovementDirection = Vector3.forward;
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
                // print(transform.rotation);
            }
            if(Input.GetKey(KeyCode.RightArrow)){
                transform.eulerAngles += new Vector3(0, turningSpeed * Time.deltaTime, 0f);
                forwardMovementDirection += new Vector3(0, turningSpeed * Time.deltaTime, 0f);
                // print(transform.rotation);
            }
        }
    }

    void UpdateGravity(){
        if(GravitationalObject == null){
            gravity = Vector3.down * gravityScale;
            return;
        }

        gravity = GravitationalObject.GetGravity(gameObject);
        Vector3 posDiff = (transform.position - GravitationalObject.GravityPointTransform.position).normalized;
        transform.eulerAngles.Set(posDiff.x, forwardMovementDirection.y, posDiff.z);

        float dotProduct = Vector3.Dot(transform.position, GravitationalObject.GravityPointTransform.position);
        float angle = Mathf.Acos(dotProduct/(transform.position.magnitude * GravitationalObject.GravityPointTransform.position.magnitude));
        Debug.Log($"Angle between objects: {angle * Mathf.Rad2Deg}");
        if(usesRigidbody){
            transform.eulerAngles = new Vector3(90f, 0f, 0f);
        } else {
            characterController.enabled = false;
            transform.eulerAngles = new Vector3(90f, 0f, 0f);
            characterController.enabled = true;
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
            GravitationalObject = gravObj;
        }
    }
}
