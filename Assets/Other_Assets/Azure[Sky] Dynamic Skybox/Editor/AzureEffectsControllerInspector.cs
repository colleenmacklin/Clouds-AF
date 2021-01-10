using UnityEngine.AzureSky;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor.AzureSky
{
	[CustomEditor(typeof(AzureEffectsController))]
	public class AzureEffectsControllerInspector : Editor
	{
		// Editor only
		private AzureEffectsController m_target;
		private readonly Color m_greenColor = new Color(0.85f, 1.0f, 0.85f);
		private readonly Color m_redColor = new Color(1.0f, 0.75f, 0.75f);
		
		// GUIContents
        private readonly GUIContent[] m_guiContent = new[]
        {
	        new GUIContent("Wind Zone", "The wind zone."),
	        new GUIContent("Wind Multiplier", "The wind zone multiplier."),
	        new GUIContent("Light Rain", "The light rain audio source."),
	        new GUIContent("Medium Rain", "The medium rain audio source."),
	        new GUIContent("Heavy Rain", "The heavy rain audio source."),
	        new GUIContent("Light Wind", "The light wind audio source."),
	        new GUIContent("Medium Wind", "The medium wind audio source."),
	        new GUIContent("Heavy Wind", "The heavy wind audio source."),
	        new GUIContent("Particle System Transform", "The particle system parent transform."),
	        new GUIContent("Rain Material", "The material used to render the rain particles."),
	        new GUIContent("Heavy Rain Material", "The material used to render the heavy rain particles."),
	        new GUIContent("Snow Material", "The material used to render the snow particles."),
	        new GUIContent("Ripple Material", "The material used to render the ripple particles."),
	        new GUIContent("Light Rain Particle", "The light rain particle system."),
	        new GUIContent("Medium Rain Particle", "The medium rain particle system."),
	        new GUIContent("Heavy Rain Particle", "The heavy rain particle system."),
	        new GUIContent("Snow Particle", "The snow particle system."),
	        new GUIContent("Follow Target", "If selected, the particle system will follow this transform in the scene, usually the main camera is used here."),
	        new GUIContent("Thunder Prefab", "The prefab that will be instantiated to create the thunder effect."),
	        new GUIContent("Audio Clip", "The audio clip used for this instance of the thunder effect."),
	        new GUIContent("Light Frequency", "The light intensity frequency of this thunder effect."),
	        new GUIContent("Audio Delay", "The delay time to play the audio after this thunder effect has been created."),
	        new GUIContent("Position:", "The position in the world where the thunder must be instantiated."),
        };
		
		// Serialized Properties
		private SerializedProperty m_showWindHeaderGroup;
		private SerializedProperty m_showSoundsHeaderGroup;
		private SerializedProperty m_showParticlesHeaderGroup;
		private SerializedProperty m_showThundersHeaderGroup;
		private SerializedProperty m_windZone;
		private SerializedProperty m_windMultiplier;
		private SerializedProperty m_lightRainSoundFx;
		private SerializedProperty m_mediumRainSoundFx;
		private SerializedProperty m_heavyRainSoundFx;
		private SerializedProperty m_lightWindSoundFx;
		private SerializedProperty m_mediumWindSoundFx;
		private SerializedProperty m_heavyWindSoundFx;
		private SerializedProperty m_particleSystemTransform;
		private SerializedProperty m_rainMaterial;
		private SerializedProperty m_heavyRainMaterial;
		private SerializedProperty m_snowMaterial;
		private SerializedProperty m_rippleMaterial;
		private SerializedProperty m_lightRainParticle;
		private SerializedProperty m_mediumRainParticle;
		private SerializedProperty m_heavyRainParticle;
		private SerializedProperty m_snowParticle;
		private SerializedProperty m_followTarget;
		
		private SerializedProperty m_thunderSettingsList;
		private ReorderableList m_reorderableThunderList;

		private void OnEnable()
		{
			// Get Target
			m_target = (AzureEffectsController) target;
			
			// Find the serialized properties
			m_showWindHeaderGroup = serializedObject.FindProperty("showWindHeaderGroup");
			m_showSoundsHeaderGroup = serializedObject.FindProperty("showSoundsHeaderGroup");
			m_showParticlesHeaderGroup = serializedObject.FindProperty("showParticlesHeaderGroup");
			m_showThundersHeaderGroup = serializedObject.FindProperty("showThundersHeaderGroup");
			m_windZone = serializedObject.FindProperty("windZone");
			m_windMultiplier = serializedObject.FindProperty("windMultiplier");
			m_lightRainSoundFx = serializedObject.FindProperty("lightRainSoundFx");
			m_mediumRainSoundFx = serializedObject.FindProperty("mediumRainSoundFx");
			m_heavyRainSoundFx = serializedObject.FindProperty("heavyRainSoundFx");
			m_lightWindSoundFx = serializedObject.FindProperty("lightWindSoundFx");
			m_mediumWindSoundFx = serializedObject.FindProperty("mediumWindSoundFx");
			m_heavyWindSoundFx = serializedObject.FindProperty("heavyWindSoundFx");
			m_particleSystemTransform = serializedObject.FindProperty("particleSystemTransform");
			m_rainMaterial = serializedObject.FindProperty("rainMaterial");
			m_heavyRainMaterial = serializedObject.FindProperty("heavyRainMaterial");
			m_snowMaterial = serializedObject.FindProperty("snowMaterial");
			m_rippleMaterial = serializedObject.FindProperty("rippleMaterial");
			m_lightRainParticle = serializedObject.FindProperty("lightRainParticle");
			m_mediumRainParticle = serializedObject.FindProperty("mediumRainParticle");
			m_heavyRainParticle = serializedObject.FindProperty("heavyRainParticle");
			m_snowParticle = serializedObject.FindProperty("snowParticle");
			m_followTarget = serializedObject.FindProperty("followTarget");
			
			m_thunderSettingsList = serializedObject.FindProperty("thunderSettingsList");
			
			// Create thunder list
			m_reorderableThunderList = new ReorderableList(serializedObject, m_thunderSettingsList, true, true, true, true)
			{
				drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
				{
					rect.y += 2;
					var element = m_reorderableThunderList.serializedProperty.GetArrayElementAtIndex(index);
					var height = EditorGUIUtility.singleLineHeight;
					
					element.isExpanded = EditorGUI.Toggle(new Rect(rect.x - 25, rect.y, rect.width + 25, height), GUIContent.none, element.isExpanded, EditorStyles.foldoutHeader);
					EditorGUI.LabelField(new Rect(rect.x + 5, rect.y, rect.width - 5, height), "Thunder " + index.ToString(), EditorStyles.miniLabel);
					
					if (element.isExpanded)
					{
						rect.y += height +1;
						EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, height), element.FindPropertyRelative("thunderPrefab"), m_guiContent[18]);
						
						rect.y += height +1;
						EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, height), element.FindPropertyRelative("audioClip"), m_guiContent[19]);
						
						rect.y += height +1;
						EditorGUI.CurveField(new Rect(rect.x, rect.y, rect.width, height), element.FindPropertyRelative("lightFrequency"), Color.yellow, new Rect(0.0f, 0.0f, 1.0f, 1.0f), m_guiContent[20]);
						
						rect.y += height +1;
						EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, height), element.FindPropertyRelative("audioDelay"), m_guiContent[21]);
						
						rect.y += height +1;
						EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, height), m_guiContent[22]);
						
						rect.y += height +1;
						EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, height), element.FindPropertyRelative("position"), GUIContent.none);
						
						rect.y += height +1;
						if (GUI.Button(new Rect(rect.x + 15, rect.y, rect.width - 15, height), "Test"))
						{
							if(Application.isPlaying)
								m_target.InstantiateThunderEffect(index);
							else
								Debug.Log("The application must be playing to perform a thunder effect.");
						}
					}
				},
				
				elementHeightCallback = (int index) =>
				{
					var element = m_reorderableThunderList.serializedProperty.GetArrayElementAtIndex(index);
					var elementHeight = EditorGUI.GetPropertyHeight(element);
					var margin = EditorGUIUtility.standardVerticalSpacing;
					if (element.isExpanded) return elementHeight + 40; else return elementHeight + margin;
				},

				drawHeaderCallback = (Rect rect) =>
				{
					EditorGUI.LabelField(rect,new GUIContent("Thunder List", ""), EditorStyles.boldLabel);
				},

				drawElementBackgroundCallback = (rect, index, active, focused) =>
				{
					if (active)
						GUI.Box(new Rect(rect.x +2, rect.y -1, rect.width -4, rect.height +1), "","selectionRect");
				},
				
				onAddCallback = (ReorderableList l) =>
				{
					var index = l.serializedProperty.arraySize;
					l.serializedProperty.arraySize++;
					l.index = index;
					var element = l.serializedProperty.GetArrayElementAtIndex(index);
					element.FindPropertyRelative("lightFrequency").animationCurveValue = AnimationCurve.Constant(0.0f, 1.0f, 0.0f);
				}
			};
		}

		public override void OnInspectorGUI()
		{
			// Start custom Inspector
			serializedObject.Update();
			EditorGUI.BeginChangeCheck();
			
			// Wind header group
			m_showWindHeaderGroup.isExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(m_showWindHeaderGroup.isExpanded, "Wind");
			if (m_showWindHeaderGroup.isExpanded)
			{
				EditorGUI.indentLevel++;
				GUI.color = m_greenColor;
				if (!m_target.windZone) GUI.color = m_redColor;
				EditorGUILayout.PropertyField(m_windZone, m_guiContent[0]);
				GUI.color = Color.white;
				EditorGUILayout.PropertyField(m_windMultiplier, m_guiContent[1]);
				EditorGUI.indentLevel--;
			}
			EditorGUILayout.EndFoldoutHeaderGroup();
			
			GUILayout.Space(2);
			
			// Sounds header group
			m_showSoundsHeaderGroup.isExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(m_showSoundsHeaderGroup.isExpanded, "Sounds");
			if (m_showSoundsHeaderGroup.isExpanded)
			{
				EditorGUI.indentLevel++;
				
				// Light rain sound fx
				GUI.color = m_greenColor;
				if (!m_target.lightRainSoundFx) GUI.color = m_redColor;
				EditorGUILayout.PropertyField(m_lightRainSoundFx, m_guiContent[2]);
				
				// Medium rain sound fx
				GUI.color = m_greenColor;
				if (!m_target.mediumRainSoundFx) GUI.color = m_redColor;
				EditorGUILayout.PropertyField(m_mediumRainSoundFx, m_guiContent[3]);
				
				// Heavy rain sound fx
				GUI.color = m_greenColor;
				if (!m_target.heavyRainSoundFx) GUI.color = m_redColor;
				EditorGUILayout.PropertyField(m_heavyRainSoundFx, m_guiContent[4]);
				
				// Light wind sound fx
				GUI.color = m_greenColor;
				if (!m_target.lightWindSoundFx) GUI.color = m_redColor;
				EditorGUILayout.PropertyField(m_lightWindSoundFx, m_guiContent[5]);
				
				// Medium wind sound fx
				GUI.color = m_greenColor;
				if (!m_target.mediumWindSoundFx) GUI.color = m_redColor;
				EditorGUILayout.PropertyField(m_mediumWindSoundFx, m_guiContent[6]);
				
				// Heavy wind sound fx
				GUI.color = m_greenColor;
				if (!m_target.heavyWindSoundFx) GUI.color = m_redColor;
				EditorGUILayout.PropertyField(m_heavyWindSoundFx, m_guiContent[7]);
				GUI.color = Color.white;
				EditorGUI.indentLevel--;
			}
			EditorGUILayout.EndFoldoutHeaderGroup();
			
			GUILayout.Space(2);
			
			// Particles header group
			m_showParticlesHeaderGroup.isExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(m_showParticlesHeaderGroup.isExpanded, "Particles");
			if (m_showParticlesHeaderGroup.isExpanded)
			{
				EditorGUI.indentLevel++;
				
				// Follow target
				GUI.color = m_greenColor;
				if (!m_target.followTarget) GUI.color = m_redColor;
				EditorGUILayout.PropertyField(m_followTarget, m_guiContent[17]);
				
				// Particle system transform
				GUI.color = m_greenColor;
				if (!m_target.particleSystemTransform) GUI.color = m_redColor;
				EditorGUILayout.PropertyField(m_particleSystemTransform, m_guiContent[8]);
				
				// Rain material
				GUI.color = m_greenColor;
				if (!m_target.rainMaterial) GUI.color = m_redColor;
				EditorGUILayout.PropertyField(m_rainMaterial, m_guiContent[9]);
				
				// Heavy rain material
				GUI.color = m_greenColor;
				if (!m_target.heavyRainMaterial) GUI.color = m_redColor;
				EditorGUILayout.PropertyField(m_heavyRainMaterial, m_guiContent[10]);
				
				// Snow material
				GUI.color = m_greenColor;
				if (!m_target.snowMaterial) GUI.color = m_redColor;
				EditorGUILayout.PropertyField(m_snowMaterial, m_guiContent[11]);
				
				// Ripple material
				GUI.color = m_greenColor;
				if (!m_target.rippleMaterial) GUI.color = m_redColor;
				EditorGUILayout.PropertyField(m_rippleMaterial, m_guiContent[12]);
				
				// Light rain particle
				GUI.color = m_greenColor;
				if (!m_target.lightRainParticle) GUI.color = m_redColor;
				EditorGUILayout.PropertyField(m_lightRainParticle, m_guiContent[13]);
				
				// Medium rain particle
				GUI.color = m_greenColor;
				if (!m_target.mediumRainParticle) GUI.color = m_redColor;
				EditorGUILayout.PropertyField(m_mediumRainParticle, m_guiContent[14]);
				
				// Heavy rain particle
				GUI.color = m_greenColor;
				if (!m_target.heavyRainParticle) GUI.color = m_redColor;
				EditorGUILayout.PropertyField(m_heavyRainParticle, m_guiContent[15]);
				
				// Snow particle
				GUI.color = m_greenColor;
				if (!m_target.snowParticle) GUI.color = m_redColor;
				EditorGUILayout.PropertyField(m_snowParticle, m_guiContent[16]);
				
				GUI.color = Color.white;
				EditorGUI.indentLevel--;
			}
			EditorGUILayout.EndFoldoutHeaderGroup();
			
			GUILayout.Space(2);
			
			// Thunder header group
			m_showThundersHeaderGroup.isExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(m_showThundersHeaderGroup.isExpanded, "Thunders");
			if (m_showThundersHeaderGroup.isExpanded)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.Space();
				m_reorderableThunderList.DoLayoutList();
				EditorGUI.indentLevel--;
			}
			EditorGUILayout.EndFoldoutHeaderGroup();
			
			
			// End custom Inspector
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(m_target, "Undo Azure Effects Controller");
				serializedObject.ApplyModifiedProperties();
			}
		}
	}
}