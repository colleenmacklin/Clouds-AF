using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class EventTest : MonoBehaviour
{

    private UnityAction someListener;
    // private Action someListener; //https://stackoverflow.com/questions/42034245/unity-eventmanager-with-delegate-instead-of-unityevent

    void Awake()
    {
        someListener = new UnityAction(SomeFunction);
       // someListener = new Action(SomeFunction);
        Debug.Log("hello I am the game tester");
    }

    void OnEnable()
    {
        EventManager.StartListening("test", someListener);
        EventManager.StartListening("Spawn", SomeOtherFunction);
        EventManager.StartListening("Destroy", SomeThirdFunction);
    }

    void OnDisable()
    {
        EventManager.StopListening("test", someListener);
        EventManager.StopListening("Spawn", SomeOtherFunction);
        EventManager.StopListening("Destroy", SomeThirdFunction);
    }

    void SomeFunction()
    {
        Debug.Log("Some Function was called!");
    }

    void SomeOtherFunction()
    {
        Debug.Log("Some Other Function was called!");
    }

    void SomeThirdFunction()
    {
        Debug.Log("Some Third Function was called!");
    }
}
