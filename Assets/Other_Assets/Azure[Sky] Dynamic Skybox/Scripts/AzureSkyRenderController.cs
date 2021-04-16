using System;
using UnityEngine;

namespace UnityEngine.AzureSky
{
    [ExecuteInEditMode]
    [AddComponentMenu("Azure[Sky]/Azure Sky Render Controller")]
    public class AzureSkyRenderController : MonoBehaviour
    {
        #if UNITY_EDITOR
        [SerializeField] private bool m_referencesHeaderGroup;
        [SerializeField] private bool m_scatteringHeaderGroup;
        [SerializeField] private bool m_outerSpaceHeaderGroup;
        [SerializeField] private bool m_fogScatteringHeaderGroup;
        [SerializeField] private bool m_cloudsHeaderGroup;
        [SerializeField] private bool m_optionsHeaderGroup;
        #endif

        // References
        [Tooltip("The Transform used to simulate the sun position in the sky.")]
        [SerializeField] private Transform m_sunTransform = null;
        [Tooltip("The Transform used to simulate the moon position in the sky.")]
        [SerializeField] private Transform m_moonTransform = null;
        [Tooltip("The material used to render the sky.")]
        [SerializeField] private Material m_skyMaterial = null;
        [Tooltip("The material used to render the fog scattering.")]
        [SerializeField] private Material m_fogMaterial = null;
        [Tooltip("The shader used to render the sky only.")]
        [SerializeField] private Shader m_emptySkyShader = null;
        [Tooltip("The shader used to render the sky with static clouds.")]
        [SerializeField] private Shader m_staticCloudsShader = null;
        [Tooltip("The shader used to render the sky with dynamic clouds.")]
        [SerializeField] private Shader m_dynamicCloudsShader = null;
        [Tooltip("The texture used to render the sun disk.")]
        [SerializeField] private Texture2D m_sunTexture = null;
        [Tooltip("The texture used to render the moon disk.")]
        [SerializeField] private Texture2D m_moonTexture = null;
        [Tooltip("The cubemap texture used to render the stars and Milky Way.")]
        [SerializeField] private Cubemap m_starfieldTexture = null;
        [Tooltip("The texture used to render the dynamic clouds.")]
        [SerializeField] private Texture2D m_dynamicCloudsTexture = null;
        [Tooltip("The texture used to render the static clouds.")]
        public Texture2D staticCloudTexture = null;

        // Scattering
        [Tooltip("The molecular density of the air.")]
        public float molecularDensity = 2.545f;
        [Tooltip("The red visible wavelength.")] // (380 to 740)
        public float wavelengthR = 680.0f;
        [Tooltip("The green visible wavelength.")]
        public float wavelengthG = 550.0f;
        [Tooltip("The blue visible wavelength.")]
        public float wavelengthB = 450.0f;
        [Tooltip("The rayleigh altitude in kilometers.")]
        public float kr = 8.4f;
        [Tooltip("The mie altitude in kilometers.")]
        public float km = 1.2f;
        [Tooltip("The rayleigh scattering multiplier.")]
        public float rayleigh = 1.5f;
        [Tooltip("The mie scattering multiplier.")]
        public float mie = 1.0f;
        [Tooltip("The mie distance.")]
        public float mieDistance = 1.0f;
        [Tooltip("The scattering intensity.")]
        public float scattering = 0.25f;
        [Tooltip("The sky luminance, useful when there is no moon at night sky.")]
        public float luminance = 1.5f;
        [Tooltip("The exposure of the internal sky shader tonemapping.")]
        public float exposure = 2.0f;
        [Tooltip("The rayleigh color multiplier.")]
        public Color rayleighColor = Color.white;
        [Tooltip("The mie color multiplier.")]
        public Color mieColor = Color.white;
        [Tooltip("The scattering color multiplier.")]
        public Color scatteringColor = Color.white;

