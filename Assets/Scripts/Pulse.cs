using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Pulse : MonoBehaviour
{
    Image image;

    [Tooltip("How long in seconds it takes to complete a pulse")]
    [SerializeField] [Range(0, 2)] float duration = 2.0f;

    [Tooltip("Minimum alpha to pulse to")]
    [SerializeField] [Range(0.0f, 1.0f)] float minAlpha = 0.0f;

    [Tooltip("Maximum alpha to pulse to")]
    [SerializeField] [Range(0.0f, 1.0f)] float maxAlpha = 1.0f;

    IEnumerator Start()
    {
        image = GetComponent<Image>();

        while (true)
        {
            image.CrossFadeAlpha(minAlpha, duration, false);
            yield return new WaitForSeconds(duration);
            image.CrossFadeAlpha(maxAlpha, duration, false);
            yield return new WaitForSeconds(duration);
        }
    }
}