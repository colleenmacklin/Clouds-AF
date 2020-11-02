using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AzureSky;

namespace UnityEditor.AzureSky
{
    [CustomEditor(typeof(AzureSkyProfile))]
    public class AzureSkyProfileInspector : Editor
    {
        // Editor only
        private AzureSkyProfile m_target;
        public Texture2D logoTexture;
        private Rect m_controlRect;
        private readonly Color m_greenColor = new Color(0.85f, 1.0f, 0.85f);
        private readonly Color m_redColor = new Color(1.0f, 0.75f, 0.75f);
        
        // Tab folds
        private bool m_showScatteringGroup;
        private bool m_showOuterSpaceGroup;
        private bool m_showFogScatteringGroup;
        private bool m_showCloudsGroup;
        private bool m_showLightingGroup;
        private bool m_showWeatherGroup;
        private bool m_showOutputsGroup;
        
        // GUIContents
        private readonly GUIContent[] m_guiContent = new[]
        {
            new GUIContent("Slider", "Use this to setting the same value for each time of day."),
            new GUIContent("Timeline Curve", "Use this to setting different values for each time of day based on the timeline."),
            new GUIContent("Sun Curve", "Use this to setting different values for each time of day based on the sun elevation in the sky."),
            new GUIContent("Moon Curve", "Use this to setting different values for each time of day based on the moon elevation in the sky."),
            new GUIContent("Property Type", "Sets the way how the property should behave."),
            new GUIContent("Molecular Density", "The molecular density of the air."),
            new GUIContent("Wavelength R", "The red wavelength from the visible spectrum."),
            new GUIContent("Wavelength G", "The green wavelength from the visible spectrum."),
            new GUIContent("Wavelength B", "The blue wavelength from the visible spectrum."),
            new GUIContent("Rayleigh", "The rayleigh multiplier coefficient."),
            new GUIContent("Mie", "The mie multiplier coefficient."),
            new GUIContent("Scattering", "The light scattering multiplier coefficient."),
            new GUIContent("Luminance", "The luminance of the sky, useful to set the intensity of the night when there is no moon in the sky."),
            new GUIContent("Exposure", "The color exposure of the internal tonemapping effect."),
            new GUIContent("Color", "Use this to setting the same color for each time of day."),
            new GUIContent("Timeline Gradient", "Use this to setting different colors for each time of day based on the timeline."),
            new GUIContent("Sun Gradient", "Use this to setting different colors for each time of day based on the sun elevation in the sky."),
            new GUIContent("Moon Gradient", "Use this to setting different colors for each time of day based on the moon elevation in the sky."),
            new GUIContent("Rayleigh Color", "Rayleigh color multiplier."),
            new GUIContent("Mie Color", "Mie color multiplier."),
            new GUIContent("Scattering Color", "Custom scattering color, it will only works if the 'Scattering Mode' is set to 'Custom Color' in the sky controller options."),
            new GUIContent("Sun Texture Size", "The size of the sun texture."),
            new GUIContent("Sun Texture Intensity", "The intensity of the sun texture."),
            new GUIContent("Sun Texture Color", "The color of the sun texture."),
            new GUIContent("Moon Texture Size", "The size of the moon texture."),
            new GUIContent("Moon Texture Intensity", "The intensity of the moon texture."),
            new GUIContent("Moon Texture Color", "The color of the moon texture."),
            new GUIContent("Stars Scintillation", "The scintillation speed of the regular stars."),
            new GUIContent("Stars Intensity", "The intensity of the regular stars."),
            new GUIContent("Milky Way Intensity", "The intensity of the Milky Way."),
            new GUIContent("Scale", "The fog scattering scale. Use this to make the fog more bright or more blue."),
            new GUIContent("Global Fog Distance", "The distance of the global fog in real world scale(meters). This is the distance at which the scene will be completely covered by the global fog."),
            new GUIContent("Global Fog Smooth", "Smooths the fog transition between the distance where the global fog starts and where it is completely covered by the global fog."),
            new GUIContent("Global Fog Density", "The density of the global fog. Use this to make the entire fog disappear at the same time regardless of distance."),
            new GUIContent("Height Fog Distance", "The distance of the height fog in real world scale(meters). This is the distance at which the scene will be completely covered by the height fog."),
            new GUIContent("Height Fog Smooth", "Smooths the fog transition between the distance where the height fog starts and where it is completely covered by the height fog."),
            new GUIContent("Height Fog Density", "The density of the height fog. Use this to make the entire fog disappear at the same time regardless of distance."),
            new GUIContent("Height Fog Start", "The height where the height fog will start"),
            new GUIContent("Height Fog End", "The height where the height fog will end"),
            new GUIContent("Static Cloud Texture", "The texture used to render the static cloud layer."),
            new GUIContent("Static Cloud Layer1 Speed", "The rotation speed of the first layer from static cloud render."),
            new GUIContent("Static Cloud Layer2 Speed", "The rotation speed of the second layer from static cloud render."),
            new GUIContent("Static Cloud Scattering", "The amount of light scattered by the static clouds."),
            new GUIContent("Static Cloud Extinction", "The fading of distant clouds on the horizon."),
            new GUIContent("Static Cloud Saturation", "Color saturation of static clouds."),
            new GUIContent("Static Cloud Opacity", "The opacity of static clouds."),
            new GUIContent("Static Cloud Color", "The color of the static clouds."),
            new GUIContent("Dynamic Cloud Altitude", "The fake altitude of dynamic clouds."),
            new GUIContent("Dynamic Cloud Direction", "The movement direction of dynamic clouds."),
            new GUIContent("Dynamic Cloud Speed", "The movement speed of dynamic clouds."),
            new GUIContent("Dynamic Cloud Density", "The density of dynamic clouds used to simulate the coverage."),
            new GUIContent("Dynamic Cloud Color1", "The first color of the dynamic clouds."),
            new GUIContent("Dynamic Cloud Color2", "The second color of the dynamic clouds."),
            new GUIContent("Directional Light Intensity", "The intensity of the directional light."),
            new GUIContent("Directional Light Color", "The color of the directional light."),
            new GUIContent("Environment Intensity", "The intensity multiplier of the environment lighting."),
            new GUIContent("Environment Ambient Color", "The ambient color of the environment lighting."),
            new GUIContent("Environment Equator Color", "The equator color of the environment lighting."),
            new GUIContent("Environment Ground Color", "The ground color of the environment lighting."),
            new GUIContent("Reflection Probe Intensity", "The intensity of the sky reflection probe."),
            new GUIContent("Light Rain Intensity", "The intensity of light rain particle."),
            new GUIContent("Medium Rain Intensity", "The intensity of medium rain particle."),
            new GUIContent("Heavy Rain Intensity", "The intensity of heavy rain particle."),
            new GUIContent("Snow Intensity", "The intensity of snow particle."),
            new GUIContent("Rain Color", "The color of the rain particles effect."),
            new GUIContent("Snow Color", "The color of the rain particles effect."),
            new GUIContent("Light Rain Sound Volume", "The volume of the light rain sound effect."),
            new GUIContent("Medium Rain Sound Volume", "The volume of the medium rain sound effect."),
            new GUIContent("Heavy Rain Sound Volume", "The volume of the heavy rain sound effect."),
            new GUIContent("Light Wind Sound Volume", "The volume of the light wind sound effect."),
            new GUIContent("Medium Wind Sound Volume", "The volume of the medium wind sound effect."),
            new GUIContent("Heavy Wind Sound Volume", "The volume of the heavy wind sound effect."),
            new GUIContent("Wind Speed", "The wind speed."),
            new GUIContent("Wind Direction", "The wind direction."),
            new GUIContent("Output Profile", "Place here the output profile that stores the extra properties you want to include in this day profile. Note that the sky controller must be using this same output profile.")
        };
        
