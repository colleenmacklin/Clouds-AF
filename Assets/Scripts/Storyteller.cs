using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storyteller : MonoBehaviour
{
    [SerializeField]
    NarratorSO narrator;
    // Start is called before the first frame update
    void Start()
    {
        narrator.ProcessNarratorFile();
        Debug.Log(narrator.StoryData[0].Name);
        Debug.Log(narrator.StoryData[0].Content["unicorn"][0]);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
