using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LuxParticles {

	[ExecuteInEditMode]
	public class LuxParticles_LocalAmbientLighting : MonoBehaviour {

		public static List<LuxParticles_LocalAmbientLighting> LocalProbes = new List<LuxParticles_LocalAmbientLighting>();

// particle systems can move!
// particle systems might be spawned after start (so we have to call UpdateAmbientLighting as it might be not per frame )

		public Vector3 SampleOffset = Vector3.zero;
		
		[System.NonSerialized] public Transform trans;
		[System.NonSerialized] public Renderer rend;
		[System.NonSerialized] public MaterialPropertyBlock m_block;
		[System.NonSerialized] public bool IsVisible;

		void OnEnable () {
			trans = this.GetComponent<Transform>();
			rend = this.GetComponent<Renderer>();
			m_block = new MaterialPropertyBlock();
		//	Make sure IsVisible = true so SH lighting will be set OnEnable
			IsVisible = true;
			Register();
		}

		void Register() {
			LocalProbes.Add(this);
		}

		void OnDisable() {
			LocalProbes.Remove(this);
			if (m_block != null) {
				m_block.Clear();
				rend.SetPropertyBlock(m_block);
				m_block = null;
			}
		}

		void OnDestroy() {
			LocalProbes.Remove(this);
			if (m_block != null) {
				m_block.Clear();
				rend.SetPropertyBlock(m_block);
				m_block = null;
			}
		}

		void OnBecameVisible () {
			IsVisible = true;
		}

		void OnBecameInvisible () {
			IsVisible = false;
		}
	}
}