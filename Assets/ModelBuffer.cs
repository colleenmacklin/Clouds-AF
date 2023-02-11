using MiniJSON;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ModelBuffer : MonoBehaviour
{

    [Header("HuggingFace Model URL")]
    public string model_url;

    [Header("HuggingFace Key API")]
    public string hf_api_key;

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
        UnityWebRequest request = UnityWebRequest.Put(model_url, bytes);
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + hf_api_key);
        request.method = "POST"; // Hack to send POST to server instead of PUT


        //webRequest = new UnityWebRequest(url);
        //webRequest.downloadHandler = new CustomWebRequest(bytes);
       // webRequest.SendWebRequest();


        yield return request.SendWebRequest();


        string data = request.downloadHandler.text;

        Debug.Log("progress" + request.downloadProgress);

        Debug.Log(data);
       
        request.Dispose(); //Colleen added to manage a memory leak. See documentation here: https://answers.unity.com/questions/1904005/a-native-collection-has-not-been-disposed-resultin-1.html

    }
}
