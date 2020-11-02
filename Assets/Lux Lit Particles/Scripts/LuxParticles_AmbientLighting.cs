using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace LuxParticles {

    [ExecuteInEditMode]
    public class LuxParticles_AmbientLighting : MonoBehaviour {

        public bool UpdatePerFrame = true;
        public bool AlwaysUseSH = false;

    	private SphericalHarmonicsL2 probe;
    	private Vector4[] SHLighting = new Vector4[7];

    	private int Lux_SHAr;
    	private int Lux_SHAg;
    	private int Lux_SHAb;
    	private int Lux_SHBr;
    	private int Lux_SHBg;
    	private int Lux_SHBb;
    	private int Lux_SHC;

        private int Lux_L_SHAr;
        private int Lux_L_SHAg;
        private int Lux_L_SHAb;
        private int Lux_L_SHBr;
        private int Lux_L_SHBg;
        private int Lux_L_SHBb;
        private int Lux_L_SHC;

        private int Lux_AmbientMode;

        const float k0 = 0.28209479177387814347f; // {0, 0} : 1/2 * sqrt(1/Pi)
        const float k1 = 0.48860251190291992159f; // {1, 0} : 1/2 * sqrt(3/Pi)
        const float k2 = 1.09254843059207907054f; // {2,-2} : 1/2 * sqrt(15/Pi)
        const float k3 = 0.31539156525252000603f; // {2, 0} : 1/4 * sqrt(5/Pi)
        const float k4 = 0.54627421529603953527f; // {2, 2} : 1/4 * sqrt(15/Pi)
        private static float[] ks = {k0, -k1, k1, -k1, k2, -k2, k3, -k2, k4};

        private int managedParticleSystems = 0;

        void OnEnable() {
            Lux_SHAr = Shader.PropertyToID("_Lux_SHAr");
    		Lux_SHAg = Shader.PropertyToID("_Lux_SHAg");
    		Lux_SHAb = Shader.PropertyToID("_Lux_SHAb");
    		Lux_SHBr = Shader.PropertyToID("_Lux_SHBr");
    		Lux_SHBg = Shader.PropertyToID("_Lux_SHBg");
    		Lux_SHBb = Shader.PropertyToID("_Lux_SHBb");
    		Lux_SHC = Shader.PropertyToID("_Lux_SHC");

            Lux_L_SHAr = Shader.PropertyToID("_Lux_L_SHAr");
            Lux_L_SHAg = Shader.PropertyToID("_Lux_L_SHAg");
            Lux_L_SHAb = Shader.PropertyToID("_Lux_L_SHAb");
            Lux_L_SHBr = Shader.PropertyToID("_Lux_L_SHBr");
            Lux_L_SHBg = Shader.PropertyToID("_Lux_L_SHBg");
            Lux_L_SHBb = Shader.PropertyToID("_Lux_L_SHBb");
            Lux_L_SHC = Shader.PropertyToID("_Lux_L_SHC");

            Lux_AmbientMode = Shader.PropertyToID("_Lux_AmbientMode");

            
        //  Wait one frame before initializing lighting so particles may register and Light Probes are available
            Invoke("UpdateAmbientLighting", 0.0f);
            //UpdateAmbientLighting();
        }

        void LateUpdate() {

            #if UNITY_EDITOR
                if (!Application.isPlaying) {
                    UpdateAmbientLighting();
                }
                else {
                    if (UpdatePerFrame) {
                        UpdateAmbientLighting();
                    }
                //  In case new particle systems have registered we have to set their ambient lighting
                    else if (LuxParticles_LocalAmbientLighting.LocalProbes != null) {
                        if(managedParticleSystems < LuxParticles_LocalAmbientLighting.LocalProbes.Count) {
                            UpdateAmbientLightingForNewParticleSystems();
                        }
                        managedParticleSystems = LuxParticles_LocalAmbientLighting.LocalProbes.Count;
                    }
                }

            #else
                if (UpdatePerFrame) {
                    UpdateAmbientLighting();
                }
            //  In case new particle systems have registered we have to set their ambient lighting
                else if (LuxParticles_LocalAmbientLighting.LocalProbes != null) {
                    if(managedParticleSystems < LuxParticles_LocalAmbientLighting.LocalProbes.Count) {
                        UpdateAmbientLightingForNewParticleSystems();
                    }
                    managedParticleSystems = LuxParticles_LocalAmbientLighting.LocalProbes.Count;
                }
            #endif
        }

        public void UpdateAmbientLighting() {

            bool UseLocalProbes = false;
            if ( LuxParticles_LocalAmbientLighting.LocalProbes != null) {
                if (LuxParticles_LocalAmbientLighting.LocalProbes.Count > 0) {
                    UseLocalProbes = true;
                }
            }

            if (RenderSettings.ambientMode == AmbientMode.Flat && !UseLocalProbes && !AlwaysUseSH) {
                Shader.SetGlobalFloat(Lux_AmbientMode, 0.0f);
            }
            else if (RenderSettings.ambientMode == AmbientMode.Trilight && !UseLocalProbes && !AlwaysUseSH) {
                Shader.SetGlobalFloat(Lux_AmbientMode, 1.0f);
            }
            else {
                Shader.SetGlobalFloat(Lux_AmbientMode, 2.0f);
                
                if(RenderSettings.ambientMode == AmbientMode.Skybox) {
                    probe = RenderSettings.ambientProbe;
                }
                else {
                //  In case ambientMode is set to Color or Trilight we sample Probe Lighting.
                    LightProbes.GetInterpolatedProbe(this.transform.position, null, out probe);
                }

                PremultiplyCoefficients(probe);
                GetShaderConstantsFromNormalizedSH(ref probe, true);
                SetSHLighting();

                if (LuxParticles_LocalAmbientLighting.LocalProbes != null) {
                    //Debug.Log(LuxParticles_LocalAmbientLighting.LocalProbes.Count);
                    for(int i = 0; i != LuxParticles_LocalAmbientLighting.LocalProbes.Count; i++) {
                        var CurrentProbe = LuxParticles_LocalAmbientLighting.LocalProbes[i];

                    //  Only update ambient lighting for visible particle systems
                        if (!CurrentProbe.IsVisible) {
                            continue;
                        }

                        LightProbes.GetInterpolatedProbe(CurrentProbe.trans.position + CurrentProbe.SampleOffset, null, out probe);
                        PremultiplyCoefficients(probe);
                        GetShaderConstantsFromNormalizedSH(ref probe, false);
                    //      Update materialpropertyblock
                        //m_Renderer.SetPropertyBlock(TouchMaterialBlock);
                        var m_block = LuxParticles_LocalAmbientLighting.LocalProbes[i].m_block;
                        m_block.Clear();
                        m_block.SetVector(Lux_L_SHAr, SHLighting[0] );
                        m_block.SetVector(Lux_L_SHAg, SHLighting[1] );
                        m_block.SetVector(Lux_L_SHAb, SHLighting[2] );
                        m_block.SetVector(Lux_L_SHBr, SHLighting[3] );
                        m_block.SetVector(Lux_L_SHBg, SHLighting[4] );
                        m_block.SetVector(Lux_L_SHBb, SHLighting[5] );
                        m_block.SetVector(Lux_L_SHC, SHLighting[6] );
                        LuxParticles_LocalAmbientLighting.LocalProbes[i].rend.SetPropertyBlock(m_block);
                    }
                }
            }
        }

        public void UpdateAmbientLightingForNewParticleSystems() {
            var total = LuxParticles_LocalAmbientLighting.LocalProbes.Count;
            for(int i = managedParticleSystems; i != total; i++) {
                var CurrentProbe = LuxParticles_LocalAmbientLighting.LocalProbes[i];
                LightProbes.GetInterpolatedProbe(CurrentProbe.trans.position + CurrentProbe.SampleOffset, null, out probe);
                PremultiplyCoefficients(probe);
                GetShaderConstantsFromNormalizedSH(ref probe, false);
                var m_block = LuxParticles_LocalAmbientLighting.LocalProbes[i].m_block;
                m_block.Clear();
                m_block.SetVector(Lux_L_SHAr, SHLighting[0] );
                m_block.SetVector(Lux_L_SHAg, SHLighting[1] );
                m_block.SetVector(Lux_L_SHAb, SHLighting[2] );
                m_block.SetVector(Lux_L_SHBr, SHLighting[3] );
                m_block.SetVector(Lux_L_SHBg, SHLighting[4] );
                m_block.SetVector(Lux_L_SHBb, SHLighting[5] );
                m_block.SetVector(Lux_L_SHC, SHLighting[6] );
                LuxParticles_LocalAmbientLighting.LocalProbes[i].rend.SetPropertyBlock(m_block);
            }
        }

        private static SphericalHarmonicsL2 PremultiplyCoefficients(SphericalHarmonicsL2 sh) {
            for (int c = 0; c < 3; c++) {
                for (int i = 0; i < 9; i++) {
                    sh[c, i] *= ks[i];
                }
            }
            return sh;
        }

    //	Prepare spherical harmonics values for efficient evaluation in a shader
    //	Please note: This needs linear color space!
        private void GetShaderConstantsFromNormalizedSH(ref SphericalHarmonicsL2 ambientProbe, bool IsSkyLighting) {

        //  ambientintensity is only needed by SH lighting from the sky
        	var ambientIntensity = 1.0f;
        //  ambient intensity needed in 5.6.3, 2017.1, 2018.3
            if (IsSkyLighting) {
                ambientIntensity = RenderSettings.ambientIntensity;
                if (QualitySettings.activeColorSpace == ColorSpace.Linear) {
                    ambientIntensity = Mathf.Pow(ambientIntensity, 2.2f);
                }
            }

            for (int channelIdx = 0; channelIdx < 3; ++channelIdx) {
                // Constant + Linear
                // In the shader we multiply the normal is not swizzled, so it's normal.xyz.
                // Swizzle the coefficients to be in { x, y, z, DC } order.
                SHLighting[channelIdx].x = ambientProbe[channelIdx, 3] * ambientIntensity;
                SHLighting[channelIdx].y = ambientProbe[channelIdx, 1] * ambientIntensity;
                SHLighting[channelIdx].z = ambientProbe[channelIdx, 2] * ambientIntensity;
                SHLighting[channelIdx].w = (ambientProbe[channelIdx, 0] - ambientProbe[channelIdx, 6]) * ambientIntensity;
                // Quadratic polynomials
                SHLighting[channelIdx + 3].x = ambientProbe[channelIdx, 4] * ambientIntensity;
                SHLighting[channelIdx + 3].y = ambientProbe[channelIdx, 5] * ambientIntensity;
                SHLighting[channelIdx + 3].z = ambientProbe[channelIdx, 6] * 3.0f * ambientIntensity;
                SHLighting[channelIdx + 3].w = ambientProbe[channelIdx, 7] * ambientIntensity;
            }
            // Final quadratic polynomial
            SHLighting[6].x = ambientProbe[0, 8] * ambientIntensity;
            SHLighting[6].y = ambientProbe[1, 8] * ambientIntensity;
            SHLighting[6].z = ambientProbe[2, 8] * ambientIntensity;
            SHLighting[6].w = 1.0f;
        }

        private void SetSHLighting() {
            Shader.SetGlobalVector(Lux_SHAr, SHLighting[0] );
            Shader.SetGlobalVector(Lux_SHAg, SHLighting[1] );
            Shader.SetGlobalVector(Lux_SHAb, SHLighting[2] );
            Shader.SetGlobalVector(Lux_SHBr, SHLighting[3] );
            Shader.SetGlobalVector(Lux_SHBg, SHLighting[4] );
            Shader.SetGlobalVector(Lux_SHBb, SHLighting[5] );
            Shader.SetGlobalVector(Lux_SHC, SHLighting[6] );
        }
    }
}