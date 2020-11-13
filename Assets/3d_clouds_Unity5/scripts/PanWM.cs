using UnityEngine;

/// <summary>
/// Placing this script on the game object will make that game object pan with mouse movement.
/// </summary>

public class PanWM : MonoBehaviour 
{
	public Vector2 degrees = new Vector2(5f, 3f);
	private Vector2 shake = new Vector2(5f, 3f);
	public static float shake_value=0f;
	public static float shake_speed=10f;
	public float range = 1f;
	private float t=0f;
	Transform mTrans;
	Quaternion mStart;
	Vector2 mRot = Vector2.zero;

	void Start ()
	{
		mTrans = transform;
		mStart = mTrans.localRotation;
	}

	void Update ()
	{
		t+=shake_speed*Time.deltaTime;
		shake= new Vector2(Mathf.Sin(t*5f)*shake_value,Mathf.Sin(t*3f)*shake_value);
		float delta = Time.deltaTime;
		Vector3 pos = Input.mousePosition;

		float halfWidth = Screen.width * 0.5f;
		float halfHeight = Screen.height * 0.5f;
		if (range < 0.1f) range = 0.1f;
		float x = Mathf.Clamp((pos.x - halfWidth) / halfWidth / range, -1f, 1f);
		float y = Mathf.Clamp((pos.y - halfHeight) / halfHeight / range, -1f, 1f);
		
		mRot = Vector2.Lerp(mRot, new Vector2(x, y), delta * 5f);
		mTrans.localRotation = mStart * Quaternion.Euler(-mRot.y * degrees.y+shake.y, mRot.x * degrees.x+shake.x, 0f);
		this.transform.eulerAngles = new Vector3(this.transform.eulerAngles.x,this.transform.eulerAngles.y,0f);
		
	}
}