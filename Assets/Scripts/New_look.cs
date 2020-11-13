using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class New_look : MonoBehaviour

{
    public float MouseSensitivity = 100f;
    public Transform playerBody;
    public float zRotation;
    float xRotation = 0f;

    //newscript
    float mDelta = 10; // Pixels. The width border at the edge in which the movement work
    float mSpeed = 3.0f; // Scale. Speed of the movement

    float xSensitivity = 5.0f;
    float ySensitivity = 5.0f;
    float xAngle = 0.0f;
    float yAngle = 0.0f;


    private Vector3 mForwardDirection; // What direction does our camera start looking at
private Vector3 mRightDirection; // The inital "right" of the camera
private Vector3 mUpDirection; // The inital "up" of the camera
 


    // Start is called before the first frame update
    void Start()
    {
        //hides the cursor
        //Cursor.lockState = CursorLockMode.Locked;
        //zRotation = 0f;

        mForwardDirection = transform.forward;
        mRightDirection = transform.right;
        mUpDirection = transform.up;

    }

    // Update is called once per frame
    void Update()
    {
        //float mouseX = Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime;
        //float mouseY = Input.GetAxis("Mouse Y") * MouseSensitivity * Time.deltaTime;
        //xRotation -= mouseY;
        //zRotation += mouseX;


        //xRotation = Mathf.Clamp(xRotation, -10f, 38f); //change back to 15
        //zRotation = Mathf.Clamp(zRotation, -38f, 38f);


        //transform.localRotation = Quaternion.Euler(xRotation, zRotation, 0f);


        // Omars "change position part"
        // Check if on the right edge
        if (Input.mousePosition.x >= Screen.width - mDelta)
        {
            // Move the camera
            transform.position += mRightDirection * Time.deltaTime * mSpeed;
        }


        if (Input.mousePosition.x <= 0 + mDelta)
        {
            // Move the camera
            transform.position -= mRightDirection * Time.deltaTime * mSpeed;
        }


        if (Input.mousePosition.y >= Screen.width - mDelta)
        {
            // Move the camera
            transform.position += mUpDirection * Time.deltaTime * mSpeed;
        }

        if (Input.mousePosition.y <= 0 + mDelta)
        {
            // Move the camera
            transform.position -= mUpDirection * Time.deltaTime * mSpeed;
        }


        // Changing an angle, if mouse button is held  
        if (Input.GetMouseButton(1))
        {
            // Update x, y angle with the mouse delta
            xAngle += Input.GetAxis("Mouse X") * xSensitivity;
            yAngle -= Input.GetAxis("Mouse Y") * ySensitivity;

            xAngle = Mathf.Clamp(xAngle, -45, 45);
            yAngle = Mathf.Clamp(yAngle, 0, 90);


            // Initialize the rotation to look in our preferred direction
            transform.rotation = Quaternion.LookRotation(mForwardDirection, mUpDirection);

            // Rotate around the current up direction by the accumulated delta mouse x
            transform.RotateAround(transform.position, transform.up, xAngle);

            // Rotate around our own right vector by the accumulated delta mouse y
            transform.RotateAround(transform.position, transform.right, yAngle);
        }

    }

    private void OnMouseDown()
    {
        Debug.Log("-----continue-----");

        EventManager.TriggerEvent("Continue");

    }

}
