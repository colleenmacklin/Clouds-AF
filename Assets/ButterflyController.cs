using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI.Table;

public class ButterflyController : MonoBehaviour
{
    [CanBeNull] //is null in opening, needs to be refereneced in main scene
    [SerializeField]
    private Storyteller _storyTeller;

    public GameObject Butterfly;
    public float startDelay;
    Camera mainCam;
    float zPosStart;
    float moveTime = 2.0f;
    private Vector3 targetPos;
    private Vector3 initialButterflyRotaion;
    private GlowButterfly _glowButterfly;
    public Vector3 _mousePos;
    public Vector3 _butterflyRotInitial;
    public bool _ismoving;
    public bool _isTalking;

    public enum ButterflyState
    {
        OFFSCREEN,
        ONSCREEN
    }

    public ButterflyState _bstate;
    public event Action MoveButterfly;

    private void Awake()
    {
        MoveButterfly += Move;
        _glowButterfly = GetComponent<GlowButterfly>();

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            NewButterfly();
        }
        else
        {
            _storyTeller.OnIntroComplete += NewButterfly;
            TextBoxController.OnDialogueComplete += NewButterfly; //TODO: Remove if keeping butterfly onscreen
        }
        if (mainCam == null)
            mainCam = Camera.main;


    }

    private void OnEnable()
    {
        Actions.MoveButterfly += Move;
    }



    private void OnDisable()
    {
        if (_storyTeller)
        {
            _storyTeller.OnIntroComplete -= NewButterfly;
        }
        Actions.MoveButterfly -= Move;
    }


    private void NewButterfly()
    {
        _isTalking = false;
        // save our z position
        zPosStart = transform.position.z;

        //set the position offscreen as a random one
        targetPos = mainCam.ViewportToWorldPoint(new Vector3(ranPos(), ranPos(), 10.0f));

        //for now just instantiate
        if (this.transform.childCount < 1)
        {
            //find a position offscreen
            this.transform.position = targetPos;

            Instantiate(Butterfly, this.transform);
            _butterflyRotInitial = Butterfly.transform.eulerAngles;

            //move Butterfly in 
            _bstate = ButterflyState.OFFSCREEN;
            Move();
            _bstate = ButterflyState.ONSCREEN;

        }
        //set array 
        _glowButterfly.SetButteflyMatArray();
        GetComponentInChildren<Shake>().enabled = false; //turn off shake


    }

    public void talkingButterfly()
    {
        GetComponentInChildren<FollowCursor>().enabled = false; //take over movement
        _glowButterfly.DeGlow();
        GetComponentInChildren<Shake>().enabled = true; //take over movement
    }

    //

    public void Move()
    {
        if (!Butterfly)
        {
            print("can't find butterfly prefab");
            return;
        }

        //set target position to move to (Mouse or a random offscreen position)
        Butterfly.GetComponent<FollowCursor>().enabled = false;


        switch (_bstate)
        {
            case ButterflyState.OFFSCREEN: //move butterfly onscreen
                print("moving butterfly onscreen");
                //set the ending position (target) to mouse
                _mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
                var mouseVector = new Vector3(_mousePos.x, _mousePos.y, 100);
                targetPos = mouseVector; //set the butterfly to the mouse cursor
                targetPos.z = zPosStart;
                StartCoroutine(moveBut(targetPos));
                //Butterfly.GetComponent<FollowCursor>().enabled = true;

                break;

            case ButterflyState.ONSCREEN: //move butterfly offscreen
                print("moving butterfly offscreen");
                targetPos = mainCam.ViewportToWorldPoint(new Vector3(ranPos(), ranPos(), 10.0f));
                StartCoroutine(moveBut(targetPos));
                _glowButterfly.DestroyButterfly();

                break;

            default:
                print("Not sure what to do: " + _bstate);

                break;
        }
    }




        IEnumerator moveBut(Vector3 tPos)
        {
            
            _ismoving = true;
    
            //yield return new WaitForSeconds(startDelay);

            Vector2 startPos = Butterfly.transform.position; //set as a random offscreen location in the NewButterfly() function
            float elapsedTime = 0.0f;
            // create our goal position
            Vector3 goalPos = tPos; //set the butterfly to the mouse cursor
            Vector3 currentRot = Butterfly.transform.eulerAngles;

            while (elapsedTime <= moveTime)
            {
                transform.position = Vector2.Lerp(startPos, goalPos, (elapsedTime / moveTime));
                transform.localRotation = Quaternion.Slerp(Butterfly.transform.localRotation, Quaternion.Euler(_butterflyRotInitial), elapsedTime / moveTime);
                elapsedTime += Time.deltaTime;
                //print("done moving butterfly, time: "+elapsedTime);

            yield return null;
            }

            print("done moving butterfly");
            _ismoving = false;

    }



    //creates a random position offscreen
    private float ranPos()
        {
            float pos = UnityEngine.Random.Range(1.0f, 2.1f);
            int sign = UnityEngine.Random.Range(0, 2);
            if (sign == 1)
            {
                pos = -pos;
            }
            else
            {
                pos = +pos;
            }
            return pos;
        }


    }
