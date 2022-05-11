using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audio_effects : MonoBehaviour
{
    public GameObject sound;
    public int startingPitch = 4;
    public int timeToDecrease = 5;
    AudioSource audioSource;


    // Start is called before the first frame update
    void onEnable()
    {
        EventManager.StartListening("sunset", lowerPitch);
    }

    void onDisable()
    {
        EventManager.StopListening("sunset", lowerPitch);
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        //Initialize the pitch
        audioSource.pitch = startingPitch;

    }

    // Update is called once per frame
    void Update()
    {
        if (audioSource.pitch > 0)
        {
            audioSource.pitch -= Time.deltaTime * startingPitch / timeToDecrease;
        }

    }

    void lowerPitch()
    {
        //Debug.Log("lowering pitch");
        if (audioSource.pitch > 0)
        {
            audioSource.pitch -= Time.deltaTime * startingPitch / timeToDecrease;
        }

    }
}
