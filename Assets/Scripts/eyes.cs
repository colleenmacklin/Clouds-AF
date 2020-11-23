using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eyes : MonoBehaviour
{
    //public GameObject openEye;
    //public GameObject closeEye;
    //public GameObject glowEye;
    public Texture2D eye;


    void OnEnable()
    {
        EventManager.StartListening("openEye", openEyes);
        EventManager.StartListening("closeEye", closeEyes);
        //EventManager.StartListening("glowEye", glowEye);



    }

    void OnDisable()
    {
        EventManager.StopListening("openeye", openEyes);
        EventManager.StopListening("closeeye", closeEyes);
        //EventManager.StopListening("glowEye", glowEye);
    }


    // Start is called before the first frame update
    void Start()
    {
        //openEye.SetActive(false);

        //change eyes to Texture2D
        Cursor.SetCursor(eye, Vector2.zero, CursorMode.Auto);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void openEyes()
    {
        //openEye.SetActive(true);
        //closeEye.SetActive(true);

    }

    void closeEyes()
    {
        //closeEye.SetActive(true);
        //openEye.SetActive(false);
    }

}
