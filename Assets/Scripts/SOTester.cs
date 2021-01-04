using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyButtons;

public class SOTester : MonoBehaviour
{
    public CloudDialogue dialogueTest;    // Start is called before the first frame update
    public string testKey;
    void Start()
    {
        dialogueTest.ReadDialogueFromFile();
        Debug.Log(dialogueTest.DialogueByKey("Horse")[0]);
    }

    [Button]
    public void TestKey(){
        Debug.Log(dialogueTest.DialogueByKey(testKey)[0]);  
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