        // Outer space
        [Tooltip("The size of the sun texture.")]
        public float sunTextureSize = 1.5f;
        [Tooltip("The intensity of the sun texture.")]
        public float sunTextureIntensity = 1.0f;
        [Tooltip("The sun texture color multiplier.")]
        public Color sunTextureColor = Color.white;
        [Tooltip("The size of the moon texture.")]
        public float moonTextureSize = 1.5f;
        [Tooltip("The intensity of the moon texture.")]
        public float moonTextureIntensity = 1.0f;
        [Tooltip("The moon texture color multiplier.")]
        public Color moonTextureColor = Color.white;
        [Tooltip("The intensity of the regular stars.")]
        public float starsIntensity = 0.5f;
        [Tooltip("The intensity of the Milky Way.")]
        public float milkyWayIntensity = 0.0f;
        [Tooltip("The star field color multiplier.")]
        public Color starfieldColor = Color.white;
        [Tooltip("The rotation of the star field on the X axis.")]
        public float starfieldRotationX = 0.0f;
        [Tooltip("The rotation of the star field on the Y axis.")]
        public float starfieldRotationY = 0.0f;
        [Tooltip("The rotation of the star field on the Z axis.")]
        public float starfieldRotationZ = 0.0f;

        // Fog scattering
        [Tooltip("The scattering scale factor.")]
        public float fogScatteringScale = 1.0f;
        [Tooltip("The distance of the global fog scattering.")]
        public float globalFogDistance = 1000.0f;
        [Tooltip("The smooth step transition from where there is no global fog to where is completely foggy.")]
        public float globalFogSmoothStep = 0.25f;
        [Tooltip("The global fog scattering density.")]
        public float globalFogDensity = 1.0f;
        [Tooltip("The distance of the height fog scattering.")]
        public float heightFogDistance = 100.0f;
        [Tooltip("The smooth step transition from where there is no height fog to where is completely foggy.")]
        public float heightFogSmoothStep = 1.0f;
        [Tooltip("The height fog scattering density.")]
        public float heightFogDensity = 0.0f;
        [Tooltip("The height fog start height.")]
        public float heightFogStart = 0.0f;
        [Tooltip("The height fog end height.")]
        public float heightFogEnd = 100.0f;

        // Clouds
        [Tooltip("The altitude of the dynamic clouds in the sky.")]
        public float dynamicCloudsAltitude = 7.5f;
        [Tooltip("The movement direction of the dynamic clouds.")]
        public float dynamicCloudsDirection = 0.0f;
        [Tooltip("The movement speed of the dynamic clouds.")]
        public float dynamicCloudsSpeed = 0.1f;
        [Tooltip("The coverage of the dynamic clouds.")]
        public float dynamicCloudsDensity = 0.75f;
        [Tooltip("The first color of the dynamic clouds.")]
        public Color dynamicCloudsColor1 = Color.white;
        [Tooltip("The second color of the dynamic clouds.")]
        public Color dynamicCloudsColor2 = Color.white;
        private Vector2 m_dynamicCloudsDirection = Vector2.zero;
        public float staticCloudLayer1Speed = 0.0025f;
        public float staticCloudLayer2Speed = 0.0075f;
        private float m_staticCloudLayer1Speed = 0f;
        private float m_staticCloudLayer2Speed = 0f;
        public float staticCloudScattering = 1.0f;
        public float staticCloudExtinction = 1.5f;
        public float staticCloudSaturation = 2.5f;
        public float staticCloudOpacity = 1.25f;
        public Color staticCloudColor = Color.white;

        // Options
        [SerializeField]
        [Tooltip("The way the sky settings should be updated. By local material or by global shader properties.")]
        private AzureShaderUpdateMode m_shaderUpdateMode = AzureShaderUpdateMode.Global;
        [SerializeField]
        [Tooltip("The way the scattering color should be performed. Automatic by the controller or by your custom colors.")]
        private AzureScatteringMode m_scatteringMode = AzureScatteringMode.Automatic;
        [SerializeField]
        [Tooltip("The cloud render system.")]
        private AzureCloudMode m_cloudMode = AzureCloudMode.Dynamic;

        private Quaternion m_starfieldRotation;
        private Matrix4x4 m_starfieldRotationMatrix;

        private void Awake()
        {
            m_dynamicCloudsDirection = Vector2.zero;
            InitializeShaderUniforms();
        }

        private void OnEnable()
        {
            if (m_skyMaterial)
                RenderSettings.skybox = m_skyMaterial;
        }


        private void LateUpdate()
        {
            // In editor only
            #if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                InitializeShaderUniforms();

                if (m_skyMaterial)
                    RenderSettings.skybox = m_skyMaterial;
            }
            #endif

