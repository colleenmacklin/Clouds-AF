using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AzureSky;

namespace UnityEditor.AzureSky
{
    [CustomEditor(typeof(AzureSkyRenderController))]
    public class AzureSkyRenderControllerInspector : Editor
    {
        // Target
        private AzureSkyRenderController m_target;
        private Rect m_controlRect;
        private Rect m_headerRect;

        // Serialized properties
        private SerializedProperty m_referencesHeaderGroup;
        private SerializedProperty m_scatteringHeaderGroup;
        private SerializedProperty m_outerSpaceHeaderGroup;
        private SerializedProperty m_cloudsHeaderGroup;
        private SerializedProperty m_fogScatteringHeaderGroup;
        private SerializedProperty m_optionsHeaderGroup;
        private SerializedProperty m_sunTransform;
        private SerializedProperty m_moonTransform;
        private SerializedProperty m_skyMaterial;
        private SerializedProperty m_fogMaterial;
        private SerializedProperty m_emptySkyShader;
        private SerializedProperty m_staticCloudsShader;
        private SerializedProperty m_dynamicCloudsShader;
        private SerializedProperty m_sunTexture;
        private SerializedProperty m_moonTexture;
        private SerializedProperty m_starfieldTexture;
        private SerializedProperty m_dynamicCloudsTexture;
        private SerializedProperty m_staticCloudTexture;
        private SerializedProperty m_molecularDensity;
        private SerializedProperty m_wavelengthR;
        private SerializedProperty m_wavelengthG;
        private SerializedProperty m_wavelengthB;
        private SerializedProperty m_kr;
        private SerializedProperty m_km;
        private SerializedProperty m_rayleigh;
        private SerializedProperty m_mie;
        private SerializedProperty m_mieDistance;
        private SerializedProperty m_scattering;
        private SerializedProperty m_luminance;
        private SerializedProperty m_exposure;
        private SerializedProperty m_rayleighColor;
        private SerializedProperty m_mieColor;
        private SerializedProperty m_scatteringColor;
        private SerializedProperty m_sunTextureSize;
        private SerializedProperty m_sunTextureIntensity;
        private SerializedProperty m_sunTextureColor;
        private SerializedProperty m_moonTextureSize;
        private SerializedProperty m_moonTextureIntensity;
        private SerializedProperty m_moonTextureColor;
        private SerializedProperty m_starsIntensity;
        private SerializedProperty m_milkyWayIntensity;
        private SerializedProperty m_starfieldColor;
        private SerializedProperty m_starfieldRotationX;
        private SerializedProperty m_starfieldRotationY;
        private SerializedProperty m_starfieldRotationZ;
        private SerializedProperty m_fogScatteringScale;
        private SerializedProperty m_globalFogDistance;
        private SerializedProperty m_globalFogSmoothStep;
        private SerializedProperty m_globalFogDensity;
        private SerializedProperty m_heightFogDistance;
        private SerializedProperty m_heightFogSmoothStep;
        private SerializedProperty m_heightFogDensity;
        private SerializedProperty m_heightFogStart;
        private SerializedProperty m_heightFogEnd;
        private SerializedProperty m_dynamicCloudsAltitude;
        private SerializedProperty m_dynamicCloudsDirection;
        private SerializedProperty m_dynamicCloudsSpeed;
        private SerializedProperty m_dynamicCloudsDensity;
        private SerializedProperty m_dynamicCloudsColor1;
        private SerializedProperty m_dynamicCloudsColor2;
        private SerializedProperty staticCloudLayer1Speed;
        private SerializedProperty staticCloudLayer2Speed;
        private SerializedProperty staticCloudScattering;
        private SerializedProperty staticCloudExtinction;
        private SerializedProperty staticCloudSaturation;
        private SerializedProperty staticCloudOpacity;
        private SerializedProperty staticCloudColor;
        private SerializedProperty m_shaderUpdateMode;
        private SerializedProperty m_scatteringMode;
        private SerializedProperty m_cloudMode;