        // Serialized properties
        private SerializedProperty m_molecularDensity;
        private SerializedProperty m_wavelengthR;
        private SerializedProperty m_wavelengthG;
        private SerializedProperty m_wavelengthB;
        private SerializedProperty m_rayleigh;
        private SerializedProperty m_mie;
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
        private SerializedProperty m_fogScatteringScale;
        private SerializedProperty m_globalFogDistance;
        private SerializedProperty m_globalFogSmooth;
        private SerializedProperty m_globalFogDensity;
        private SerializedProperty m_heightFogDistance;
        private SerializedProperty m_heightFogSmooth;
        private SerializedProperty m_heightFogDensity;
        private SerializedProperty m_heightFogStart;
        private SerializedProperty m_heightFogEnd;
        private SerializedProperty m_staticCloudTexture;
        private SerializedProperty m_staticCloudLayer1Speed;
        private SerializedProperty m_staticCloudLayer2Speed;
        private SerializedProperty m_staticCloudScattering;
        private SerializedProperty m_staticCloudExtinction;
        private SerializedProperty m_staticCloudSaturation;
        private SerializedProperty m_staticCloudOpacity;
        private SerializedProperty m_staticCloudColor;
        private SerializedProperty m_dynamicCloudAltitude;
        private SerializedProperty m_dynamicCloudDirection;
        private SerializedProperty m_dynamicCloudSpeed;
        private SerializedProperty m_dynamicCloudDensity;
        private SerializedProperty m_dynamicCloudColor1;
        private SerializedProperty m_dynamicCloudColor2;
        private SerializedProperty m_directionalLightIntensity;
        private SerializedProperty m_directionalLightColor;
        private SerializedProperty m_environmentIntensity;
        private SerializedProperty m_environmentAmbientColor;
        private SerializedProperty m_environmentEquatorColor;
        private SerializedProperty m_environmentGroundColor;
        private SerializedProperty m_lightRainIntensity;
        private SerializedProperty m_mediumRainIntensity;
        private SerializedProperty m_heavyRainIntensity;
        private SerializedProperty m_snowIntensity;
        private SerializedProperty m_rainColor;
        private SerializedProperty m_snowColor;
        private SerializedProperty m_lightRainSoundVolume;
        private SerializedProperty m_mediumRainSoundVolume;
        private SerializedProperty m_heavyRainSoundVolume;
        private SerializedProperty m_lightWindSoundVolume;
        private SerializedProperty m_mediumWindSoundVolume;
        private SerializedProperty m_heavyWindSoundVolume;
        private SerializedProperty m_windSpeed;
        private SerializedProperty m_windDirection;
        private SerializedProperty m_outputProfile;
        