            // Clouds movement
            m_dynamicCloudsDirection = ComputeCloudPosition();
            m_staticCloudLayer1Speed += staticCloudLayer1Speed * Time.deltaTime;
            m_staticCloudLayer2Speed += staticCloudLayer2Speed * Time.deltaTime;
            if (m_staticCloudLayer1Speed >= 1.0f) { m_staticCloudLayer1Speed -= 1.0f; }
            if (m_staticCloudLayer2Speed >= 1.0f) { m_staticCloudLayer2Speed -= 1.0f; }
            UpdateShaderUniforms();
        }

        private void InitializeShaderUniforms()
        {
            switch (m_shaderUpdateMode)
            {
                case AzureShaderUpdateMode.Local:
                    m_skyMaterial.SetTexture(AzureShaderUniforms.SunTexture, m_sunTexture);
                    m_skyMaterial.SetTexture(AzureShaderUniforms.MoonTexture, m_moonTexture);
                    m_skyMaterial.SetTexture(AzureShaderUniforms.StarFieldTexture, m_starfieldTexture);
                    m_skyMaterial.SetTexture(AzureShaderUniforms.DynamicCloudTexture, m_dynamicCloudsTexture);
                    m_skyMaterial.SetTexture(AzureShaderUniforms.StaticCloudTexture, staticCloudTexture);
                    break;
                case AzureShaderUpdateMode.Global:
                    Shader.SetGlobalTexture(AzureShaderUniforms.SunTexture, m_sunTexture);
                    Shader.SetGlobalTexture(AzureShaderUniforms.MoonTexture, m_moonTexture);
                    Shader.SetGlobalTexture(AzureShaderUniforms.StarFieldTexture, m_starfieldTexture);
                    Shader.SetGlobalTexture(AzureShaderUniforms.DynamicCloudTexture, m_dynamicCloudsTexture);
                    Shader.SetGlobalTexture(AzureShaderUniforms.StaticCloudTexture, staticCloudTexture);
                    break;
            }
        }

        private void UpdateShaderUniforms()
        {
            m_starfieldRotation = Quaternion.Euler(starfieldRotationX, starfieldRotationY, starfieldRotationZ);
            m_starfieldRotationMatrix = Matrix4x4.TRS(Vector3.zero, m_starfieldRotation, Vector3.one);

            switch (m_shaderUpdateMode)
            {
                case AzureShaderUpdateMode.Local:
                    UpdateLocalShaderUniforms(m_skyMaterial);
                    UpdateLocalShaderUniforms(m_fogMaterial);
                    break;
                case AzureShaderUpdateMode.Global:
                    UpdateGlobalShaderUniforms();
                    break;
            }
        }

