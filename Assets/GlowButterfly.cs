using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    private MeshRenderer[] _mats;

    private Animator _animator;

    private Color _glowCol;
    private Color _startCol;
    private Color _currentCol;

    //this is for opening only 
    private GameObject _selectedCloudObject = null;

    private float _butterflyIdleSpeed = 1.3f;
    private bool _isGlowing = false;
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
    }
        

    private void Start()
    {
        
        _glowCol = new Color(60f, 60f, 60f, 60f);
        
    }

    public void SetButteflyMatArray()
    {
        _mats = GetComponentsInChildren<MeshRenderer>();
        _startCol = _mats[1].material.GetColor("_Color");
        _animator = GetComponentInChildren<Animator>();
        SetButterflySpeed(_butterflyIdleSpeed);
    }
  


    //call this when hovering over cloud
    public void StartGlow(GameObject cloud)
    {
        if (!_isGlowing)
        {
            StartCoroutine(glowBut());

        }

        _selectedCloudObject = cloud;
    }

    public void DeGlow()
    {
        //TODO make this a fade out 
        StopAllCoroutines();
        _isGlowing = false;
        foreach (MeshRenderer m in _mats)
        {
            m.material.SetColor("_Color", _startCol);

            m.material.SetFloat("_Amount", 0);

        }

        SetButterflySpeed(_butterflyIdleSpeed);
    }

    private IEnumerator glowBut()
    {
        _isGlowing = true;
        //TODO: Don't change underlying cloud while glowing
        float i = 0;
        float j = 0;
        Debug.Log("doing it");


        float speed = _butterflyIdleSpeed;
        while (j < 1)
        {
            
            _currentCol = Vector4.Lerp(_startCol, _glowCol, i);
            
            foreach (MeshRenderer m in _mats)
            {
                m.material.SetColor("_Color", _currentCol);
                if (i > 0.7f)
                {
                    m.material.SetFloat("_Amount", j);
                    j += 0.02f;
                }

            }
            SetButterflySpeed(speed);
            yield return new WaitForSeconds(.05f);
            i += 0.01f;
            speed += 0.2f;

        }
        //invoke glow is finished

        if (_raycasterScript)
        {

        _raycasterScript.StartCloudTalking();
        }
        if(_raycasterOpening)
        {
            _opening.GetClickedCloud(_selectedCloudObject);
        }
        _isGlowing = false;

        DestroyButterfly();
        yield return null;
    }

    public void DestroyButterfly()
    {
        if (_raycasterScript)
        {
            GameObject g = this.gameObject;
            Object.Destroy(g.transform.GetChild(0).gameObject);
        }

        else
        {
            GameObject buttefly = GetComponentInChildren<FollowCursor>().gameObject;
            buttefly.SetActive(false);
        }
       
    }

    private void SetButterflySpeed(float speed)
    {

        _animator.speed = speed;
    }


  

}