        private SerializedProperty m_outputPropertyList;
        private ReorderableList m_reorderableOutputPropertyList;
        
        private void OnEnable()
        {
            // Get target
            m_target = (AzureSkyProfile) target;
            
            // Find the serialized properties
            m_molecularDensity = serializedObject.FindProperty("molecularDensity");
            m_wavelengthR = serializedObject.FindProperty("wavelengthR");
            m_wavelengthG = serializedObject.FindProperty("wavelengthG");
            m_wavelengthB = serializedObject.FindProperty("wavelengthB");
            m_rayleigh = serializedObject.FindProperty("rayleigh");
            m_mie = serializedObject.FindProperty("mie");
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
            m_fogScatteringScale = serializedObject.FindProperty("fogScatteringScale");
            m_globalFogDistance = serializedObject.FindProperty("globalFogDistance");
            m_globalFogSmooth = serializedObject.FindProperty("globalFogSmooth");
            m_globalFogDensity = serializedObject.FindProperty("globalFogDensity");
            m_heightFogDistance = serializedObject.FindProperty("heightFogDistance");
            m_heightFogSmooth = serializedObject.FindProperty("heightFogSmooth");
            m_heightFogDensity = serializedObject.FindProperty("heightFogDensity");
            m_heightFogStart = serializedObject.FindProperty("heightFogStart");
            m_heightFogEnd = serializedObject.FindProperty("heightFogEnd");
            m_staticCloudTexture = serializedObject.FindProperty("staticCloudTexture");
            m_staticCloudLayer1Speed = serializedObject.FindProperty("staticCloudLayer1Speed");
            m_staticCloudLayer2Speed = serializedObject.FindProperty("staticCloudLayer2Speed");
            m_staticCloudScattering = serializedObject.FindProperty("staticCloudScattering");
            m_staticCloudExtinction = serializedObject.FindProperty("staticCloudExtinction");
            m_staticCloudSaturation = serializedObject.FindProperty("staticCloudSaturation");
            m_staticCloudOpacity = serializedObject.FindProperty("staticCloudOpacity");
            m_staticCloudColor = serializedObject.FindProperty("staticCloudColor");
            m_dynamicCloudAltitude = serializedObject.FindProperty("dynamicCloudAltitude");
            m_dynamicCloudDirection = serializedObject.FindProperty("dynamicCloudDirection");
            m_dynamicCloudSpeed = serializedObject.FindProperty("dynamicCloudSpeed");
            m_dynamicCloudDensity = serializedObject.FindProperty("dynamicCloudDensity");
            m_dynamicCloudColor1 = serializedObject.FindProperty("dynamicCloudColor1");
            m_dynamicCloudColor2 = serializedObject.FindProperty("dynamicCloudColor2");
            m_directionalLightIntensity = serializedObject.FindProperty("directionalLightIntensity");
            m_directionalLightColor = serializedObject.FindProperty("directionalLightColor");
            m_environmentIntensity = serializedObject.FindProperty("environmentIntensity");
            m_environmentAmbientColor = serializedObject.FindProperty("environmentAmbientColor");
            m_environmentEquatorColor = serializedObject.FindProperty("environmentEquatorColor");
            m_environmentGroundColor = serializedObject.FindProperty("environmentGroundColor");
            m_lightRainIntensity = serializedObject.FindProperty("lightRainIntensity");
            m_mediumRainIntensity = serializedObject.FindProperty("mediumRainIntensity");
            m_heavyRainIntensity = serializedObject.FindProperty("heavyRainIntensity");
            m_snowIntensity = serializedObject.FindProperty("snowIntensity");
            m_rainColor = serializedObject.FindProperty("rainColor");
            m_snowColor = serializedObject.FindProperty("snowColor");
            m_lightRainSoundVolume = serializedObject.FindProperty("lightRainSoundVolume");
            m_mediumRainSoundVolume = serializedObject.FindProperty("mediumRainSoundVolume");
            m_heavyRainSoundVolume = serializedObject.FindProperty("heavyRainSoundVolume");
            m_lightWindSoundVolume = serializedObject.FindProperty("lightWindSoundVolume");
            m_mediumWindSoundVolume = serializedObject.FindProperty("mediumWindSoundVolume");
            m_heavyWindSoundVolume = serializedObject.FindProperty("heavyWindSoundVolume");
            m_windSpeed = serializedObject.FindProperty("windSpeed");
            m_windDirection = serializedObject.FindProperty("windDirection");
            m_outputProfile = serializedObject.FindProperty("outputProfile");
            
            // Create the output property list
            m_outputPropertyList = serializedObject.FindProperty("outputPropertyList");
            m_reorderableOutputPropertyList = new ReorderableList(serializedObject, m_outputPropertyList, false, false, false, false)
            {
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    rect.y += 2;
                    float height = EditorGUIUtility.singleLineHeight;
                    Rect fieldRect = new Rect(rect.x - 15, rect.y, rect.width + 15, height);
                    var element = m_reorderableOutputPropertyList.serializedProperty.GetArrayElementAtIndex(index);
                    var type = element.FindPropertyRelative("type");
                    type.enumValueIndex = (int) m_target.outputProfile.outputList[index].type;
                    
                    // Output index
                    EditorGUI.LabelField(fieldRect, "Output " + index.ToString());
                    
                    // Draw property
                    fieldRect = new Rect(rect.x + 50, rect.y, rect.width - 50, height);
                    switch (type.enumValueIndex)
                    {
                        // Slider
                        case 0:
                            EditorGUI.Slider(fieldRect, element.FindPropertyRelative("slider"), 0.0f, 1.0f, GUIContent.none);
                            break;
                        
                        // Timeline curve
                        case 1:
                            EditorGUI.CurveField(fieldRect, element.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0, 0.0f, 24.0f, 1.0f), GUIContent.none);
                            break;
                        
                        // Sun curve
                        case 2:
                            EditorGUI.CurveField(fieldRect, element.FindPropertyRelative("sunCurve"), Color.yellow, new Rect(-1.0f, 0.0f, 2.0f, 1.0f), GUIContent.none);
                            break;
                        
                        // Moon curve
                        case 3:
                            EditorGUI.CurveField(fieldRect, element.FindPropertyRelative("moonCurve"), Color.cyan, new Rect(-1.0f, 0.0f, 2.0f, 1.0f), GUIContent.none);
                            break;
                        
                        // Color
                        case 4:
                            EditorGUI.PropertyField(fieldRect, element.FindPropertyRelative("color"), GUIContent.none);
                            break;
                        
                        // Timeline gradient
                        case 5:
                            EditorGUI.PropertyField(fieldRect, element.FindPropertyRelative("timelineGradient"), GUIContent.none);
                            break;
                        
                        // Sun gradient
                        case 6:
                            EditorGUI.PropertyField(fieldRect, element.FindPropertyRelative("sunGradient"), GUIContent.none);
                            break;
                        
                        // Moon gradient
                        case 7:
                            EditorGUI.PropertyField(fieldRect, element.FindPropertyRelative("moonGradient"), GUIContent.none);
                            break;
                    }
                    
                    // Description
                    fieldRect = new Rect(rect.x, rect.y + height + 2, rect.width, height * 3.0f);
                    string description = "Property Type: " + m_target.outputProfile.outputList[index].type + "\nDescription: " + m_target.outputProfile.outputList[index].description;
                    EditorGUI.HelpBox(fieldRect, description, MessageType.Info);
                },
                
