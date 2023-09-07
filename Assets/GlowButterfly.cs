using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static ButterflyController;

public class GlowButterfly : MonoBehaviour
{
    [CanBeNull] //null in opening, referenced in main
    [SerializeField]
    private Raycaster _raycasterScript;

    [CanBeNull] //null in main, referenced in opening
    [SerializeField]
    private RaycasterOpening _raycasterOpening;

    [CanBeNull] //null in main, referenced in opening
    [SerializeField]
    private Opening _opening;

    public ButterflyController butterflyController;

    private MeshRenderer[] _meshRenderers = null;

    //flapping animator
    private Animator _animator;

    private Color _glowCol;
    private Color _startCol;
    private Color _currentCol;

    //this is for opening only 
    private GameObject _selectedCloudObject = null;

    private float _butterflyIdleSpeed = 1.3f;
    private bool _isGlowing = false;
    public Camera _camera;
    public Vector3 v3Pos;
    private Vector3 velocity = Vector3.zero;
    public float rotationSpeed = 1.0f;
    public float smoothTime = 0.9f; //0.3f
    float angle;
    public float maxTurnSpeed = 90;



    private void Awake()
    {
        if (_raycasterScript)
        {
            _raycasterScript.OnHoverOverTargetCloud += StartGlow;
            _raycasterScript.OnHoverExit += DeGlow;
        }

        if (_raycasterOpening)
        {
            _raycasterOpening.OnHoverOverTargetCloud += StartGlow;
            _raycasterOpening.OnHoverExit += DeGlow;

        }

        foreach (Camera c in Camera.allCameras)
        {
            if (c.gameObject.name == "Main Camera")
            {
                _camera = c;
            }
        }

    }


    private void Start()
    {

        _glowCol = new Color(60f, 60f, 60f, 60f);
        //Debug.Log("random point: " + v3Pos);


    }

    public void SetButteflyMatArray()
    {
        _meshRenderers = GetComponentsInChildren<MeshRenderer>();
        _startCol = _meshRenderers[1].material.GetColor("_Color");
        _animator = GetComponentInChildren<Animator>();

        SetButterflySpeed(_butterflyIdleSpeed);
    }



    //call this when hovering over cloud
    public void StartGlow(GameObject cloud)
    {
        if (transform.childCount > 0)
        {
            if (!_isGlowing)
            {

                StopAllCoroutines();
                StartCoroutine(glowBut());

            }

            _selectedCloudObject = cloud;

        }
    }

    public void DeGlow()
    {
        if (transform.childCount > 0)
        {
            if (_isGlowing)
            {

                StopAllCoroutines();
                StartCoroutine(UnglowBut());

            }


            //re-enable following-cursor script on ButterflyPrefab
            GetComponentInChildren<FollowCursor>().enabled = true;

            if (_meshRenderers != null)
            {
                foreach (MeshRenderer m in _meshRenderers)
                {
                    m.material.SetColor("_Color", _startCol);

                    m.material.SetFloat("_Amount", 0);

                }
                SetButterflySpeed(_butterflyIdleSpeed);
            }
        }

        //disable circling script on ButterflyContainer
        GetComponent<ButterflyCircle>().enabled = false;

    }

   

    private IEnumerator glowBut()
    {
        _isGlowing = true;

        //switch butterfly transform method by disabling ButterflyPrefab following-cursor script
        //GetComponentInChildren<FollowCursor>().enabled = false;
        //and enabling ButterflyContainer circling script
        //GetComponent<ButterflyCircle>().enabled = true;


        //TODO: Don't change underlying cloud while glowing
        float i = 0;
        float j = 0;
        Debug.Log("doing it");


        float speed = _butterflyIdleSpeed;
        while (j < 1)
        {
            _currentCol = Vector4.Lerp(_startCol, _glowCol, i);


            if (_meshRenderers != null)
            {
                foreach (MeshRenderer m in _meshRenderers)
                {
                    if (m)
                        m.material.SetColor("_Color", _currentCol);
                    if (i > 0.7f)
                    {
                        if (m)
                            m.material.SetFloat("_Amount", j);
                        j += 0.02f;
                    }

                }
                SetButterflySpeed(speed);
            }

            yield return new WaitForSeconds(.02f);//cm sped up from .05f

            i += 0.01f;
            speed += 0.2f;

        }
        //invoke glow is finished

        if (_raycasterScript)
        {
            _raycasterScript.StartCloudTalking();
            butterflyController.talkingButterfly();

        }
        if (_raycasterOpening)
        {
            _opening.GetClickedCloud(_selectedCloudObject);
        }
        _isGlowing = false;

        //fly offscreen

        //Actions.MoveButterfly();
        //butterflyController._bstate = ButterflyState.OFFSCREEN;

        //DestroyButterfly(); //this seems to happen before the MoveButterfly CoRoutine is finished - might need to call it from that script
        yield return null;
    }


    private IEnumerator UnglowBut()
    {
        _isGlowing = false;

        //TODO: Don't change underlying cloud while glowing
        float i = 0;
        float j = 0;

        float speed = _butterflyIdleSpeed;
        while (j < 1)
        {
            _currentCol = Vector4.Lerp(_glowCol, _startCol, i);


            if (_meshRenderers != null)
            {
                foreach (MeshRenderer m in _meshRenderers)
                {
                    if (m)
                        m.material.SetColor("_Color", _startCol);
                    if (i > 0.7f)
                    {
                        if (m)
                            m.material.SetFloat("_Amount", j);
                        j += 0.02f;
                    }

                }
                SetButterflySpeed(speed);
            }

            yield return new WaitForSeconds(.02f);//cm sped up from .05f

            i += 0.01f;
            speed += 0.2f;

        }
        //invoke Deglow is finished
        yield return null;
    }



    //TODO: Move to Butterfly Controller
    public void DestroyButterfly()
    {
        Debug.Log("Destroy!!!Butterfly!!!!");
        StopAllCoroutines();

        if (_raycasterScript)
        {
            GameObject g = this.gameObject;
            Object.Destroy(g.transform.GetChild(0).gameObject);
        }

        else
        {
            if (GetComponentInChildren<FollowCursor>() != null)
            {
                GameObject buttefly = GetComponentInChildren<FollowCursor>().gameObject;
                buttefly.SetActive(false);
            }

        }

    }

    private void SetButterflySpeed(float speed)
    {
        _animator.speed = speed;
    }




}
