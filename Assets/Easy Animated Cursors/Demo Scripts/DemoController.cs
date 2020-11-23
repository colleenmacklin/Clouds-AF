using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemoController : MonoBehaviour
{

    public GameObject CursorManager;
    public Sprite[] LoadingCursorArray;
    public Sprite[] ConnectingCursorArray;
    public Sprite[] KillSpriteArray;
    public Sprite MainCursor;
    public Text NotifierText;

    public GameObject ButtonLoad, ButtonConnect, ButtonKill;

    private CursorManager cursorScript;

    void Start()
    {
        cursorScript = CursorManager.GetComponent<CursorManager>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void LoadMyCursor()
    {
        cursorScript.SetCursor(LoadingCursorArray);
        cursorScript.IsAnimated = true;
        cursorScript.SetCursorSize(0.5f);
        cursorScript.SetAnimationSpeed(1000f);
        NotifierText.text = "Loading mindset...";
        Invoke("FakeLoadingBehaviour", 3f);
        ButtonLoad.SetActive(false);
    }

    /// <summary>
    ///  Button will call this
    /// </summary>
    public void ConnectMyCursor()
    {
        cursorScript.SetCursor(ConnectingCursorArray);
        cursorScript.IsAnimated = true;
        cursorScript.SetCursorSize(0.7f);
        cursorScript.SetAnimationSpeed(200);
        NotifierText.text = "Connecting to mind...";
        Invoke("FakeConnectionBehaviour", 8f);
        ButtonConnect.SetActive(false);
    }


    public void KillMyCursor()
    {
        cursorScript.SetCursor(KillSpriteArray);
        cursorScript.IsAnimated = true;
        cursorScript.SetCursorSize(1f);
        cursorScript.SetAnimationSpeed(120);
        NotifierText.text = "Terminating mindset...";
        Invoke("FakeKillingBehaviour", 5f);
        ButtonKill.SetActive(false);
    }

    public void CursorOnHoverStart()
    {
        cursorScript.SetCursorSize(0.4f);
    }

    public void CursorOnHoverEnd()
    {
        cursorScript.SetCursorSize(0.23f);
    }

    public void KillOnHoverStart()
    {
        cursorScript.SetCursor(KillSpriteArray);
        cursorScript.IsAnimated = true;
        cursorScript.SetAnimationSpeed(100f);
        cursorScript.SetCursorSize(0.4f);
        //cursorScript.StartAnimation();
        //cursorScript.openEyes();
    }

    public void KillOnHoverEnd()
    {
        cursorScript.StopAnimation();
        cursorScript.SetCursor(MainCursor);
        cursorScript.SetAnimationSpeed(500f);
        cursorScript.SetCursorSize(0.23f);
    }


    public void FakeConnectionBehaviour()
    {
        NotifierText.text = "Mind connection established";
        cursorScript.SetCursor(MainCursor);
        cursorScript.SetCursorSize(0.23f);
        ButtonKill.SetActive(true);
    }


    public void FakeLoadingBehaviour()
    {
        NotifierText.text = "Mindset Loaded";
        cursorScript.SetCursor(MainCursor);
        cursorScript.SetCursorSize(0.23f);
        ButtonConnect.SetActive(true);
    }

    public void FakeKillingBehaviour()
    {
        NotifierText.text = "Mind Connection Termianted";
        cursorScript.SetCursor(MainCursor);
        cursorScript.SetCursorSize(0.23f);
        ButtonLoad.SetActive(true);
    }
}