                elementHeightCallback = (int index) => EditorGUIUtility.singleLineHeight * 4.5f,
                
                drawElementBackgroundCallback = (rect, index, active, focused) =>
                {
                    
                }
            };
        }

        public override void OnInspectorGUI()
        {
            // Logo
            m_controlRect = EditorGUILayout.GetControlRect();
            EditorGUI.LabelField(new Rect(m_controlRect.x - 3, m_controlRect.y, m_controlRect.width + 3, 65), "", "", "selectionRect");
            if (logoTexture)
                GUI.DrawTexture(new Rect(m_controlRect.x, m_controlRect.y, 261, 56), logoTexture);
            GUILayout.Space(50);
            
            // Start custom Inspector
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            
            // Scattering tab
            m_showScatteringGroup = EditorGUILayout.BeginFoldoutHeaderGroup(m_target.showScatteringGroup, "Scattering");
            if (m_showScatteringGroup)
            {
                EditorGUI.indentLevel++;
                DrawFloatProperty(m_molecularDensity, 0.1f, 3.0f, 2.545f, m_guiContent[5]);
                DrawFloatProperty(m_wavelengthR, 380.0f, 740.0f, 680.0f, m_guiContent[6]);
                DrawFloatProperty(m_wavelengthG, 380.0f, 740.0f, 550.0f, m_guiContent[7]);
                DrawFloatProperty(m_wavelengthB, 380.0f, 740.0f, 450.0f, m_guiContent[8]);
                DrawFloatProperty(m_rayleigh, 0.0f, 10.0f, 1.5f, m_guiContent[9]);
                DrawFloatProperty(m_mie, 0.0f, 10.0f, 1.0f, m_guiContent[10]);
                DrawFloatProperty(m_scattering, 0.0f, 1.0f, 0.25f, m_guiContent[11]);
                DrawFloatProperty(m_luminance, 0.0f, 5.0f, 1.5f, m_guiContent[12]);
                DrawFloatProperty(m_exposure, 0.0f, 10.0f, 2.0f, m_guiContent[13]);
                DrawColorProperty(m_rayleighColor, ref m_target.rayleighColor, m_guiContent[18]);
                DrawColorProperty(m_mieColor, ref m_target.mieColor, m_guiContent[19]);
                DrawColorProperty(m_scatteringColor, ref m_target.scatteringColor, m_guiContent[20]);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            
            // Outer Space tab
            m_showOuterSpaceGroup = EditorGUILayout.BeginFoldoutHeaderGroup(m_target.showOuterSpaceGroup, "Outer Space");
            if (m_showOuterSpaceGroup)
            {
                EditorGUI.indentLevel++;
                DrawFloatProperty(m_sunTextureSize, 0.5f, 10.0f, 2.5f, m_guiContent[21]);
                DrawFloatProperty(m_sunTextureIntensity, 0.0f, 2.0f, 1.0f, m_guiContent[22]);
                DrawColorProperty(m_sunTextureColor, ref m_target.sunTextureColor, m_guiContent[23]);
                DrawFloatProperty(m_moonTextureSize, 1.0f, 20.0f, 10.0f, m_guiContent[24]);
                DrawFloatProperty(m_moonTextureIntensity, 0.0f, 2.0f, 1.0f, m_guiContent[25]);
                DrawColorProperty(m_moonTextureColor, ref m_target.moonTextureColor, m_guiContent[26]);
                DrawFloatProperty(m_starsIntensity, 0.0f, 1.0f, 0.5f, m_guiContent[28]);
                DrawFloatProperty(m_milkyWayIntensity, 0.0f, 1.0f, 0.0f, m_guiContent[29]);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            
            // Fog Scattering tab
            m_showFogScatteringGroup = EditorGUILayout.BeginFoldoutHeaderGroup(m_target.showFogScatteringGroup, "Fog Scattering");
            if (m_showFogScatteringGroup)
            {
                EditorGUI.indentLevel++;
                DrawFloatProperty(m_fogScatteringScale, 0.0f, 1.0f, 1.0f, m_guiContent[30]);
                DrawFloatProperty(m_globalFogDistance, 0.0f, 25000.0f, 1000.0f, m_guiContent[31]);
                DrawFloatProperty(m_globalFogSmooth, -1.0f, 2.0f, 0.25f, m_guiContent[32]);
                DrawFloatProperty(m_globalFogDensity, 0.0f, 1.0f, 1.0f, m_guiContent[33]);
                DrawFloatProperty(m_heightFogDistance, 0.0f, 1500.0f, 100.0f, m_guiContent[34]);
                DrawFloatProperty(m_heightFogSmooth, -1.0f, 2.0f, 1.0f, m_guiContent[35]);
                DrawFloatProperty(m_heightFogDensity, 0.0f, 1.0f, 0.0f, m_guiContent[36]);
                DrawFloatProperty(m_heightFogStart, -500.0f, 500.0f, 0.0f, m_guiContent[37]);
                DrawFloatProperty(m_heightFogEnd, 0.0f, 2000.0f, 100.0f, m_guiContent[38]);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            
            // Clouds tab
            m_showCloudsGroup = EditorGUILayout.BeginFoldoutHeaderGroup(m_target.showCloudsGroup, "Clouds");
            if (m_showCloudsGroup)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_staticCloudTexture, m_guiContent[39]);
                DrawFloatProperty(m_staticCloudLayer1Speed, -0.01f, 0.01f, 0.0f, m_guiContent[40]);
                DrawFloatProperty(m_staticCloudLayer2Speed, -0.01f, 0.01f, 0.0f, m_guiContent[41]);
                DrawFloatProperty(m_staticCloudScattering, 0.0f, 2.0f, 1.0f, m_guiContent[42]);
                DrawFloatProperty(m_staticCloudExtinction, 1.0f, 5.0f, 1.5f, m_guiContent[43]);
                DrawFloatProperty(m_staticCloudSaturation, 1.0f, 8.0f, 2.5f, m_guiContent[44]);
                DrawFloatProperty(m_staticCloudOpacity, 0.0f, 2.0f, 1.25f, m_guiContent[45]);
                DrawColorProperty(m_staticCloudColor, ref m_target.staticCloudColor, m_guiContent[46]);
                DrawFloatProperty(m_dynamicCloudAltitude, 1.0f, 20.0f, 7.5f, m_guiContent[47]);
                DrawFloatProperty(m_dynamicCloudDirection, -180.0f, 180.0f, 0.0f, m_guiContent[48]);
                DrawFloatProperty(m_dynamicCloudSpeed, 0.0f, 1.0f, 0.1f, m_guiContent[49]);
                DrawFloatProperty(m_dynamicCloudDensity, 0.0f, 1.0f, 0.75f, m_guiContent[50]);
                DrawColorProperty(m_dynamicCloudColor1, ref m_target.dynamicCloudColor1, m_guiContent[51]);
                DrawColorProperty(m_dynamicCloudColor2, ref m_target.dynamicCloudColor2, m_guiContent[52]);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            
            // Lighting tab
            m_showLightingGroup = EditorGUILayout.BeginFoldoutHeaderGroup(m_target.showLightingGroup, "Lighting");
            if (m_showLightingGroup)
            {
                EditorGUI.indentLevel++;
                DrawFloatProperty(m_directionalLightIntensity, 0.0f, 8.0f, 1.0f, m_guiContent[53]);
                DrawColorProperty(m_directionalLightColor, ref m_target.directionalLightColor, m_guiContent[54]);
                DrawFloatProperty(m_environmentIntensity, 0.0f, 8.0f, 1.0f, m_guiContent[55]);
                DrawColorProperty(m_environmentAmbientColor, ref m_target.environmentAmbientColor, m_guiContent[56]);
                DrawColorProperty(m_environmentEquatorColor, ref m_target.environmentEquatorColor, m_guiContent[57]);
                DrawColorProperty(m_environmentGroundColor, ref m_target.environmentGroundColor, m_guiContent[58]);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            
            // Weather tab
            m_showWeatherGroup = EditorGUILayout.BeginFoldoutHeaderGroup(m_target.showWeatherGroup, "Weather");
            if (m_showWeatherGroup)
            {
                EditorGUI.indentLevel++;
                DrawFloatProperty(m_lightRainIntensity, 0.0f, 1.0f, 0.0f, m_guiContent[60]);
                DrawFloatProperty(m_mediumRainIntensity, 0.0f, 1.0f, 0.0f, m_guiContent[61]);
                DrawFloatProperty(m_heavyRainIntensity, 0.0f, 1.0f, 0.0f, m_guiContent[62]);
                DrawFloatProperty(m_snowIntensity, 0.0f, 1.0f, 0.0f, m_guiContent[63]);
                DrawColorProperty(m_rainColor, ref m_target.rainColor, m_guiContent[64]);
                DrawColorProperty(m_snowColor, ref m_target.snowColor, m_guiContent[65]);
                DrawFloatProperty(m_lightRainSoundVolume, 0.0f, 1.0f, 0.0f, m_guiContent[66]);
                DrawFloatProperty(m_mediumRainSoundVolume, 0.0f, 1.0f, 0.0f, m_guiContent[67]);
                DrawFloatProperty(m_heavyRainSoundVolume, 0.0f, 1.0f, 0.0f, m_guiContent[68]);
                DrawFloatProperty(m_lightWindSoundVolume, 0.0f, 1.0f, 0.0f, m_guiContent[69]);
                DrawFloatProperty(m_mediumWindSoundVolume, 0.0f, 1.0f, 0.0f, m_guiContent[70]);
                DrawFloatProperty(m_heavyWindSoundVolume, 0.0f, 1.0f, 0.0f, m_guiContent[71]);
                DrawFloatProperty(m_windSpeed, 0.0f, 1.0f, 0.0f, m_guiContent[72]);
                DrawFloatProperty(m_windDirection, -180.0f, 180.0f, 0.0f, m_guiContent[73]);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            
            // Outputs tab
            m_showOutputsGroup = EditorGUILayout.BeginFoldoutHeaderGroup(m_target.showOutputsGroup, "Outputs");
            if (m_showOutputsGroup)
            {
                EditorGUI.indentLevel++;
                GUI.color = m_greenColor;
                if (!m_target.outputProfile) GUI.color = m_redColor;
                EditorGUILayout.PropertyField(m_outputProfile, m_guiContent[74]);
                GUI.color = Color.white;

                if (m_target.outputProfile)
                {
                    m_reorderableOutputPropertyList.serializedProperty.arraySize = m_target.outputProfile.outputList.Count;
                    m_reorderableOutputPropertyList.DoLayoutList();
                }
                else
                {
                    EditorGUILayout.HelpBox("There is no output profile attached, please add an output profile if you want to extend this day profile with extra properties.", MessageType.Error);
                }
                
                
                GUI.color = Color.white;
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            // End custom Inspector
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(m_target, "Undo Azure Sky Profile");
                m_target.showScatteringGroup = m_showScatteringGroup;
                m_target.showOuterSpaceGroup = m_showOuterSpaceGroup;
                m_target.showFogScatteringGroup = m_showFogScatteringGroup;
                m_target.showCloudsGroup = m_showCloudsGroup;
                m_target.showLightingGroup = m_showLightingGroup;
                m_target.showWeatherGroup = m_showWeatherGroup;
                m_target.showOutputsGroup = m_showOutputsGroup;
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void DrawFloatProperty(SerializedProperty property, float minValue, float maxValue, float defaultValue, GUIContent content)
        {
            EditorGUILayout.BeginHorizontal();
            property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, content);
            if (GUILayout.Button(EditorGUIUtility.IconContent("Refresh"), EditorStyles.miniButton, GUILayout.Width(50)))
            {
                property.FindPropertyRelative("type").enumValueIndex = 0;
                property.FindPropertyRelative("slider").floatValue = defaultValue;
                property.FindPropertyRelative("timelineCurve").animationCurveValue = AnimationCurve.Linear(0.0f, defaultValue, 24.0f, defaultValue);
                property.FindPropertyRelative("sunCurve").animationCurveValue = AnimationCurve.Linear (-1.0f, defaultValue, 1.0f, defaultValue);
                property.FindPropertyRelative("moonCurve").animationCurveValue = AnimationCurve.Linear (-1.0f, defaultValue, 1.0f, defaultValue);
            }
            EditorGUILayout.EndHorizontal();
            
            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(property.FindPropertyRelative("type"), m_guiContent[4]);
                switch (property.FindPropertyRelative("type").enumValueIndex)
                {
                    case 0:
                        EditorGUILayout.Slider(property.FindPropertyRelative("slider"), minValue, maxValue, m_guiContent[0]);
                        maxValue -= minValue;
                        //GUI.enabled = false;
                        //EditorGUILayout.CurveField(property.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0, minValue, 24, maxValue), m_guiContent[1]);
                        //EditorGUILayout.CurveField(property.FindPropertyRelative("sunCurve"), Color.yellow, new Rect(-1.0f, minValue, 2.0f, maxValue), m_guiContent[2]);
                        //EditorGUILayout.CurveField(property.FindPropertyRelative("moonCurve"), Color.cyan, new Rect(-1.0f, minValue, 2.0f, maxValue), m_guiContent[3]);
                        //GUI.enabled = true;
                        break;
                    case 1:
                        //GUI.enabled = false;
                        //EditorGUILayout.Slider(property.FindPropertyRelative("slider"), minValue, maxValue, m_guiContent[0]);
                        maxValue -= minValue;
                        //GUI.enabled = true;
                        EditorGUILayout.CurveField(property.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0, minValue, 24, maxValue), m_guiContent[1]);
                        //GUI.enabled = false;
                        //EditorGUILayout.CurveField(property.FindPropertyRelative("sunCurve"), Color.yellow, new Rect(-1.0f, minValue, 2.0f, maxValue), m_guiContent[2]);
                        //EditorGUILayout.CurveField(property.FindPropertyRelative("moonCurve"), Color.cyan, new Rect(-1.0f, minValue, 2.0f, maxValue), m_guiContent[3]);
                        //GUI.enabled = true;
                        break;
                    case 2:
                        //GUI.enabled = false;
                        //EditorGUILayout.Slider(property.FindPropertyRelative("slider"), minValue, maxValue, m_guiContent[0]);
                        maxValue -= minValue;
                        //EditorGUILayout.CurveField(property.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0, minValue, 24, maxValue), m_guiContent[1]);
                        //GUI.enabled = true;
                        EditorGUILayout.CurveField(property.FindPropertyRelative("sunCurve"), Color.yellow, new Rect(-1.0f, minValue, 2.0f, maxValue), m_guiContent[2]);
                        //GUI.enabled = false;
                        //EditorGUILayout.CurveField(property.FindPropertyRelative("moonCurve"), Color.cyan, new Rect(-1.0f, minValue, 2.0f, maxValue), m_guiContent[3]);
                        //GUI.enabled = true;
                        break;
                    case 3:
                        //GUI.enabled = false;
                        //EditorGUILayout.Slider(property.FindPropertyRelative("slider"), minValue, maxValue, m_guiContent[0]);
                        maxValue -= minValue;
                        //EditorGUILayout.CurveField(property.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0, minValue, 24, maxValue), m_guiContent[1]);
                        //EditorGUILayout.CurveField(property.FindPropertyRelative("sunCurve"), Color.yellow, new Rect(-1.0f, minValue, 2.0f, maxValue), m_guiContent[2]);
                        //GUI.enabled = true;
                        EditorGUILayout.CurveField(property.FindPropertyRelative("moonCurve"), Color.cyan, new Rect(-1.0f, minValue, 2.0f, maxValue), m_guiContent[3]);
                        break;
                }
                EditorGUI.indentLevel--;
            }
        }
        
        private void DrawColorProperty(SerializedProperty property, ref AzureColorProperty targetProperty, GUIContent content)
        {
            EditorGUILayout.BeginHorizontal();
            property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, content);
            if (GUILayout.Button(EditorGUIUtility.IconContent("Refresh"), EditorStyles.miniButton, GUILayout.Width(50)))
            {
                Undo.RecordObject(m_target, "Undo Azure Sky Profile");
                targetProperty = new AzureColorProperty
                (
                    Color.white,
                    new Gradient(),
                    new Gradient(),
                    new Gradient()
                );
            }
            EditorGUILayout.EndHorizontal();
            
            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(property.FindPropertyRelative("type"), m_guiContent[4]);
                switch (property.FindPropertyRelative("type").enumValueIndex)
                {
                    case 0:
                        EditorGUILayout.PropertyField(property.FindPropertyRelative("color"), m_guiContent[14]);
                        //GUI.enabled = false;
                        //EditorGUILayout.PropertyField(property.FindPropertyRelative("timelineGradient"), m_guiContent[15]);
                        //EditorGUILayout.PropertyField(property.FindPropertyRelative("sunGradient"), m_guiContent[16]);
                        //EditorGUILayout.PropertyField(property.FindPropertyRelative("moonGradient"), m_guiContent[17]);
                        //GUI.enabled = true;
                        break;
                    case 1:
                        //GUI.enabled = false;
                        //EditorGUILayout.PropertyField(property.FindPropertyRelative("color"), m_guiContent[14]);
                        //GUI.enabled = true;
                        EditorGUILayout.PropertyField(property.FindPropertyRelative("timelineGradient"), m_guiContent[15]);
                        //GUI.enabled = false;
                        //EditorGUILayout.PropertyField(property.FindPropertyRelative("sunGradient"), m_guiContent[16]);
                        //EditorGUILayout.PropertyField(property.FindPropertyRelative("moonGradient"), m_guiContent[17]);
                        //GUI.enabled = true;
                        break;
                    case 2:
                        //GUI.enabled = false;
                        //EditorGUILayout.PropertyField(property.FindPropertyRelative("color"), m_guiContent[14]);
                        //EditorGUILayout.PropertyField(property.FindPropertyRelative("timelineGradient"), m_guiContent[15]);
                        //GUI.enabled = true;
                        EditorGUILayout.PropertyField(property.FindPropertyRelative("sunGradient"), m_guiContent[16]);
                        //GUI.enabled = false;
                        //EditorGUILayout.PropertyField(property.FindPropertyRelative("moonGradient"), m_guiContent[17]);
                        //GUI.enabled = true;
                        break;
                    case 3:
                        //GUI.enabled = false;
                        //EditorGUILayout.PropertyField(property.FindPropertyRelative("timelineGradient"), m_guiContent[15]);
                        //EditorGUILayout.PropertyField(property.FindPropertyRelative("sunGradient"), m_guiContent[16]);
                        //GUI.enabled = true;
                        EditorGUILayout.PropertyField(property.FindPropertyRelative("moonGradient"), m_guiContent[17]);
                        break;
                }
                EditorGUI.indentLevel--;
            }
        }
    }
}