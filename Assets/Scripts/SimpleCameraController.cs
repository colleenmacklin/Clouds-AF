using UnityEngine;

namespace UnityTemplateProjects
{
    public class SimpleCameraController : MonoBehaviour
    {

        [Header("Rotation Settings")]
        [Tooltip("X = Change in mouse position.\nY = angle change factor for camera rotation.")]
        public AnimationCurve mouseSensitivityCurve = new AnimationCurve(new Keyframe(0f, 0.5f, 0f, 5f), new Keyframe(1f, 2.5f, 0f, 0f));

        public float maxAngleX;
        public float maxAngleY;


        void Update()
        {
            Vector2 mousePos = Input.mousePosition;
            mousePos = Camera.main.ScreenToViewportPoint(mousePos);
            mousePos.x = Mathf.Clamp01(mousePos.x);
            mousePos.y = Mathf.Clamp01(mousePos.y);
            mousePos = mousePos * 2 - Vector2.one;
//            print(mousePos);
            mousePos.y /= Camera.main.aspect; // less sensitive on y

            float yaw = mouseSensitivityCurve.Evaluate(Mathf.Abs(mousePos.x)) * mousePos.x * maxAngleX;
            float pitch = mouseSensitivityCurve.Evaluate(Mathf.Abs(mousePos.y)) * mousePos.y * maxAngleY;

           // print(yaw);
           // print(pitch);
            transform.localEulerAngles = new Vector3(-pitch, yaw, 0);

        }
    }
}