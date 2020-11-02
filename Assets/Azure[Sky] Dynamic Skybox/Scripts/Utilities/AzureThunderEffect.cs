namespace UnityEngine.AzureSky
{
    public class AzureThunderEffect : MonoBehaviour
    {
        public AudioClip audioClip;
        public AnimationCurve lightFrequency;
        public float audioDelay;
        private AudioSource m_audioSource;
        private Light m_thunderLight;
        private float m_time = 0.0f;
        private bool m_canPlayAudioClip = true;

        private void Start()
        {
            m_audioSource = GetComponent<AudioSource>();
            m_thunderLight = GetComponent<Light>();
            m_audioSource.clip = audioClip;
        }

        private void Update()
        {
            m_time += Time.deltaTime;

            m_thunderLight.intensity = lightFrequency.Evaluate(m_time / audioClip.length);
            Shader.SetGlobalFloat (AzureShaderUniforms.ThunderLightningEffect, m_thunderLight.intensity);

            if (m_time >= audioDelay && m_canPlayAudioClip)
            {
                m_audioSource.Play();
                m_canPlayAudioClip = false;
            }

            if(m_time >= audioDelay + audioClip.length)
                Destroy(gameObject);
        }
    }
}