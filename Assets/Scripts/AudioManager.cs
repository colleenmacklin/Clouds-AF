using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip defaultAmbiance;
    public static AudioManager instance;
    private AudioSource track01, track02;
    private bool isPLayingTrack01;

    public void Awake()
    {
        if (instance == null){
            instance = this;
        }
    }

    private void Start()
    {
        track01 = gameObject.AddComponent<AudioSource>();
        track02 = gameObject.AddComponent<AudioSource>();
        isPLayingTrack01 = true;
        swapTrack(defaultAmbiance);
    }
    public void swapTrack(AudioClip newClip)
    {
        StopAllCoroutines();
        StartCoroutine(FadeTrack(newClip));

        isPLayingTrack01 = !isPLayingTrack01;

    }
    public void returnToDefaul() {
        swapTrack(defaultAmbiance);
    }
    private IEnumerator FadeTrack(AudioClip newClip)
    {
        float timeToFade = 2.25f;
        float timeElapsed = 0;

        if (isPLayingTrack01)
        {
            track02.clip = newClip;
            track02.Play();
            track02.loop = true;
            while (timeElapsed < timeToFade)
            {
                track02.volume = Mathf.Lerp(0, 1, timeElapsed / timeToFade);
                track01.volume = Mathf.Lerp(1, 0, timeElapsed / timeToFade);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            track01.Stop();
        }
        else
        {
            track01.clip = newClip;
            track01.Play();
            track01.loop = true;

            while (timeElapsed < timeToFade)
            {
                track01.volume = Mathf.Lerp(0, 1, timeElapsed / timeToFade);
                track02.volume = Mathf.Lerp(1, 0, timeElapsed / timeToFade);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            track02.Stop();
        }
        isPLayingTrack01 = !isPLayingTrack01;

    }
}
