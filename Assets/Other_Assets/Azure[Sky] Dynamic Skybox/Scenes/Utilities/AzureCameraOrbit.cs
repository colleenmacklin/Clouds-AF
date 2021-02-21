namespace UnityEngine.AzureSky
{
	public class AzureCameraOrbit : MonoBehaviour
	{
		private float m_mouseX = 0.0f;
		private float m_mouseY = 0.0f;

		private Vector3 m_startRotation;

		private void Start()
		{
			m_startRotation = transform.eulerAngles;
		}

		private void Update()
		{
			if (Input.GetMouseButton(1))
			{
				m_mouseX += Input.GetAxis("Mouse X") * 2.5f;
				m_mouseY -= Input.GetAxis("Mouse Y") * 2.5f;
				transform.localRotation = Quaternion.Euler(new Vector3(m_mouseY, m_mouseX, transform.localRotation.z) + m_startRotation);
			}
		}
	}
}