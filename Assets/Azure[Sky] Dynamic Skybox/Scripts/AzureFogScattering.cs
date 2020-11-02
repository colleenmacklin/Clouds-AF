//Based on Unity's GlobalFog.
namespace UnityEngine.AzureSky
{
    [ImageEffectAllowedInSceneView]
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("Azure[Sky]/Azure Fog Scattering")]
    public class AzureFogScattering : MonoBehaviour
    {
        public Material fogScatteringMaterial;
        private Camera m_camera = null;
        private Transform m_cameraTransform = null;
        private Vector3[] m_frustumCorners = new Vector3[4];
        private Rect m_rect = new Rect(0, 0, 1, 1);
        private Matrix4x4 m_frustumCornersArray;

        private static readonly int FrustumCorners = Shader.PropertyToID("_FrustumCorners");

        private void Start()
        {
            m_camera = GetComponent<Camera>();
            m_cameraTransform = m_camera.transform;
        }
        
        [ImageEffectOpaque]
        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            m_camera.depthTextureMode |= DepthTextureMode.Depth;

            if (fogScatteringMaterial == null)
            {
                Graphics.Blit(source, destination);
                return;
            }

            m_camera.CalculateFrustumCorners(m_rect, m_camera.farClipPlane, m_camera.stereoActiveEye, m_frustumCorners);

            m_frustumCornersArray = Matrix4x4.identity;
            m_frustumCornersArray.SetRow(0, m_cameraTransform.TransformVector(m_frustumCorners[0]));  // bottom left
            m_frustumCornersArray.SetRow(2, m_cameraTransform.TransformVector(m_frustumCorners[1]));  // top left
            m_frustumCornersArray.SetRow(3, m_cameraTransform.TransformVector(m_frustumCorners[2]));  // top right
            m_frustumCornersArray.SetRow(1, m_cameraTransform.TransformVector(m_frustumCorners[3]));  // bottom right

            fogScatteringMaterial.SetMatrix(FrustumCorners, m_frustumCornersArray);
            Graphics.Blit(source, destination, fogScatteringMaterial, 0);
        }
    }
}