        private void UpdateLocalShaderUniforms(Material mat)
        {
            mat.SetVector(AzureShaderUniforms.SunDirection, transform.InverseTransformDirection(-m_sunTransform.forward));
            mat.SetVector(AzureShaderUniforms.MoonDirection, transform.InverseTransformDirection(-m_moonTransform.forward));
            mat.SetMatrix(AzureShaderUniforms.SunMatrix, m_sunTransform.worldToLocalMatrix);
            mat.SetMatrix(AzureShaderUniforms.MoonMatrix, m_moonTransform.worldToLocalMatrix);
            mat.SetMatrix(AzureShaderUniforms.UpDirectionMatrix, transform.worldToLocalMatrix);
            mat.SetInt(AzureShaderUniforms.ScatteringMode, (int)m_scatteringMode);
            mat.SetFloat(AzureShaderUniforms.Kr, kr * 1000f);
            mat.SetFloat(AzureShaderUniforms.Km, km * 1000f);
            mat.SetVector(AzureShaderUniforms.Rayleigh, ComputeRayleigh() * rayleigh);
            mat.SetVector(AzureShaderUniforms.Mie, ComputeMie() * mie);
            mat.SetFloat(AzureShaderUniforms.MieDistance, mieDistance);
            mat.SetFloat(AzureShaderUniforms.Scattering, scattering * 60f);
            mat.SetFloat(AzureShaderUniforms.Luminance, luminance);
            mat.SetFloat(AzureShaderUniforms.Exposure, exposure);
            mat.SetColor(AzureShaderUniforms.RayleighColor, rayleighColor);
            mat.SetColor(AzureShaderUniforms.MieColor, mieColor);
            mat.SetColor(AzureShaderUniforms.ScatteringColor, scatteringColor);
            mat.SetFloat(AzureShaderUniforms.SunTextureSize, sunTextureSize);
            mat.SetFloat(AzureShaderUniforms.SunTextureIntensity, sunTextureIntensity);
            mat.SetColor(AzureShaderUniforms.SunTextureColor, sunTextureColor);
            mat.SetFloat(AzureShaderUniforms.MoonTextureSize, moonTextureSize);
            mat.SetFloat(AzureShaderUniforms.MoonTextureIntensity, moonTextureIntensity);
            mat.SetColor(AzureShaderUniforms.MoonTextureColor, moonTextureColor);
            mat.SetFloat(AzureShaderUniforms.StarsIntensity, starsIntensity);
            mat.SetFloat(AzureShaderUniforms.MilkyWayIntensity, milkyWayIntensity);
            mat.SetColor(AzureShaderUniforms.StarFieldColor, starfieldColor);
            mat.SetMatrix(AzureShaderUniforms.StarFieldRotation, m_starfieldRotationMatrix);
            mat.SetFloat(AzureShaderUniforms.FogScatteringScale, fogScatteringScale);
            mat.SetFloat(AzureShaderUniforms.GlobalFogDistance, globalFogDistance);
            mat.SetFloat(AzureShaderUniforms.GlobalFogSmoothStep, globalFogSmoothStep);
            mat.SetFloat(AzureShaderUniforms.GlobalFogDensity, globalFogDensity);
            mat.SetFloat(AzureShaderUniforms.HeightFogDistance, heightFogDistance);
            mat.SetFloat(AzureShaderUniforms.HeightFogSmoothStep, heightFogSmoothStep);
            mat.SetFloat(AzureShaderUniforms.HeightFogDensity, heightFogDensity);
            mat.SetFloat(AzureShaderUniforms.HeightFogStart, heightFogStart);
            mat.SetFloat(AzureShaderUniforms.HeightFogEnd, heightFogEnd);
            mat.SetFloat(AzureShaderUniforms.DynamicCloudAltitude, dynamicCloudsAltitude);
            mat.SetVector(AzureShaderUniforms.DynamicCloudDirection, m_dynamicCloudsDirection);
            mat.SetFloat(AzureShaderUniforms.DynamicCloudDensity, Mathf.Lerp(25.0f, 0.0f, dynamicCloudsDensity));
            mat.SetVector(AzureShaderUniforms.DynamicCloudColor1, dynamicCloudsColor1);
            mat.SetVector(AzureShaderUniforms.DynamicCloudColor2, dynamicCloudsColor2);
            //mat.SetFloat(AzureShaderUniforms.StaticCloudInterpolator, staticCloudInterpolator);
            mat.SetFloat(AzureShaderUniforms.StaticCloudLayer1Speed, m_staticCloudLayer1Speed);
            mat.SetFloat(AzureShaderUniforms.StaticCloudLayer2Speed, m_staticCloudLayer2Speed);
            mat.SetFloat(AzureShaderUniforms.StaticCloudScattering, staticCloudScattering);
            mat.SetFloat(AzureShaderUniforms.StaticCloudExtinction, staticCloudExtinction);
            mat.SetFloat(AzureShaderUniforms.StaticCloudSaturation, staticCloudSaturation);
            mat.SetFloat(AzureShaderUniforms.StaticCloudOpacity, staticCloudOpacity);
            mat.SetVector(AzureShaderUniforms.StaticCloudColor, staticCloudColor);
        }

