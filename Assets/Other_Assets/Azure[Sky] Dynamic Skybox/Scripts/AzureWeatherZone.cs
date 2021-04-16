namespace UnityEngine.AzureSky
{
    [ExecuteInEditMode]
    [AddComponentMenu("Azure[Sky]/Azure Weather Zone")]
    public sealed class AzureWeatherZone : MonoBehaviour
    {
        public float blendDistance = 0f;
        public AzureWeatherProfile profile;
        private Collider m_tempCollider;
        private Color m_gizmosColor1 = new Color(0, 1, 0, 0.25f);
        
        private void OnEnable()
        {
            m_tempCollider = GetComponent<Collider>();
        }
        
        // Draws the zone collider gizmos
        // Based on Unity's PostProcessVolume.cs
        private void OnDrawGizmos()
        {
            m_tempCollider = GetComponent<Collider>();
            if (m_tempCollider == null)
                return;
            
            if (m_tempCollider.enabled)
            {
                var scale = transform.lossyScale;
                var invScale = new Vector3(1f / scale.x, 1f / scale.y, 1f / scale.z);
                Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, scale);
                
                // We'll just use scaling as an approximation for volume skin. It's far from being
                // correct (and is completely wrong in some cases). Ultimately we'd use a distance
                // field or at least a tesselate + push modifier on the collider's mesh to get a
                // better approximation, but the current Gizmoz system is a bit limited and because
                // everything is dynamic in Unity and can be changed at anytime, it's hard to keep
                // track of changes in an elegant way (which we'd need to implement a nice cache
                // system for generated volume meshes).
                var type = m_tempCollider.GetType();
                if (type == typeof(BoxCollider))
                {
                    var c = (BoxCollider) m_tempCollider;
                    Gizmos.color = m_gizmosColor1;
                    Gizmos.DrawCube(c.center, c.size);
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireCube(c.center, c.size + invScale * blendDistance * 4f);
                }
                else if (type == typeof(SphereCollider))
                {
                    var c = (SphereCollider) m_tempCollider;
                    Gizmos.DrawSphere(c.center, c.radius);
                    Gizmos.DrawWireSphere(c.center, c.radius + invScale.x * blendDistance * 2f);
                }
                else if (type == typeof(MeshCollider))
                {
                    var c = (MeshCollider) m_tempCollider;
                    
                    // Only convex mesh collider are allowed
                    if (!c.convex)
                        c.convex = true;
                    
                    // Mesh pivot should be centered or this won't work
                    Gizmos.DrawMesh(c.sharedMesh);
                    Gizmos.DrawWireMesh(c.sharedMesh, Vector3.zero, Quaternion.identity,
                        Vector3.one + invScale * blendDistance * 4f);
                }
            }
            
            m_tempCollider = null;
        }
    }
}