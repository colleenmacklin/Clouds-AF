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

        DestroyButterfly();
        yield return null;
    }

    public void DestroyButterfly()
    {
        GameObject g = this.gameObject;
        Object.Destroy(g.transform.GetChild(0).gameObject);
    }

    private void SetButterflySpeed(float speed)
    {

        _animator.speed = speed;
    }


   

}