        private void OnEnable()
        {
            // Get target
            m_target = (AzureSkyRenderController)target;
            m_target.UpdateSkySettings();

            // Find the serialized properties
            m_referencesHeaderGroup = serializedObject.FindProperty("m_referencesHeaderGroup");
            m_scatteringHeaderGroup = serializedObject.FindProperty("m_scatteringHeaderGroup");
            m_outerSpaceHeaderGroup = serializedObject.FindProperty("m_outerSpaceHeaderGroup");
            m_cloudsHeaderGroup = serializedObject.FindProperty("m_cloudsHeaderGroup");
            m_fogScatteringHeaderGroup = serializedObject.FindProperty("m_fogScatteringHeaderGroup");
            m_optionsHeaderGroup = serializedObject.FindProperty("m_optionsHeaderGroup");
            m_sunTransform = serializedObject.FindProperty("m_sunTransform");
            m_moonTransform = serializedObject.FindProperty("m_moonTransform");
            m_skyMaterial = serializedObject.FindProperty("m_skyMaterial");
            m_fogMaterial = serializedObject.FindProperty("m_fogMaterial");
            m_emptySkyShader = serializedObject.FindProperty("m_emptySkyShader");
            m_staticCloudsShader = serializedObject.FindProperty("m_staticCloudsShader");
            m_dynamicCloudsShader = serializedObject.FindProperty("m_dynamicCloudsShader");
            m_staticCloudTexture = serializedObject.FindProperty("staticCloudTexture");
            m_sunTexture = serializedObject.FindProperty("m_sunTexture");
            m_moonTexture = serializedObject.FindProperty("m_moonTexture");
            m_starfieldTexture = serializedObject.FindProperty("m_starfieldTexture");
            m_dynamicCloudsTexture = serializedObject.FindProperty("m_dynamicCloudsTexture");
            m_molecularDensity = serializedObject.FindProperty("molecularDensity");
            m_wavelengthR = serializedObject.FindProperty("wavelengthR");
            m_wavelengthG = serializedObject.FindProperty("wavelengthG");
            m_wavelengthB = serializedObject.FindProperty("wavelengthB");
            m_kr = serializedObject.FindProperty("kr");
            m_km = serializedObject.FindProperty("km");
            m_rayleigh = serializedObject.FindProperty("rayleigh");
            m_mie = serializedObject.FindProperty("mie");
            m_mieDistance = serializedObject.FindProperty("mieDistance");
            m_scattering = serializedObject.FindProperty("scattering");
            m_luminance = serializedObject.FindProperty("luminance");
            m_exposure = serializedObject.FindProperty("exposure");
            m_rayleighColor = serializedObject.FindProperty("rayleighColor");
            m_mieColor = serializedObject.FindProperty("mieColor");
            m_scatteringColor = serializedObject.FindProperty("scatteringColor");
            m_sunTextureSize = serializedObject.FindProperty("sunTextureSize");
            m_sunTextureIntensity = serializedObject.FindProperty("sunTextureIntensity");
            m_sunTextureColor = serializedObject.FindProperty("sunTextureColor");
            m_moonTextureSize = serializedObject.FindProperty("moonTextureSize");
            m_moonTextureIntensity = serializedObject.FindProperty("moonTextureIntensity");
            m_moonTextureColor = serializedObject.FindProperty("moonTextureColor");
            m_starsIntensity = serializedObject.FindProperty("starsIntensity");
            m_milkyWayIntensity = serializedObject.FindProperty("milkyWayIntensity");
            m_starfieldColor = serializedObject.FindProperty("starfieldColor");
            m_starfieldRotationX = serializedObject.FindProperty("starfieldRotationX");
            m_starfieldRotationY = serializedObject.FindProperty("starfieldRotationY");
            m_starfieldRotationZ = serializedObject.FindProperty("starfieldRotationZ");
            m_fogScatteringScale = serializedObject.FindProperty("fogScatteringScale");
            m_globalFogDistance = serializedObject.FindProperty("globalFogDistance");
            m_globalFogSmoothStep = serializedObject.FindProperty("globalFogSmoothStep");
            m_globalFogDensity = serializedObject.FindProperty("globalFogDensity");
            m_heightFogDistance = serializedObject.FindProperty("heightFogDistance");
            m_heightFogSmoothStep = serializedObject.FindProperty("heightFogSmoothStep");
            m_heightFogDensity = serializedObject.FindProperty("heightFogDensity");
            m_heightFogStart = serializedObject.FindProperty("heightFogStart");
            m_heightFogEnd = serializedObject.FindProperty("heightFogEnd");
            m_dynamicCloudsAltitude = serializedObject.FindProperty("dynamicCloudsAltitude");
            m_dynamicCloudsDirection = serializedObject.FindProperty("dynamicCloudsDirection");
            m_dynamicCloudsSpeed = serializedObject.FindProperty("dynamicCloudsSpeed");
            m_dynamicCloudsDensity = serializedObject.FindProperty("dynamicCloudsDensity");
            m_dynamicCloudsColor1 = serializedObject.FindProperty("dynamicCloudsColor1");
            m_dynamicCloudsColor2 = serializedObject.FindProperty("dynamicCloudsColor2");
            staticCloudLayer1Speed = serializedObject.FindProperty("staticCloudLayer1Speed");
            staticCloudLayer2Speed = serializedObject.FindProperty("staticCloudLayer2Speed");
            staticCloudScattering = serializedObject.FindProperty("staticCloudScattering");
            staticCloudExtinction = serializedObject.FindProperty("staticCloudExtinction");
            staticCloudSaturation = serializedObject.FindProperty("staticCloudSaturation");
            staticCloudOpacity = serializedObject.FindProperty("staticCloudOpacity");
            staticCloudColor = serializedObject.FindProperty("staticCloudColor");
            m_shaderUpdateMode = serializedObject.FindProperty("m_shaderUpdateMode");
            m_scatteringMode = serializedObject.FindProperty("m_scatteringMode");
            m_cloudMode = serializedObject.FindProperty("m_cloudMode");
        }

