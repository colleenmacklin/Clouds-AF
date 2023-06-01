using MiniJSON;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR;
using static System.Net.WebRequestMethods;

public class ModelBuffer : MonoBehaviour
{
    /*
    [Header("Philosopher Model URL")]
    public string philosopher_model_url;
    [Header("Comedian Model URL")]
    public string comedian_model_url;
    [Header("Primordial Earth Model URL")]
    public string primordial_earth_model_url;
    */

    Dictionary<String, String> model_urls = new Dictionary<String, String>()
    {
        {"philosopher", "https://api-inference.huggingface.co/models/Triangles/fantastic_philosopher_124_4000"},
        {"comedian", "https://api-inference.huggingface.co/models/Triangles/comedian_4000_gpt_only"},
        {"primordial_earth", "https://api-inference.huggingface.co/models/Triangles/primordial_earth_gpt_content_only"}
    };


    //[Header("HuggingFace Key API")]
    //public string hf_api_key; //make static so that it can move to next scene

    private float _waitForCount = 30; //seconds
    private bool _finishedCount = false;

    public static event Action OnStartedLoadingModel;
    public string modelURL; //model that has been selected, passed to next Scene
    private string hf_api_key = "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";

    private void Awake()
    {
        ModelInfo.hf_api_key = hf_api_key; //setting static variable so it can be shared across scenes
    }
    private void Start()
    {
        //StartCoroutine(LoadModel()); //comment out and add to GetModel

    }

    private void OnEnable()
    {
        Actions.GetModel += GetModel;
    }

    private void Disable()
    {
        Actions.GetModel -= GetModel;
    }

    private void Update()
    {
        if (ModelInfo.modelURL == null)
            Debug.Log("no model selected yet...");
        {
            if (!_finishedCount)
            {
                if (_waitForCount > 0)
                {
                    _waitForCount -= Time.deltaTime;
                }
                else
                {
                    _finishedCount = true;
                    ModelInfo.modelURL = model_urls["philosopher"]; //use as default if player does not make a selection within 30seconds
                    modelURL = ModelInfo.modelURL; //just doing this to show the URL in the inspector, can remove once built
                    StartCoroutine(LoadModel());
                    Debug.Log("Loading MOdel");
                }
            }
        }
    }


    private IEnumerator LoadModel()
    {
        string prompt = "when I look up at the clouds";

        // Form the JSON
        var form = new Dictionary<string, object>();
        form["n"] = 1; //the number of generated texts
        form["inputs"] = prompt;

        var json = Json.Serialize(form);
        Debug.Log("JSON" + json);
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);

        // Make the web request

        //UnityWebRequest request = UnityWebRequest.Put(model_url, bytes); //old
        UnityWebRequest request = UnityWebRequest.Put(ModelInfo.modelURL, bytes); 
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + ModelInfo.hf_api_key);
        request.method = "POST"; // Hack to send POST to server instead of PUT

        yield return request.SendWebRequest();


        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log("failed request error: " + request.error);
            Debug.Log("failed downloadHandler.data: " + request.downloadHandler.data);
            //TODO: when a 503 error is thrown (model is not ready) set "wait_for_model" = true; remake the request
            string data = request.downloadHandler.text;
            Debug.Log(data);
        }
        else
        {
            string data = request.downloadHandler.text;
            // Process the result
            Debug.Log(data);

        }

        Debug.Log("progress" + request.downloadProgress);
        request.Dispose(); //Colleen added to manage a memory leak. See documentation here: https://answers.unity.com/questions/1904005/a-native-collection-has-not-been-disposed-resultin-1.html

        OnStartedLoadingModel?.Invoke(); //speaks to TitleCounter

        }

    public void GetModel(String _name)
    {
        //check name+URL pair in Dictionary
        switch (_name)
        {
            case "philosopher":
                ModelInfo.ModelName = "philosopher";
                ModelInfo.modelURL = model_urls["philosopher"];
                modelURL = ModelInfo.modelURL; //just doing this to show the URL in the inspector, can remove once built

                break;
            case "comedian":
                ModelInfo.ModelName = "comedian";
                ModelInfo.modelURL = model_urls["comedian"];
                modelURL = ModelInfo.modelURL; //just doing this to show the URL in the inspector, can remove once built

                break;
            case "primordial_earth":
                ModelInfo.ModelName = "primordial_earth";
                ModelInfo.modelURL = model_urls["primordial_earth"];
                modelURL = ModelInfo.modelURL; //just doing this to show the URL in the inspector, can remove once built

                break;
            default:
                ModelInfo.ModelName = "philosopher";
                ModelInfo.modelURL = model_urls["philosopher"]; //Pass on the philosopher model as a failsafe
                modelURL = ModelInfo.modelURL; //just doing this to show the URL in the inspector, can remove once built

                Debug.LogWarning("There is no valid model");
                break;
        }
        Debug.Log("model selected: "+_name);

    }


    private void OnDisable()
    {
        StopCoroutine(LoadModel());
    }
}

