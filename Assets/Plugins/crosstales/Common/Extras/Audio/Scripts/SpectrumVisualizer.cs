using UnityEngine;

namespace Crosstales.Common.Audio
{
   /// <summary>Simple spectrum visualizer.</summary>
   public class SpectrumVisualizer : MonoBehaviour
   {
      #region Variables

      ///<summary>FFT-analyzer with the spectrum data.</summary>
      [Tooltip("FFT-analyzer with the spectrum data.")] public FFTAnalyzer Analyzer;

      ///<summary>Prefab for the frequency representation.</summary>
      [Tooltip("Prefab for the frequency representation.")] public GameObject VisualPrefab;

      ///<summary>Width per prefab.</summary>
      [Tooltip("Width per prefab.")] public float Width = 0.075f;

      ///<summary>Gain-power for the frequency.</summary>
      [Tooltip("Gain-power for the frequency.")] public float Gain = 70f;

      ///<summary>Frequency band from left-to-right (default: true).</summary>
      [Tooltip("Frequency band from left-to-right (default: true).")] public bool LeftToRight = true;

      ///<summary>Opacity of the material of the prefab (default: 1).</summary>
      [Tooltip("Opacity of the material of the prefab (default: 1).")] [Range(0f, 1f)] public float Opacity = 1f;

      private Transform _tf;
      private Transform[] _visualTransforms;

      private Vector3 _visualPos = Vector3.zero;

      private int _samplesPerChannel;

      #endregion


      #region MonoBehaviour methods

      private void Start()
      {
         _tf = transform;
         _samplesPerChannel = Analyzer.Samples.Length / 2;
         _visualTransforms = new Transform[_samplesPerChannel];

         for (int ii = 0; ii < _samplesPerChannel; ii++)
         {
            //cut the upper frequencies >11000Hz
            GameObject tempCube;

            if (LeftToRight)
            {
               Vector3 position = _tf.position;
               tempCube = Instantiate(VisualPrefab, new Vector3(position.x + ii * Width, position.y, position.z), Quaternion.identity);
            }
            else
            {
               Vector3 position = _tf.position;
               tempCube = Instantiate(VisualPrefab, new Vector3(position.x - ii * Width, position.y, position.z), Quaternion.identity);
            }

            tempCube.GetComponent<Renderer>().material.color = Crosstales.Common.Util.BaseHelper.HSVToRGB(360f / _samplesPerChannel * ii, 1f, 1f, Opacity);

            _visualTransforms[ii] = tempCube.GetComponent<Transform>();
            _visualTransforms[ii].parent = _tf;
         }
      }

      private void Update()
      {
         for (int ii = 0; ii < _visualTransforms.Length; ii++)
         {
            _visualPos.Set(Width, Analyzer.Samples[ii] * Gain, Width);
            _visualTransforms[ii].localScale = _visualPos;
         }
      }

      #endregion
   }
}
// © 2015-2023 crosstales LLC (https://www.crosstales.com)