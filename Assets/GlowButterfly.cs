using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowButterfly : MonoBehaviour
{

    [SerializeField]
    private Raycaster _raycasterScript;

    private MeshRenderer[] _mats;

    private Animator _animator;

    private Color _glowCol;
    private Color _startCol;
    private Color _currentCol;


    private float _butterflyIdleSpeed = 1.3f;
    private bool _isGlowing = false;
    private void Awake()
    {
        
        _raycasterScript.OnHoverOverTargetCloud += StartGlow;
        _raycasterScript.OnHoverExit += DeGlow;
    }

    private void Start()
    {
        
        _glowCol = new Color(45f, 45f, 45f, 45f);
        
    }

    public void SetButteflyMatArray()
    {
        _mats = GetComponentsInChildren<MeshRenderer>();
        _startCol = _mats[1].material.GetColor("_Color");
        _animator = GetComponentInChildren<Animator>();
        SetButterflySpeed(_butterflyIdleSpeed);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            StartGlow();
        }
    }


    //call this when hovering over cloud
    public void StartGlow()
    {
        if (!_isGlowing)
        {
            StartCoroutine(glowBut());

        }

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
            i += 0.005f;
            speed += 0.05f;

        }
        //invoke glow is finished

        _raycasterScript.StartCloudTalking();
        _isGlowing = false;

        GameObject g = this.gameObject;
        Object.Destroy(g.transform.GetChild(0).gameObject);
        yield return null;
    }

    private void SetButterflySpeed(float speed)
    {

        _animator.speed = speed;
    }


    //todo 
    //spawn butterfly on start fly in
    //hook into when hovering over object
    //turn off current weird mouse stuff
    //animate butterfly flapping
    //separate camera?
    //fly in from side 
    //hook into when dialogue is done, fly in from side 
    //start dialogue when glow is done
    // decrease glow when move away from cloud
    //flap as move w speed as fast as mouse moves?
    //also the camera stuff?

}