        private void UpdateGlobalShaderUniforms()
        {
            Shader.SetGlobalVector(AzureShaderUniforms.SunDirection, transform.InverseTransformDirection(-m_sunTransform.forward));
            Shader.SetGlobalVector(AzureShaderUniforms.MoonDirection, transform.InverseTransformDirection(-m_moonTransform.forward));
            Shader.SetGlobalMatrix(AzureShaderUniforms.SunMatrix, m_sunTransform.worldToLocalMatrix);
            Shader.SetGlobalMatrix(AzureShaderUniforms.MoonMatrix, m_moonTransform.worldToLocalMatrix);
            Shader.SetGlobalMatrix(AzureShaderUniforms.UpDirectionMatrix, transform.worldToLocalMatrix);
            Shader.SetGlobalInt(AzureShaderUniforms.ScatteringMode, (int)m_scatteringMode);
            Shader.SetGlobalFloat(AzureShaderUniforms.Kr, kr * 1000f);
            Shader.SetGlobalFloat(AzureShaderUniforms.Km, km * 1000f);
            Shader.SetGlobalVector(AzureShaderUniforms.Rayleigh, ComputeRayleigh() * rayleigh);
            Shader.SetGlobalVector(AzureShaderUniforms.Mie, ComputeMie() * mie);
            Shader.SetGlobalFloat(AzureShaderUniforms.MieDistance, mieDistance);
            Shader.SetGlobalFloat(AzureShaderUniforms.Scattering, scattering * 60f);
            Shader.SetGlobalFloat(AzureShaderUniforms.Luminance, luminance);
            Shader.SetGlobalFloat(AzureShaderUniforms.Exposure, exposure);
            Shader.SetGlobalColor(AzureShaderUniforms.RayleighColor, rayleighColor);
            Shader.SetGlobalColor(AzureShaderUniforms.MieColor, mieColor);
            Shader.SetGlobalColor(AzureShaderUniforms.ScatteringColor, scatteringColor);
            Shader.SetGlobalFloat(AzureShaderUniforms.SunTextureSize, sunTextureSize);
            Shader.SetGlobalFloat(AzureShaderUniforms.SunTextureIntensity, sunTextureIntensity);
            Shader.SetGlobalColor(AzureShaderUniforms.SunTextureColor, sunTextureColor);
            Shader.SetGlobalFloat(AzureShaderUniforms.MoonTextureSize, moonTextureSize);
            Shader.SetGlobalFloat(AzureShaderUniforms.MoonTextureIntensity, moonTextureIntensity);
            Shader.SetGlobalColor(AzureShaderUniforms.MoonTextureColor, moonTextureColor);
            Shader.SetGlobalFloat(AzureShaderUniforms.StarsIntensity, starsIntensity);
            Shader.SetGlobalFloat(AzureShaderUniforms.MilkyWayIntensity, milkyWayIntensity);
            Shader.SetGlobalColor(AzureShaderUniforms.StarFieldColor, starfieldColor);
            Shader.SetGlobalMatrix(AzureShaderUniforms.StarFieldRotation, m_starfieldRotationMatrix);
            Shader.SetGlobalFloat(AzureShaderUniforms.FogScatteringScale, fogScatteringScale);
            Shader.SetGlobalFloat(AzureShaderUniforms.GlobalFogDistance, globalFogDistance);
            Shader.SetGlobalFloat(AzureShaderUniforms.GlobalFogSmoothStep, globalFogSmoothStep);
            Shader.SetGlobalFloat(AzureShaderUniforms.GlobalFogDensity, globalFogDensity);
            Shader.SetGlobalFloat(AzureShaderUniforms.HeightFogDistance, heightFogDistance);
            Shader.SetGlobalFloat(AzureShaderUniforms.HeightFogSmoothStep, heightFogSmoothStep);
            Shader.SetGlobalFloat(AzureShaderUniforms.HeightFogDensity, heightFogDensity);
            Shader.SetGlobalFloat(AzureShaderUniforms.HeightFogStart, heightFogStart);
            Shader.SetGlobalFloat(AzureShaderUniforms.HeightFogEnd, heightFogEnd);
            Shader.SetGlobalFloat(AzureShaderUniforms.DynamicCloudAltitude, dynamicCloudsAltitude);
            Shader.SetGlobalVector(AzureShaderUniforms.DynamicCloudDirection, m_dynamicCloudsDirection);
            Shader.SetGlobalFloat(AzureShaderUniforms.DynamicCloudDensity, Mathf.Lerp(25.0f, 0.0f, dynamicCloudsDensity));
            Shader.SetGlobalVector(AzureShaderUniforms.DynamicCloudColor1, dynamicCloudsColor1);
            Shader.SetGlobalVector(AzureShaderUniforms.DynamicCloudColor2, dynamicCloudsColor2);
            //Shader.SetGlobalFloat(AzureShaderUniforms.StaticCloudInterpolator, staticCloudInterpolator);
            Shader.SetGlobalFloat(AzureShaderUniforms.StaticCloudLayer1Speed, m_staticCloudLayer1Speed);
            Shader.SetGlobalFloat(AzureShaderUniforms.StaticCloudLayer2Speed, m_staticCloudLayer2Speed);
            Shader.SetGlobalFloat(AzureShaderUniforms.StaticCloudScattering, staticCloudScattering);
            Shader.SetGlobalFloat(AzureShaderUniforms.StaticCloudExtinction, staticCloudExtinction);
            Shader.SetGlobalFloat(AzureShaderUniforms.StaticCloudSaturation, staticCloudSaturation);
            Shader.SetGlobalFloat(AzureShaderUniforms.StaticCloudOpacity, staticCloudOpacity);
            Shader.SetGlobalVector(AzureShaderUniforms.StaticCloudColor, staticCloudColor);
        }

