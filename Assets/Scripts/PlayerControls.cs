using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private float movementSpeed = .5f;
    [SerializeField] private float gravityScale = .5f;
    [SerializeField] private float jumpSpeed = 3f;
    private float yVelocity = 0f;
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        Vector3 movementVector = new Vector3();

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

        if(characterController.velocity.y == 0f) {
            if(Input.GetKey(KeyCode.Space)){
                yVelocity = jumpSpeed;
            }
        }

        characterController.Move(Vector3.up * yVelocity * Time.fixedDeltaTime);
        yVelocity += Physics.gravity.y * gravityScale * Time.fixedDeltaTime;
        // characterController.Move(Physics.gravity * gravityScale * Time.fixedDeltaTime);
    }
}
