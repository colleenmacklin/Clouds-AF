using UnityEngine.Rendering;

namespace UnityEngine.AzureSky
{
    [ExecuteInEditMode]
    [AddComponentMenu("Azure[Sky]/Azure Reflection Controller")]
    public class AzureReflectionController : MonoBehaviour
    {
        public ReflectionProbe reflectionProbe;
        public AzureReflectionProbeState state = AzureReflectionProbeState.Off;
        public Transform followTarget;
        public ReflectionProbeRefreshMode refreshMode = ReflectionProbeRefreshMode.OnAwake;
        public ReflectionProbeTimeSlicingMode timeSlicingMode = ReflectionProbeTimeSlicingMode.NoTimeSlicing;
        public bool updateAtFirstFrame = true;
        public float refreshInterval = 2.0f;
        private float m_timeSinceLastProbeUpdate = 0;
        
        private void Awake()
        {
            if (state != AzureReflectionProbeState.On)
                return;
            if (refreshMode == ReflectionProbeRefreshMode.ViaScripting && updateAtFirstFrame)
            {
                reflectionProbe.RenderProbe();
                //DynamicGI.UpdateEnvironment();
            }
        }
        
        private void Update()
        {
            // Not included in the build
            #if UNITY_EDITOR
            reflectionProbe.mode = ReflectionProbeMode.Realtime;
            reflectionProbe.refreshMode = refreshMode;
            reflectionProbe.timeSlicingMode = timeSlicingMode;
            #endif
            
            if (!Application.isPlaying || state != AzureReflectionProbeState.On) return;
            
            if (followTarget)
                reflectionProbe.transform.position = followTarget.position;
            
            if (refreshMode == ReflectionProbeRefreshMode.EveryFrame)
            {
                reflectionProbe.RenderProbe();
                //DynamicGI.UpdateEnvironment();
                return;
            }

            if (refreshMode != ReflectionProbeRefreshMode.ViaScripting) return;
            
            m_timeSinceLastProbeUpdate += Time.deltaTime;

            if (!(m_timeSinceLastProbeUpdate >= refreshInterval)) return;
            reflectionProbe.RenderProbe();
            //DynamicGI.UpdateEnvironment();
            m_timeSinceLastProbeUpdate = 0;
        }
        
        public void UpdateReflectionProbe()
        {
            reflectionProbe.RenderProbe();
            //DynamicGI.UpdateEnvironment();
        }
    }
}