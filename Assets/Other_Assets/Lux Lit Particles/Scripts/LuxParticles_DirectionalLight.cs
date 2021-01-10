using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using System;

namespace LuxParticles {

	[ExecuteInEditMode]
	[RequireComponent(typeof(Light))]
	public class LuxParticles_DirectionalLight : MonoBehaviour {

		Light m_light;
		private CommandBuffer GetShadowCascades_CB;

		// Use this for initialization
		void OnEnable () {

			m_light = GetComponent<Light>();

			if(GetShadowCascades_CB == null) {
				GetShadowCascades_CB = new CommandBuffer();
				GetShadowCascades_CB.name = "LuxParticles GetShadowCascades";
				GetShadowCascades_CB.SetGlobalTexture("_LuxParticles_CascadedShadowMap", BuiltinRenderTextureType.CurrentActive);
			}

			m_light.AddCommandBuffer(LightEvent.AfterShadowMap, GetShadowCascades_CB);
		}

		void OnDisable() {
			if(GetComponent<Light>()) {
				if(GetShadowCascades_CB != null)
					GetComponent<Light>().RemoveCommandBuffer(LightEvent.AfterShadowMap, GetShadowCascades_CB);
			}

			#if UNITY_EDITOR
				OnDestroy(); // release buffers
			#endif

		}

		void OnDestroy() {
			if(GetShadowCascades_CB != null) {
				GetShadowCascades_CB.Release();
				GetShadowCascades_CB = null;
			}
		}
	}
}