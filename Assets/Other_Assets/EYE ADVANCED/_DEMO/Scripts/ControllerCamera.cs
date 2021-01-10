using UnityEngine;
using System.Collections;

public class ControllerCamera : MonoBehaviour {

	//PUBLIC VARIABLES
	public Transform setCamera;
	public Transform cameraTarget;
	public float followDistance = 5.0f;
	public float followHeight = 1.0f;
	public float followSensitivity = 2.0f;
	public bool useRaycast = true;
	public Vector2 axisSensitivity = new Vector2(4.0f,4.0f);
	public float camFOV = 35.0f;
	public float camRotation = 0.0f;
	public float camHeight = 0.0f;
	public float camYDamp = 0.0f;
	public Vector2 camLookOffset = new Vector2(0.0f,0.0f);

	//PRIVATE VARIABLES
	//private Vector3 targetPosition;
	private float MouseRotationDistance = 0.0f;
	private float MouseVerticalDistance = 0.0f;
	private float MouseScrollDistance = 0.0f;
	//private bool isControllable = true;
	//private Transform playerObject;
	//private float CameraHeightDistance = 0.0f;



	void Start () {
		camLookOffset.x = cameraTarget.transform.localPosition.x;
		camLookOffset.y = cameraTarget.transform.localPosition.y;
	}
	


	void LateUpdate () {
	
		if (setCamera == null) setCamera = Camera.main.transform;

		//CHECK FOR MOUSE INPUT
		//targetPosition = cameraTarget.position;
		//var oldMouseRotation = MouseRotationDistance;
		//var oldMouseVRotation = MouseVerticalDistance;
		
		//orbitView = true;
		//isControllable = true;

		if (Input.mousePosition.x > 365f && Input.mousePosition.y < 648f && Input.mousePosition.y > 50f){

			if (Input.GetMouseButton(0)){
				MouseRotationDistance = Input.GetAxisRaw("Mouse X")*2.7f;
				MouseVerticalDistance = Input.GetAxisRaw("Mouse Y")*2.7f;
			} else {
				MouseRotationDistance = 0.0f;
				MouseVerticalDistance = 0.0f;
			}
			
			MouseScrollDistance = Input.GetAxisRaw("Mouse ScrollWheel");
			
			if (Input.GetMouseButton(2)){
				camLookOffset.x += Input.GetAxisRaw("Mouse X")*0.001f;
				camLookOffset.y += Input.GetAxisRaw("Mouse Y")*0.001f;
			}

		} else {
			MouseRotationDistance = 0.0f;
			MouseVerticalDistance = 0.0f;
		}

		//GET CHARACTER
		//cameraTarget = playerObject;


		//CHECK GAME MODES
		//isControllable = false;
		//Screen.lockCursor = false;
		followHeight = 1.5f;



		//rotate target
		Vector3 rotVec = new Vector3(	cameraTarget.transform.eulerAngles.x - MouseVerticalDistance,
										cameraTarget.transform.eulerAngles.y - MouseRotationDistance,
										cameraTarget.transform.eulerAngles.z);
		cameraTarget.transform.eulerAngles = rotVec;

		//move target
		Vector3 tgtVec = new Vector3(	camLookOffset.x,
										camLookOffset.y,
										cameraTarget.transform.localPosition.z);
		cameraTarget.transform.localPosition = tgtVec;

		//zoom camera
		Vector3 zmVec = new Vector3(	setCamera.localPosition.x,
										setCamera.localPosition.y,
										Mathf.Clamp(setCamera.localPosition.z,-9.73f,-9.66f));
		setCamera.localPosition = zmVec;

		if (setCamera.localPosition.z >= -9.73f && setCamera.localPosition.z <= -9.66f){
			if (MouseScrollDistance != 0.0f){
				setCamera.transform.Translate(-Vector3.forward*MouseScrollDistance*0.02f,transform);
			}
		}


	/*
		camRotation = Mathf.Lerp(oldMouseRotation,MouseRotationDistance,Time.deltaTime*axisSensitivity.x);
		
		camHeight = Mathf.Lerp(camHeight,camHeight+MouseVerticalDistance,Time.deltaTime*axisSensitivity.y);
		camHeight = Mathf.Clamp(camHeight,0.1,8.0);
		
		camHeight += CameraHeightDistance;
		
		//set camera to follow target object
		var followPos : Vector3 = targetPosition;
		followPos.y = targetPosition.y + followHeight;
		
		//rotate figure based on mouse input
		//cameraTarget.transform.eulerAngles.y -= camRotation;

		//set camera distance
		setCamera.transform.position = targetPosition;
		setCamera.transform.position.y += camHeight;//followPos.y;

		//Zoom Camera
		setCamera.transform.Translate(-Vector3.forward*followDistance,transform);
		setCamera.transform.eulerAngles.z += Mathf.Sin(50.0*(Time.deltaTime*0.1));
		setCamera.transform.LookAt(followPos);
		
		//camLookOffset.y = Mathf.Clamp(camLookOffset.y,-2.0,2.0);
		setCamera.transform.position.y += camLookOffset.y;
		setCamera.transform.Translate(Vector3.left * (camLookOffset.x));
		
		//set camera settings
		setCamera.GetComponent.<Camera>().fieldOfView = camFOV;

		//MouseRotationDistance = Input.GetAxisRaw("Mouse X");
		//MouseVerticalDistance = Input.GetAxisRaw("Mouse Y");
	*/

	}
}
