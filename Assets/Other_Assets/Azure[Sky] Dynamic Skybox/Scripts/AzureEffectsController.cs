using System.Collections.Generic;

namespace UnityEngine.AzureSky
{
	[ExecuteInEditMode]
	[AddComponentMenu("Azure[Sky]/Azure Effects Controller")]
	public class AzureEffectsController : MonoBehaviour
	{
		// Not included in the build
		#if UNITY_EDITOR
		public bool showWindHeaderGroup = false;
		public bool showSoundsHeaderGroup = false;
		public bool showParticlesHeaderGroup = false;
		public bool showThundersHeaderGroup = false;
		#endif
		
		private AzureSkyController m_skyController;
		
		// Wind
		public WindZone windZone;
		public float windMultiplier = 1.0f;
		private Vector3 m_windDirection = Vector3.forward;
		
		// Sounds
		public AudioSource lightRainSoundFx;
		public AudioSource mediumRainSoundFx;
		public AudioSource heavyRainSoundFx;
		public AudioSource lightWindSoundFx;
		public AudioSource mediumWindSoundFx;
		public AudioSource heavyWindSoundFx;

		// Particles
		public Transform particleSystemTransform;
		public Material rainMaterial, heavyRainMaterial, snowMaterial, rippleMaterial;
		public ParticleSystem lightRainParticle, mediumRainParticle, heavyRainParticle, snowParticle;
		public Transform followTarget;

		// Thunders
		public List<AzureThunderSettings> thunderSettingsList = new List<AzureThunderSettings>();

		private void OnEnable()
		{
			m_skyController = GetComponent<AzureSkyController>();
		}

		private void Start()
		{
			m_skyController = GetComponent<AzureSkyController>();
			UpdateParticlesMaterials();
			UpdateParticlesPosition();
		}
		
		private void Update()
		{
			UpdateParticlesMaterials();
			UpdateParticlesPosition();
			if (Application.isPlaying)
			{
				SoundEffectController(m_skyController.settings.LightRainSoundVolume, lightRainSoundFx);
				SoundEffectController(m_skyController.settings.MediumRainSoundVolume, mediumRainSoundFx);
				SoundEffectController(m_skyController.settings.HeavyRainSoundVolume, heavyRainSoundFx);
				SoundEffectController(m_skyController.settings.LightWindSoundVolume, lightWindSoundFx);
				SoundEffectController(m_skyController.settings.MediumWindSoundVolume, mediumWindSoundFx);
				SoundEffectController(m_skyController.settings.HeavyWindSoundVolume, heavyWindSoundFx);

				ParticleEffectController(m_skyController.settings.LightRainIntensity * 4000.0f, lightRainParticle);
				ParticleEffectController(m_skyController.settings.MediumRainIntensity * 4000.0f, mediumRainParticle);
				ParticleEffectController(m_skyController.settings.HeavyRainIntensity * 2000.0f, heavyRainParticle);
				ParticleEffectController(m_skyController.settings.SnowIntensity * 2000.0f, snowParticle);

				windZone.windMain = m_skyController.settings.WindSpeed * windMultiplier;
				m_windDirection = new Vector3(0.0f, m_skyController.settings.WindDirection + 180.0f, 0.0f);
				windZone.transform.rotation = Quaternion.Euler(m_windDirection);
			}
		}
		
		/// <summary>
		/// Start and stop the sounds effect.
		/// </summary>
		private void SoundEffectController(float volume, AudioSource sound)
		{
			sound.volume = volume;
			if (volume > 0)
			{
				if (!sound.isPlaying) sound.Play ();
			}
			else if (sound.isPlaying) sound.Stop ();
		}
		
		/// <summary>
		/// Start and stop the particle effect.
		/// </summary>
		private void ParticleEffectController(float intensity, ParticleSystem particle)
		{
			var emission = particle.emission;
			emission.rateOverTimeMultiplier = intensity;
			if (intensity > 0)
			{
				if (!particle.isPlaying) particle.Play ();
			}
			else if (particle.isPlaying) particle.Stop ();
		}

		/// <summary>
		/// Updates the particle position.
		/// </summary>
		private void UpdateParticlesPosition()
		{
			if (followTarget)
				particleSystemTransform.position = followTarget.position;
		}

		/// <summary>
		/// Updates the particles color.
		/// </summary>
		private void UpdateParticlesMaterials()
		{
			rainMaterial.SetColor("_TintColor", m_skyController.settings.RainColor);
			heavyRainMaterial.SetColor("_TintColor", m_skyController.settings.RainColor);
			snowMaterial.SetColor("_TintColor", m_skyController.settings.SnowColor);
			rippleMaterial.SetColor("_TintColor", m_skyController.settings.RainColor);
		}

		/// <summary>
		/// Create a thunder effect in the scene. When the thunder sound is over, the instance is automatically deleted.
		/// </summary>
		public void InstantiateThunderEffect(int index)
		{
			Transform thunder = Instantiate(thunderSettingsList[index].thunderPrefab, thunderSettingsList[index].position, thunderSettingsList[index].thunderPrefab.rotation);
			AzureThunderEffect thunderEffect = thunder.GetComponent<AzureThunderEffect>();
			thunderEffect.audioClip = thunderSettingsList[index].audioClip;
			thunderEffect.audioDelay = thunderSettingsList[index].audioDelay;
			thunderEffect.lightFrequency = thunderSettingsList[index].lightFrequency;
		}
		
		/// <summary>
		/// Create a thunder effect in the scene. When the thunder sound is over, the instance is automatically deleted.
		/// </summary>
		public void InstantiateThunderEffect(int index, Vector3 worldPos)
		{
			Transform thunder = Instantiate(thunderSettingsList[index].thunderPrefab, worldPos, thunderSettingsList[index].thunderPrefab.rotation);
			AzureThunderEffect thunderEffect = thunder.GetComponent<AzureThunderEffect>();
			thunderEffect.audioClip = thunderSettingsList[index].audioClip;
			thunderEffect.audioDelay = thunderSettingsList[index].audioDelay;
			thunderEffect.lightFrequency = thunderSettingsList[index].lightFrequency;
		}
	}
}