        public override void OnInspectorGUI()
        {
            // Start custom inspector
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();


            // References header group
            m_controlRect = EditorGUILayout.GetControlRect();
            m_headerRect = new Rect(m_controlRect.x + 15, m_controlRect.y, m_controlRect.width - 20, EditorGUIUtility.singleLineHeight);
            m_referencesHeaderGroup.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(m_headerRect, m_referencesHeaderGroup.isExpanded, GUIContent.none, "RL Header");
            EditorGUI.Foldout(m_headerRect, m_referencesHeaderGroup.isExpanded, GUIContent.none);
            EditorGUI.LabelField(m_headerRect, new GUIContent(" References", ""));
            if (m_referencesHeaderGroup.isExpanded)
            {
                EditorGUILayout.Space(2);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_sunTransform);
                EditorGUILayout.PropertyField(m_moonTransform);
                EditorGUILayout.PropertyField(m_skyMaterial);
                EditorGUILayout.PropertyField(m_fogMaterial);
                EditorGUILayout.PropertyField(m_emptySkyShader);
                EditorGUILayout.PropertyField(m_staticCloudsShader);
                EditorGUILayout.PropertyField(m_dynamicCloudsShader);
                EditorGUILayout.PropertyField(m_sunTexture);
                EditorGUILayout.PropertyField(m_moonTexture);
                EditorGUILayout.PropertyField(m_starfieldTexture);
                EditorGUILayout.PropertyField(m_dynamicCloudsTexture);
                EditorGUILayout.PropertyField(m_staticCloudTexture);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.Space(2);

            // Scattering header group
            m_controlRect = EditorGUILayout.GetControlRect();
            m_headerRect = new Rect(m_controlRect.x + 15, m_controlRect.y, m_controlRect.width - 20, EditorGUIUtility.singleLineHeight);
            m_scatteringHeaderGroup.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(m_headerRect, m_scatteringHeaderGroup.isExpanded, GUIContent.none, "RL Header");
            EditorGUI.Foldout(m_headerRect, m_scatteringHeaderGroup.isExpanded, GUIContent.none);
            EditorGUI.LabelField(m_headerRect, new GUIContent(" Scattering", ""));
            if (m_scatteringHeaderGroup.isExpanded)
            {
                EditorGUILayout.Space(2);
                EditorGUI.indentLevel++;
                EditorGUILayout.Slider(m_molecularDensity, 0.1f, 3.0f);
                EditorGUILayout.Slider(m_wavelengthR, 380f, 740f);
                EditorGUILayout.Slider(m_wavelengthG, 380f, 740f);
                EditorGUILayout.Slider(m_wavelengthB, 380f, 740f);
                EditorGUILayout.Slider(m_kr, 0f, 20f);
                EditorGUILayout.Slider(m_km, 0f, 10f);
                EditorGUILayout.Slider(m_rayleigh, 0f, 5f);
                EditorGUILayout.Slider(m_mie, 0f, 5f);
                EditorGUILayout.Slider(m_mieDistance, 0f, 1f);
                EditorGUILayout.Slider(m_scattering, 0f, 1f);
                EditorGUILayout.Slider(m_luminance, 0f, 5f);
                EditorGUILayout.Slider(m_exposure, 0f, 10f);
                EditorGUILayout.PropertyField(m_rayleighColor);
                EditorGUILayout.PropertyField(m_mieColor);
                EditorGUILayout.PropertyField(m_scatteringColor);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.Space(2);

            // Outer space header group
            m_controlRect = EditorGUILayout.GetControlRect();
            m_headerRect = new Rect(m_controlRect.x + 15, m_controlRect.y, m_controlRect.width - 20, EditorGUIUtility.singleLineHeight);
            m_outerSpaceHeaderGroup.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(m_headerRect, m_outerSpaceHeaderGroup.isExpanded, GUIContent.none, "RL Header");
            EditorGUI.Foldout(m_headerRect, m_outerSpaceHeaderGroup.isExpanded, GUIContent.none);
            EditorGUI.LabelField(m_headerRect, new GUIContent(" Outer Space", ""));
            if (m_outerSpaceHeaderGroup.isExpanded)
            {
                EditorGUILayout.Space(2);
                EditorGUI.indentLevel++;
                EditorGUILayout.Slider(m_sunTextureSize, 0.5f, 10f);
                EditorGUILayout.Slider(m_sunTextureIntensity, 0f, 2f);
                EditorGUILayout.PropertyField(m_sunTextureColor);
                EditorGUILayout.Slider(m_moonTextureSize, 1f, 20f);
                EditorGUILayout.Slider(m_moonTextureIntensity, 0f, 2f);
                EditorGUILayout.PropertyField(m_moonTextureColor);
                EditorGUILayout.Slider(m_starsIntensity, 0f, 1f);
                EditorGUILayout.Slider(m_milkyWayIntensity, 0f, 1f);
                EditorGUILayout.PropertyField(m_starfieldColor);
                EditorGUILayout.Slider(m_starfieldRotationX, -180f, 180f);
                EditorGUILayout.Slider(m_starfieldRotationY, -180f, 180f);
                EditorGUILayout.Slider(m_starfieldRotationZ, -180f, 180f);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.Space(2);

            // Fog Scattering header group
            m_controlRect = EditorGUILayout.GetControlRect();
            m_headerRect = new Rect(m_controlRect.x + 15, m_controlRect.y, m_controlRect.width - 20, EditorGUIUtility.singleLineHeight);
            m_fogScatteringHeaderGroup.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(m_headerRect, m_fogScatteringHeaderGroup.isExpanded, GUIContent.none, "RL Header");
            EditorGUI.Foldout(m_headerRect, m_fogScatteringHeaderGroup.isExpanded, GUIContent.none);
            EditorGUI.LabelField(m_headerRect, new GUIContent(" Fog Scattering", ""));
            if (m_fogScatteringHeaderGroup.isExpanded)
            {
                EditorGUILayout.Space(2);
                EditorGUI.indentLevel++;
                EditorGUILayout.Slider(m_fogScatteringScale, 0f, 1f);
                EditorGUILayout.Slider(m_globalFogDistance, 0f, 25000f);
                EditorGUILayout.Slider(m_globalFogSmoothStep, -1f, 2f);
                EditorGUILayout.Slider(m_globalFogDensity, 0f, 1f);
                EditorGUILayout.Slider(m_heightFogDistance, 0f, 1500f);
                EditorGUILayout.Slider(m_heightFogSmoothStep, -1f, 2f);
                EditorGUILayout.Slider(m_heightFogDensity, 0f, 1f);
                EditorGUILayout.Slider(m_heightFogStart, -500f, 500f);
                EditorGUILayout.Slider(m_heightFogEnd, 0f, 2500f);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.Space(2);

            // Clouds header group
            m_controlRect = EditorGUILayout.GetControlRect();
            m_headerRect = new Rect(m_controlRect.x + 15, m_controlRect.y, m_controlRect.width - 20, EditorGUIUtility.singleLineHeight);
            m_cloudsHeaderGroup.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(m_headerRect, m_cloudsHeaderGroup.isExpanded, GUIContent.none, "RL Header");
            EditorGUI.Foldout(m_headerRect, m_cloudsHeaderGroup.isExpanded, GUIContent.none);
            EditorGUI.LabelField(m_headerRect, new GUIContent(" Clouds", ""));
            if (m_cloudsHeaderGroup.isExpanded)
            {
                EditorGUILayout.Space(2);
                EditorGUI.indentLevel++;
                EditorGUILayout.Slider(m_dynamicCloudsAltitude, 1f, 20f);
                EditorGUILayout.Slider(m_dynamicCloudsDirection, 0f, 1f);
                EditorGUILayout.Slider(m_dynamicCloudsSpeed, 0f, 1f);
                EditorGUILayout.Slider(m_dynamicCloudsDensity, 0f, 1f);
                EditorGUILayout.PropertyField(m_dynamicCloudsColor1);
                EditorGUILayout.PropertyField(m_dynamicCloudsColor2);
                EditorGUILayout.Slider(staticCloudLayer1Speed, 0f, 0.01f);
                EditorGUILayout.Slider(staticCloudLayer2Speed, 0f, 0.01f);
                EditorGUILayout.Slider(staticCloudScattering, 0f, 2f);
                EditorGUILayout.Slider(staticCloudExtinction, 1f, 5f);
                EditorGUILayout.Slider(staticCloudSaturation, 1f, 8f);
                EditorGUILayout.Slider(staticCloudOpacity, 0f, 2f);
                EditorGUILayout.PropertyField(staticCloudColor);


                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.Space(2);

            // Options header group
            m_controlRect = EditorGUILayout.GetControlRect();
            m_headerRect = new Rect(m_controlRect.x + 15, m_controlRect.y, m_controlRect.width - 20, EditorGUIUtility.singleLineHeight);
            m_optionsHeaderGroup.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(m_headerRect, m_optionsHeaderGroup.isExpanded, GUIContent.none, "RL Header");
            EditorGUI.Foldout(m_headerRect, m_optionsHeaderGroup.isExpanded, GUIContent.none);
            EditorGUI.LabelField(m_headerRect, new GUIContent(" Options", ""));
            if (m_optionsHeaderGroup.isExpanded)
            {
                EditorGUILayout.Space(2);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_shaderUpdateMode);
                EditorGUILayout.PropertyField(m_scatteringMode);
                EditorGUILayout.PropertyField(m_cloudMode);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.Space(2);


            // Update the inspector when there is a change
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                m_target.UpdateSkySettings();
            }
        }
    }
}