        /// <summary>
        /// Total rayleigh computation.
        /// </summary>
        private Vector3 ComputeRayleigh()
        {
            Vector3 rayleigh = Vector3.one;
            Vector3 lambda = new Vector3(wavelengthR, wavelengthG, wavelengthB) * 1e-9f;
            float n = 1.0003f; // Refractive index of air
            float pn = 0.035f; // Depolarization factor for standard air.
            float n2 = n * n;
            //float N = 2.545E25f;
            float N = molecularDensity * 1E25f;
            float temp = (8.0f * Mathf.PI * Mathf.PI * Mathf.PI * ((n2 - 1.0f) * (n2 - 1.0f))) / (3.0f * N) * ((6.0f + 3.0f * pn) / (6.0f - 7.0f * pn));

            rayleigh.x = temp / Mathf.Pow(lambda.x, 4.0f);
            rayleigh.y = temp / Mathf.Pow(lambda.y, 4.0f);
            rayleigh.z = temp / Mathf.Pow(lambda.z, 4.0f);

            return rayleigh;
        }

        /// <summary>
        /// Total mie computation.
        /// </summary>
        private Vector3 ComputeMie()
        {
            Vector3 mie;

            //float c = (0.6544f * Turbidity - 0.6510f) * 1e-16f;
            float c = (0.6544f * 5.0f - 0.6510f) * 10f * 1e-9f;
            Vector3 k = new Vector3(686.0f, 678.0f, 682.0f);

            mie.x = (434.0f * c * Mathf.PI * Mathf.Pow((4.0f * Mathf.PI) / wavelengthR, 2.0f) * k.x);
            mie.y = (434.0f * c * Mathf.PI * Mathf.Pow((4.0f * Mathf.PI) / wavelengthG, 2.0f) * k.y);
            mie.z = (434.0f * c * Mathf.PI * Mathf.Pow((4.0f * Mathf.PI) / wavelengthB, 2.0f) * k.z);

            //float c = (6544f * 5.0f - 6510f) * 10.0f * 1.0e-9f;
            //mie.x = (0.434f * c * Mathf.PI * Mathf.Pow((2.0f * Mathf.PI) / wavelengthR, 2.0f) * k.x) / 3.0f;
            //mie.y = (0.434f * c * Mathf.PI * Mathf.Pow((2.0f * Mathf.PI) / wavelengthG, 2.0f) * k.y) / 3.0f;
            //mie.z = (0.434f * c * Mathf.PI * Mathf.Pow((2.0f * Mathf.PI) / wavelengthB, 2.0f) * k.z) / 3.0f;

            return mie;
        }

        /// <summary>
        /// Returns the cloud uv position based on the direction and speed.
        /// </summary>
        private Vector2 ComputeCloudPosition()
        {
            float x = m_dynamicCloudsDirection.x;
            float z = m_dynamicCloudsDirection.y;
            float dir = Mathf.Lerp(0f, 360f, dynamicCloudsDirection);
            float windSpeed = dynamicCloudsSpeed * 0.05f * Time.deltaTime;

            x += windSpeed * Mathf.Sin(0.01745329f * dir);
            z += windSpeed * Mathf.Cos(0.01745329f * dir);

            if (x >= 1.0f) x -= 1.0f;
            if (z >= 1.0f) z -= 1.0f;

            return new Vector2(x, z);
        }

        /// <summary>
        /// Updates the material settings if there is a change from Inspector.
        /// </summary>
        public void UpdateSkySettings()
        {
            switch (m_cloudMode)
            {
                case AzureCloudMode.Off:
                    m_skyMaterial.shader = m_emptySkyShader;
                    break;
                case AzureCloudMode.Static:
                    m_skyMaterial.shader = m_staticCloudsShader;
                    break;
                case AzureCloudMode.Dynamic:
                    m_skyMaterial.shader = m_dynamicCloudsShader;
                    break;
            }
        }
    }
}