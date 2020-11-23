using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour

{
    public float MouseSensitivity = 100f;
    public Transform playerBody;
    public float zRotation;
    float xRotation = 0f;
    


    // Start is called before the first frame update
    void Start()
    {
        //hides the cursor
        Cursor.lockState = CursorLockMode.Locked;
        zRotation = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * MouseSensitivity * Time.deltaTime;
        xRotation -= mouseY;
        zRotation += mouseX;


        xRotation = Mathf.Clamp(xRotation, -10f, 38f); //change back to 15
        zRotation = Mathf.Clamp(zRotation, -38f, 38f);


        transform.localRotation = Quaternion.Euler(xRotation, zRotation, 0f);
        //transform.localRotation = Quaternion.Euler(0f, zRotation, 0f);

        //playerBody.Rotate(Vector3.up * mouseX);
    }

    private void OnMouseDown()
    {
        Debug.Log("-----continue-----");

            EventManager.TriggerEvent("Continue");
      
    }

}
