using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SOTester : MonoBehaviour
{
    public CloudDialogue dialogueTest;    // Start is called before the first frame update
    public string testKey;
    void Start()
    {
        dialogueTest.ReadDialogueFromFile();
        Debug.Log(dialogueTest);
        Debug.Log(dialogueTest.DialogueByKey("poodle")[0]);
    }


    public void TestKey()
    {
        Debug.Log(dialogueTest.DialogueByKey(testKey)[0]);
    }

}
