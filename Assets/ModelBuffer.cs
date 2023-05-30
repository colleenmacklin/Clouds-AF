using MiniJSON;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ModelBuffer : MonoBehaviour
{

    [Header("Philosopher Model URL")]
    public string philosopher_model_url;
    [Header("Comedian Model URL")]
    public string comedian_model_url;
    [Header("Primordial Earth Model URL")]
    public string primordial_earth_model_url;


    [Header("HuggingFace Key API")]
    public string hf_api_key;

    //TODO: Dictionary

    public static event Action OnStartedLoadingModel;


    private void Awake()
    {
       
    }
    private void Start()
    {
        StartCoroutine(LoadModel());

    }


    private IEnumerator LoadModel()
    {
        string prompt = "some dummy text here";
        // Form the JSON
        var form = new Dictionary<string, object>();
        form["n"] = 1; //the number of generated texts
        form["inputs"] = prompt;

        var json = Json.Serialize(form);
        Debug.Log("JSON" + json);
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);

        // Make the web request
        //TODO: requests for each model?

        UnityWebRequest request = UnityWebRequest.Put(model_url, bytes);
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + hf_api_key);
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

        OnStartedLoadingModel?.Invoke();

        }


    private void OnDisable()
    {
        StopCoroutine(LoadModel());
    }
}

